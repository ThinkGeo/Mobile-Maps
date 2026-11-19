using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The ground's height from the ThinkGeo Cloud: under one point, or sampled along a
/// line at a chosen interval.
/// </summary>
public class Elevation : SamplePage
{
    private static readonly LineShape SampleLine = new(
        "LINESTRING(-10776298 3912306,-10776496 3912399,-10776675 3912478,-10776890 3912516,"
        + "-10777189 3912509,-10777329 3912442,-10777664 3912174)");

    private readonly DynamicSource _queryShape = new();
    private readonly DynamicSource _elevationPoints = new();

    private ElevationCloudClient _client;
    private MapView _map;
    private Label _status;
    private double _interval = 50;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.Images.Add("star", new PointStyle(PointSymbolType.Star, 20, GeoBrushes.DodgerBlue));
        style.AddDynamic(_queryShape);
        style.AddDynamic(_elevationPoints);
        map.Basemap = new GpuBasemap(style);

        _client = new ElevationCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };

        _status = AddToPanel(new Label { FontSize = 12, MaxLines = 3 });
        AddToPanel(new Label { Text = "Sample interval along the line", FontSize = 11, TextColor = Colors.Gray });
        var interval = new Slider { Minimum = 20, Maximum = 200, Value = _interval };
        interval.ValueChanged += (_, e) => _interval = e.NewValue;
        AddToPanel(interval);

        var buttons = new HorizontalStackLayout { Spacing = 8 };
        buttons.Add(Button("Along the line", () => ShowLineAsync(SampleLine)));
        AddToPanel(buttons);
        AddToPanel(new Label { Text = "Tap the map for one point.", FontSize = 11, TextColor = Colors.Gray });

        map.SingleTap += (_, e) => _ = ShowPointAsync(map.ToWorldCoordinate(e.X, e.Y));

        map.CenterPoint = new PointShape(-10776981, 3912345);
        map.MapScale = 6200;
        _ = map.RefreshAsync();
        _ = ShowLineAsync(SampleLine);
    }

    private Button Button(string text, Func<Task> action)
    {
        var button = new Button { Text = text, FontSize = 12, Padding = new Thickness(8, 4) };
        button.Clicked += async (_, _) => await action();
        return button;
    }

    private async Task ShowPointAsync(PointShape point)
    {
        Ask(new Feature(point), isPoint: true);
        try
        {
            var feet = await _client.GetElevationOfPointAsync(point.X, point.Y, 3857);
            _elevationPoints.Clear();
            _status.Text = $"Elevation: {feet:0.0} feet";
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private async Task ShowLineAsync(LineShape line)
    {
        Ask(new Feature(line), isPoint: false);
        try
        {
            var result = await _client.GetElevationOfLineAsync(
                line, 3857, _interval, DistanceUnit.Meter, DistanceUnit.Feet);
            Show(result.ElevationPoints);
            _status.Text = FormattableString.Invariant(
                $"{result.ElevationPoints.Count} samples every {_interval:0} m · low {result.LowestElevationPoint.Elevation:0.0} ft · high {result.HighestElevationPoint.Elevation:0.0} ft · average {result.AverageElevation:0.0} ft");
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private void Ask(Feature asked, bool isPoint)
    {
        _queryShape.Clear();
        if (isPoint) _queryShape.SetPoint("asked", asked, sizeInPixel: 20f, iconName: "star");
        else _queryShape.SetLine("asked", asked, GeoColors.DodgerBlue, 2f);
    }

    /// <summary>
    /// One marker per sample, warm where the ground is higher. The whole set is
    /// rewritten on every answer, which is the geometry source's case.
    /// </summary>
    private void Show(Collection<CloudElevationPointResult> results)
    {
        _elevationPoints.Clear();
        if (results.Count == 0) return;

        var low = results.Min(r => r.Elevation);
        var high = results.Max(r => r.Elevation);
        var span = Math.Max(1, high - low);

        for (var i = 0; i < results.Count; i++)
        {
            var strength = (results[i].Elevation - low) / span;
            var color = GeoColor.FromArgb(235, (byte)(60 + (195 * strength)), (byte)(200 - (150 * strength)), 60);
            _elevationPoints.SetPoint("sample-" + i, new Feature(results[i].Point), color: color, sizeInPixel: 9f);
        }
    }
}
