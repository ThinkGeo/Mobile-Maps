using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Zoom in and out, to a scale, to a layer's extent and to a point - and read where
/// the camera ended up.
/// </summary>
public class ZoomToExtents : SamplePage
{
    private const string BoundaryStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "boundary",
          "paint": {
              "fill-color": "rgba(100,150,255,0.10)",
              "fill-outline-color": "#C8C8C8" }
        }
      ]
    }
    """;

    private ShapeFileFeatureSource _friscoCityBoundary;
    private MapView _map;
    private Label _readout;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var shapefilePath = SampleShared.Shapefile("City_ETJ.shp");

        // The boundary never changes, so it is cut into tiles and drawn with the
        // basemap in one pass rather than by an overlay above it.
        var boundary = new FeatureSourceVectorTileSource();
        boundary.FeatureSources.Add("boundary", new ShapeFileFeatureSource(shapefilePath)
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        });

        var cloud = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark, cloud).AddStyle(BoundaryStyle, boundary));

        // Read a second time, for the extent the zoom buttons target. Nothing draws
        // it, so it is a FeatureSource and not a layer.
        _friscoCityBoundary = new ShapeFileFeatureSource(shapefilePath)
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        _readout = AddToPanel(new Label { FontSize = 12, FontFamily = "Courier" });

        var buttons = new HorizontalStackLayout { Spacing = 6 };
        buttons.Add(Button("Zoom in", () => map.ZoomInAsync()));
        buttons.Add(Button("Zoom out", () => map.ZoomOutAsync()));
        buttons.Add(Button("1:50,000", () => map.ZoomToAsync(50000)));
        AddToPanel(buttons);
        AddToPanel(Button("Zoom to the city boundary", ZoomToBoundaryAsync));

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 180000;
        map.CurrentExtentChanged += (_, _) => ShowReadout();

        ShowReadout();
        _ = map.RefreshAsync();
    }

    private Button Button(string text, Func<Task> action)
    {
        var button = new Button { Text = text, FontSize = 12, Padding = new Thickness(8, 4) };
        button.Clicked += async (_, _) =>
        {
            await action();
            ShowReadout();
        };
        return button;
    }

    private async Task ZoomToBoundaryAsync()
    {
        // A FeatureSource opens synchronously; only the layers above it are async.
        _friscoCityBoundary.Open();
        _map.CurrentExtent = _friscoCityBoundary.GetBoundingBox();
        await _map.RefreshAsync();
    }

    private void ShowReadout()
        => _readout.Text = $"1:{_map.MapScale:N0}   rotation {_map.MapRotation:N0}°";
}
