using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same map from several raster tile providers. A new provider is a new style
/// over the same map: the raster registration is swapped in place, the camera holds.
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
    };

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(-10778800, 3915300);

        // Raster tiles carry their labels as pixels: a zoom of their own draws them
        // crisp, a scale between two shrinks or stretches them.
        map.MapScale = new ZoomLevelSet().ZoomLevel13.Scale;

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
        _ => new OpenStreetMapRasterTileSource(UserAgent),
    };

    private async Task ShowAsync(int index)
    {
        try
        {
            var source = OpenChosen(index);
            var style = new MapStyle().AddRaster(source);

            if (_map.Basemap == null)
            {
                _map.Basemap = new GpuBasemap(style);
            }
            else
            {
                await _map.Basemap.SetStyleAsync(style);
            }

            _status.Text = source.GetType().Name;
        }
        catch (Exception exception)
        {
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
