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
    // OpenStreetMap's tile policy asks every application for a descriptive user agent.
    private const string UserAgent = "ThinkGeo HowDoI Sample (support@thinkgeo.com)";

    private static readonly string[] Providers =
    {
        "ThinkGeo Cloud - Light",
        "ThinkGeo Cloud - Dark",
        "ThinkGeo Cloud - Aerial",
        "ThinkGeo Cloud - Hybrid",
        "OpenStreetMap",
        "WMS - NASA satellite imagery",
        "WMTS - Swiss national map",
    };

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(Providers, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    /// <summary>The line that differs: which class serves the tiles.</summary>
    private static RasterTileSource OpenChosen(int index) => index switch
    {
        0 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Light_V2_X1),
        1 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Dark_V2_X1),
        2 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1),
        3 => new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Hybrid_V2_X1),
        4 => new OpenStreetMapRasterTileSource(UserAgent),
        5 => OpenNasaImagery(),
        _ => OpenSwisstopo(),
    };

    /// <summary>
    /// A WMS server. The layer names, the output format and any extra parameters are
    /// the server's own vocabulary, read from its capabilities document.
    /// </summary>
    private static WmsRasterTileSource OpenNasaImagery()
    {
        var imagery = new WmsRasterTileSource(new Uri("https://gibs.earthdata.nasa.gov/wms/epsg3857/best/wms.cgi"));
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
        };

    private async Task ShowAsync(int index)
    {
        try
        {
            var source = OpenChosen(index);
            MapStyle style;

            if (index == 5)
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
            _status.Text = source.GetType().Name;
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
        if (index == 5)
        {
            // North America, at a scale where a day of weather is the subject.
            _map.CenterPoint = new PointShape(-10500000, 4500000);
            _map.MapScale = 25000000;
            return;
        }

        if (index == 6 && source is WmtsRasterTileSource wmts)
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
