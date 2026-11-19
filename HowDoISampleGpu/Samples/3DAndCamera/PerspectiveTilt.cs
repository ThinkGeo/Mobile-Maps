using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The camera tilted off straight-down, over a vector basemap whose buildings stand
/// up, and over a raster one where the same tilt is a flat sheet in perspective.
/// </summary>
public class PerspectiveTilt : SamplePage
{
    private static readonly RectangleShape MidtownManhattan =
        new(-8_236_900, 4_975_700, -8_235_100, 4_974_330);

    private static readonly PointShape SanFranciscoBay = new(-13_630_000, 4_540_000);

    private MapView _map;
    private GpuBasemap _basemap;
    private Switch _buildings;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(new[] { "Vector - Manhattan, tilted", "Raster - San Francisco Bay" }, 0,
            index => _ = ShowBasemapAsync(index == 0));

        var row = new HorizontalStackLayout { Spacing = 8 };
        _buildings = new Switch { IsToggled = true };
        _buildings.Toggled += (_, _) =>
        {
            if (_basemap == null) return;
            // A renderer switch, not a restyle: no tile is rebuilt.
            _basemap.IsExtrusionDrawingEnabled = _buildings.IsToggled;
            _ = _map.RefreshAsync();
        };
        row.Add(new Label { Text = "Buildings", FontSize = 14, VerticalOptions = LayoutOptions.Center });
        row.Add(_buildings);
        AddToPanel(row);

        _ = ShowBasemapAsync(vector: true);
    }

    private async Task ShowBasemapAsync(bool vector)
    {
        // A raster basemap and a vector one are different styles over different
        // sources, so this is the one case where the basemap really is rebuilt.
        _basemap = new GpuBasemap(vector
            ? new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            })
            : new MapStyle().AddRaster(
                new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Dark_V2_X1),
                sourceId: "thinkgeo"));

        _basemap.IsExtrusionDrawingEnabled = _buildings.IsToggled;
        _map.Basemap = _basemap;

        if (vector)
        {
            _map.CurrentExtent = MidtownManhattan;
            await _map.RefreshAsync();
            try
            {
                await _map.TiltAsync(50, 600, Easing.CubicInOut);
            }
            catch (OperationCanceledException)
            {
                // The other basemap was picked before the tilt finished.
            }
        }
        else
        {
            // Raster tiles carry their labels as pixels: a zoom of their own draws them
            // crisp, a scale between two shrinks or stretches them.
            _map.CenterPoint = SanFranciscoBay;
            _map.MapScale = new ZoomLevelSet().ZoomLevel12.Scale;
            await _map.RefreshAsync();
        }
    }
}
