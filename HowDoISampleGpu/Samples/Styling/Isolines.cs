using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Contours drawn from scattered measurements: mosquito counts around Frisco turned
/// into isolines, with the level count, line width, interpolation power and search
/// radius all live.
/// </summary>
public class Isolines : SamplePage
{
    private const string DataPointsStyle = """
    {
      "layers": [
        { "id": "data-points",
          "type": "circle",
          "source-layer": "points",
          "paint": {
              "circle-radius": 5,
              "circle-color": "#FFFFFF",
              "circle-stroke-color": "#1E2329",
              "circle-stroke-width": 2 }
        }
      ]
    }
    """;

    private readonly Dictionary<PointShape, double> _points;
    private readonly RectangleShape _dataBounds;

    private GpuBasemap _basemap;
    private MapStyle _style;
    private PointValueSource _isolineSource;
    private MapView _map;
    private Label _status;

    private bool _fillBands;
    private bool _showLabels = true;
    private double _searchRadius;   // 0 = every point
    private int _levelCount = 25;
    private float _lineWidth = 3f;
    private double _power = 2.0;

    public Isolines()
    {
        _points = LoadCsv(Path.Combine(SampleShared.DataFolder, "Csv", "Frisco_Mosquitos.csv"));
        var xs = _points.Keys.Select(p => p.X).ToList();
        var ys = _points.Keys.Select(p => p.Y).ToList();
        _dataBounds = new RectangleShape(xs.Min(), ys.Max(), xs.Max(), ys.Min());
    }

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var pointTiles = new FeatureSourceVectorTileSource();
        pointTiles.FeatureSources.Add("points", new InMemoryFeatureSource(
            Array.Empty<FeatureSourceColumn>(),
            _points.Keys.Select(point => new Feature(point)).ToList()));

        _style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        }).AddStyle(DataPointsStyle, pointTiles);

        _basemap = new GpuBasemap(_style);
        map.Basemap = _basemap;

        var start = (RectangleShape)_dataBounds.CloneDeep();
        start.ScaleUp(60);
        map.CurrentExtent = start;

        _status = AddStatusLabel();

        var toggles = new HorizontalStackLayout { Spacing = 10 };
        toggles.Add(Toggle("Bands", _fillBands, value => { _fillBands = value; ApplyIsolines(); }));
        toggles.Add(Toggle("Labels", _showLabels, value => { _showLabels = value; ApplyIsolines(); }));
        AddToPanel(toggles);

        AddSlider("Levels", 5, 60, _levelCount, value => { _levelCount = (int)value; ApplyIsolines(); });
        AddSlider("Line width", 1, 8, _lineWidth, value => { _lineWidth = (float)value; ApplyIsolines(); });
        AddSlider("Power", 0.5, 5, _power, value => { _power = value; ApplyIsolines(); });
        AddSlider("Search radius", 0, 1, 0, value =>
        {
            _searchRadius = value * _dataBounds.Width;
            ApplyIsolines();
        });

        ApplyIsolines();
        _ = map.RefreshAsync();
    }

    private View Toggle(string name, bool value, Action<bool> changed)
    {
        var row = new HorizontalStackLayout { Spacing = 6 };
        var toggle = new Switch { IsToggled = value, Scale = 0.8 };
        toggle.Toggled += (_, e) => changed(e.Value);
        row.Add(new Label { Text = name, FontSize = 12, VerticalOptions = LayoutOptions.Center });
        row.Add(toggle);
        return row;
    }

    private void AddSlider(string name, double min, double max, double value, Action<double> changed)
    {
        AddToPanel(new Label { Text = name, FontSize = 11, TextColor = Colors.Gray });
        var slider = new Slider { Minimum = min, Maximum = max, Value = value };
        slider.DragCompleted += (_, _) => changed(slider.Value);
        AddToPanel(slider);
    }

    private void ApplyIsolines()
    {
        // Breaks over the data, coloured light blue to red.
        var levels = IsoLineLayer.GetIsoLineLevels(_points, _levelCount);
        var colors = GeoColor.GetColorsInQualityFamily(
            GeoColor.FromHtml("#4EA8FF"), GeoColors.Red, levels.Count, ColorWheelDirection.Clockwise);

        var display = new IsolineStyle
        {
            Levels = levels.Select((level, i) => new ColorStop(level, colors[i])).ToList(),
            LineWidthInPixel = _lineWidth,
            FillOpacity = _fillBands ? 0.45f : 0f,
            ShowLabels = _showLabels,
            LabelEveryNthLevel = 2,
            LabelColor = GeoColor.FromHtml("#E7E9EC"),
            LabelHaloColor = GeoColor.FromHtml("#1E2329"),
        };

        // The document did not change, so SetStyleAsync only re-sends the isoline
        // registration; no tile is rebuilt.
        if (_isolineSource != null) _style.RemoveIsolines(_isolineSource);

        _isolineSource = new PointValueSource(_points) { Power = _power, SearchRadius = _searchRadius };
        _style.AddIsolines(_isolineSource, display);
        _ = _basemap.SetStyleAsync(_style);

        _status.Text = string.Format(CultureInfo.InvariantCulture,
            "{0:N0} points, {1} levels", _points.Count, levels.Count);
        _ = _map.RefreshAsync();
    }

    private static Dictionary<PointShape, double> LoadCsv(string path)
    {
        var points = new Dictionary<PointShape, double>();
        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length < 3) continue;

            points[new PointShape(
                double.Parse(parts[0], CultureInfo.InvariantCulture),
                double.Parse(parts[1], CultureInfo.InvariantCulture))] =
                double.Parse(parts[2], CultureInfo.InvariantCulture);
        }

        return points;
    }
}
