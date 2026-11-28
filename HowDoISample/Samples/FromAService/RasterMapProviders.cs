using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The same map from several raster services. Tile providers, a WMS server and a WMTS
/// server all arrive the same way - a raster source in a style - so they belong in one
/// place: the question is "how do I put a raster service on the map", not "what is the
/// difference between WMS and WMTS".
/// </summary>
public class RasterMapProviders : SamplePage
{
    // OpenStreetMap's tile policy asks every application for a clear, unique user
    // agent that names it - a library default is blocked. It also asks for the credit
    // on the map, which the page carries, and forbids caching the tiles for offline
    // use, which is why this one cell alone is given no FileTileCache.
    private const string UserAgent = "ThinkGeo Maps sample gallery (support@thinkgeo.com)";

    /// <summary>
    /// Four raster services. The aerial imagery is the one that has to arrive as
    /// pixels: a photograph has no geometry to send.
    /// </summary>
    private static readonly (string Name, string Shows, string Credit)[] Providers =
    {
        ("ThinkGeo Aerial", "Aerial imagery of Frisco, Texas",
            "Imagery \u00a9 ThinkGeo"),
        ("OpenStreetMap", "OpenStreetMap's standard tiles",
            "\u00a9 OpenStreetMap contributors"),
        ("WMS NASA", "NASA VIIRS true colour, two days ago",
            "VIIRS imagery courtesy NASA EOSDIS GIBS \u00b7 \u00a9 OpenStreetMap contributors"),
        ("WMTS Swisstopo", "Swiss national map, Switzerland only",
            "\u00a9 swisstopo"),
    };

    private const int NasaIndex = 2;
    private const int SwisstopoIndex = 3;

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoiceStrip(Providers.Select(p => p.Name).ToList(), 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    /// <summary>The line that differs: which class serves the tiles.</summary>
    private static RasterTileSource OpenChosen(int index) => index switch
    {
        // X2, not X1: a phone's screen is two or three device pixels to the point, so
        // the 512-pixel X1 tile would be magnified and the imagery would read as soft.
        0 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2),
        // openstreetmap.org serves one 256-pixel tile set.
        1 => new OpenStreetMapRasterTileSource(UserAgent),
        NasaIndex => OpenNasaImagery(),
        _ => OpenSwisstopo(),
    };

    /// <summary>
    /// A WMS server. The layer names, the output format and any extra parameters are
    /// the server's own vocabulary, read from its capabilities document - or, as here,
    /// known in advance and stated.
    /// </summary>
    /// <remarks>
    /// Opening a WMS normally begins by fetching GetCapabilities and adopting what it
    /// says. This server's is 2.3 MB listing more than a thousand layers, which on a
    /// mobile network can outlast the client's timeout. Nothing in that document is
    /// needed here: the layer, the format and the coordinate system are known, so
    /// they are set and FastMode skips the fetch. FastMode cannot learn the CRS from
    /// the server, so it is named.
    /// </remarks>
    private static WmsRasterTileSource OpenNasaImagery()
    {
        var imagery = new WmsRasterTileSource(new Uri("https://gibs.earthdata.nasa.gov/wms/epsg3857/best/wms.cgi"));
        imagery.Client.FastMode = true;
        imagery.Client.Crs = "EPSG:3857";
        imagery.Client.OutputFormat = "image/png";
        imagery.Client.ActiveLayerNames.Add("VIIRS_NOAA20_CorrectedReflectance_TrueColor");

        // Two days back: the newest day's mosaic can still be empty while it is assembled.
        imagery.Client.Parameters["TIME"] = DateTime.UtcNow.AddDays(-2).ToString(
            "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        // A server that needs credentials gets them on every request here, for example:
        // imagery.SendingHttpRequest += (_, e) => e.HttpRequestMessage.Headers.Authorization =
        //     new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-token");

        return imagery;
    }

    /// <summary>
    /// A WMTS server, which serves a fixed tile grid: TileMatrixSetName picks the
    /// web-mercator one from among those the server declares.
    /// </summary>
    private static WmtsRasterTileSource OpenSwisstopo()
        => new(new Uri("https://wmts.geo.admin.ch/EPSG/3857/1.0.0"), WmtsServerEncodingType.Restful)
        {
            ActiveLayerName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale",
            ActiveStyleName = "default",
            OutputFormat = "image/png",
            TileMatrixSetName = "3857_19",

            // A WMTS has to read its capabilities - the tile matrix set is in them -
            // and this server's is 1.8 MB, which a mobile network can take longer than
            // the default timeout to deliver.
            TimeoutInSeconds = 60,
        };

    private async Task ShowAsync(int index)
    {
        try
        {
            var source = OpenChosen(index);
            MapStyle style;

            if (index == NasaIndex)
            {
                // The imagery goes into the basemap's own layers rather than over them -
                // above the fills, below the lines and labels. Nothing is reordered: the
                // slot is found by walking the layers the style already declared.
                var basemapTiles = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
                {
                    VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
                };

                style = new MapStyle();
                style.AddStyle(ThinkGeoVectorStyles.Dark, basemapTiles);
                style.AddRaster(source, slot: StyleLayerSlot.AboveFills);
            }
            else
            {
                style = new MapStyle().AddRaster(source);
            }

            if (_map.Basemap == null)
            {
                _map.Basemap = new GpuBasemap(style);
            }
            else
            {
                await _map.Basemap.SetStyleAsync(style);
            }

            await ArriveAsync(index, source);

            // What is on the screen.
            _status.Text = Providers[index].Shows;

            // The credit follows the cell: each of these four is somebody else's map.
            SetAttribution(Providers[index].Credit);
        }
        catch (Exception exception)
        {
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    /// <summary>Each service has somewhere worth looking; the tile providers share one.</summary>
    private async Task ArriveAsync(int index, RasterTileSource source)
    {
        if (index == NasaIndex)
        {
            // North America, at a scale where a day of weather is the subject.
            _map.CenterPoint = new PointShape(-10500000, 4500000);
            _map.MapScale = 25000000;
            return;
        }

        if (index == SwisstopoIndex && source is WmtsRasterTileSource wmts)
        {
            // Fit the view to the extent the server declares for this layer. Opening
            // the source is what reads the capabilities document that says so.
            // (This public server can be slow.)
            await wmts.OpenAsync();
            var boundingBox = wmts.GetBoundingBox();
            _map.CenterPoint = boundingBox.GetCenterPoint();
            _map.MapScale = MapUtil.GetScale(_map.MapUnit, boundingBox, _map.CanvasWidth, _map.CanvasHeight);
            return;
        }

        _map.CenterPoint = new PointShape(-10778800, 3915300);

        // Raster tiles carry their labels as pixels: a zoom of their own draws them
        // crisp, a scale between two shrinks or stretches them.
        _map.MapScale = new ZoomLevelSet().ZoomLevel13.Scale;
    }
}
