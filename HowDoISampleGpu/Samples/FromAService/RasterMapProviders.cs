using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

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
    /// Four raster services, and the only styled basemap among them is the one that
    /// cannot be anything else.
    /// <para>
    /// ThinkGeo Cloud publishes Light, Dark and Hybrid as raster tiles as well, in a
    /// v1 and a v2 of each, and cells for them used to stand here. They are the same
    /// maps Vector Map Providers already shows, flattened to pixels on the server, so
    /// what a reader got from tapping along the strip was the provider's catalogue
    /// rather than anything about rasters. The aerial imagery is the one that has to
    /// arrive as pixels: a photograph has no geometry to send.
    /// </para>
    /// </summary>
    private static readonly (string Name, string Shows, string Credit)[] Providers =
    {
        ("ThinkGeo Aerial", "Aerial imagery over Frisco, Texas",
            "Imagery \u00a9 ThinkGeo"),
        ("OpenStreetMap", "The community's own street map, drawn to pixels",
            "\u00a9 OpenStreetMap contributors"),
        ("WMS NASA", "VIIRS true colour, the whole continent two days back",
            "VIIRS imagery courtesy NASA EOSDIS GIBS \u00b7 \u00a9 OpenStreetMap contributors"),
        ("WMTS Swisstopo", "The Swiss national map, on the grid the server declares",
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
        // X2, not X1. A phone's screen is two or three device pixels to the point,
        // so the X1 tile - 512 pixels over the ground the X2 tile covers with 1024 -
        // is magnified before it is ever shown and the imagery reads as soft. The
        // tile is four times the bytes and worth it: this is the one provider here
        // whose subject IS the pixels.
        0 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2),
        // No X2 counterpart: openstreetmap.org serves one 256-pixel tile set and
        // nothing else, which is part of what the cell is showing.
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
    /// FastMode is what makes this one usable on a phone. Opening a WMS normally begins
    /// by fetching GetCapabilities and adopting what it says; NASA's is 2.3 MB listing
    /// 1,610 layers, and all of it has to arrive and parse before the first tile is
    /// asked for. On a desktop that is a second nobody notices. On a mobile network it
    /// can outlast the client's own twenty-second timeout, and then the open throws and
    /// the sample reports a server that is in fact answering perfectly well - which is
    /// exactly how it failed. Nothing in that document is needed here: the layer, the
    /// format and the coordinate system are all known, so they are set and the fetch is
    /// skipped. FastMode cannot learn the CRS from the server, so it is named.
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

            // A WMTS has to read its capabilities - the tile matrix set is in them and
            // nothing else knows it - and this server's is 1.8 MB describing 688 layers.
            // That is a tenth of a second on a desk and can be most of a minute on a
            // train, so the twenty seconds a desktop client defaults to is not a limit
            // this server is failing to meet, it is a limit set for a different network.
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

            // What is on the screen, not the name of the class that fetched it: the
            // cell above already says which service this is, and the class follows
            // from the cell.
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
