using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The world as a globe, as web mercator, and as Robinson - the same map morphing
/// between the shapes it can be drawn in.
/// </summary>
public class TheGlobe : SamplePage
{
    private const double WorldHalfSize = 20037508.342789244;
    private static readonly TimeSpan MorphDuration = TimeSpan.FromMilliseconds(900);

    // Italy, so zooming in lands on a coast. Open ocean has no detail to show:
    // past zoom 8 the satellite tiles there are a flat, black sea.
    private static readonly PointShape Center = new(1390000, 5160000);

    private static readonly (string Name, string Definition)[] Choices =
    {
        ("Globe", "globe"),
        ("Web Mercator", "3857"),
        ("Robinson", "+proj=robin +datum=WGS84 +units=m +no_defs"),
    };

    private MapView _map;
    private Label _status;
    private string _choice = "globe";

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(Choices.Select(c => c.Name).ToList(), 0, index => _ = MorphAsync(Choices[index].Definition));
        _status = AddStatusLabel();

        try
        {
            var source = new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2);
            map.Basemap = new GpuBasemap(new MapStyle().AddRaster(source, sourceId: "thinkgeo"))
            {
                DisplayProjection = DisplayProjection.Globe,
            };

            map.CurrentExtent = new RectangleShape(
                Center.X - WorldHalfSize, Center.Y + WorldHalfSize,
                Center.X + WorldHalfSize, Center.Y - WorldHalfSize);

            _status.Text = "Globe";
            _ = map.RefreshAsync();
        }
        catch (Exception exception)
        {
            _status.Text = "could not open the tile source: " + exception.Message.Split('\n')[0];
        }
    }

    private async Task MorphAsync(string definition)
    {
        if (definition == _choice) return;
        _choice = definition;

        var target = definition switch
        {
            "globe" => DisplayProjection.Globe,
            "3857" => DisplayProjection.WebMercator,
            _ => DisplayProjection.FromProjString(definition),
        };

        // Zoom-out stops where the world fills the window: a projected canvas is not
        // cyclic, so there is nothing to show past its edge.
        var before = _map.CurrentExtent;
        _map.MaxScale = definition == "3857" ? double.MaxValue : WorldFillScale();
        _map.CurrentExtent = before;
        await _map.RefreshAsync();

        try
        {
            // The camera does not move: the screen centre keeps the same ground point
            // and only the shape under it changes.
            await _map.Basemap.AnimateDisplayProjectionAsync(target, MorphDuration);
            _status.Text = Choices.First(c => c.Definition == definition).Name;
        }
        catch (OperationCanceledException)
        {
            // Another projection was picked before this morph finished.
        }
    }

    private double WorldFillScale()
    {
        var world = new RectangleShape(-WorldHalfSize, WorldHalfSize, WorldHalfSize, -WorldHalfSize);
        return MapUtil.GetScale(world, Math.Max(1, _map.CanvasWidth), GeographyUnit.Meter) * 1.05;
    }
}
