using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// Contours drawn from scattered measurements: mosquito counts around Frisco turned
/// into isolines, with the number of levels live.
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

    private bool _fillBands;
    private bool _rebuilding;
    private bool _moved;
    private int _levelCount = 25;

    /// <summary>Zero: every measurement is in range of every cell.</summary>
    private const double SearchRadius = 0;

    /// <summary>Labelled contours, which is most of what makes a contour readable.</summary>
    private const bool ShowLabels = true;

    /// <summary>
    /// Fixed: line width is cosmetic, and the inverse-distance power changes this
    /// data too little to see. The slider is kept for what changes the contours: how
    /// many there are.
    /// </summary>
    private const float LineWidth = 3f;
    private const double Power = 2.0;

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

        // One row: how many contours on the left, and whether they are drawn as lines
        // or as filled bands between them, on the right.
        var row = new Grid { ColumnSpacing = 14 };
        row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var caption = new Label
        {
            Text = FormattableString.Invariant($"Levels {_levelCount:0}"),
            FontSize = 11,
            TextColor = Colors.Gray,
        };

        var levels = new Slider { Minimum = 5, Maximum = 60, Value = _levelCount };
        levels.ValueChanged += (_, e) =>
        {
            caption.Text = FormattableString.Invariant($"Levels {e.NewValue:0}");
            _levelCount = (int)e.NewValue;
            Reapply();
        };

        row.Children.Add(new VerticalStackLayout { Children = { caption, levels } });

        var bands = new ChoiceStrip(new[] { "Lines", "Filled" }, 0, index =>
        {
            _fillBands = index == 1;
            Reapply();
        });

        row.Children.Add(bands);
        Grid.SetColumn((BindableObject)bands, 1);
        AddToPanel(row);

        ApplyIsolines();
        _ = map.RefreshAsync();

    }

    /// <summary>
    /// Rebuilds the isolines for the controls as they now stand, and - if they moved
    /// again while it was working - once more for where they ended up.
    /// <para>
    /// Every move of the slider asks for this. One rebuild is interpolation,
    /// contouring and a restyle, so a drag asks for more of them than a frame can
    /// carry; running them all in turn would leave work queued after the finger has
    /// stopped. This drops the ones in the middle and draws where the finger is.
    /// </para>
    /// </summary>
    private async void Reapply()
    {
        if (_rebuilding)
        {
            _moved = true;
            return;
        }

        _rebuilding = true;

        try
        {
            do
            {
                _moved = false;
                ApplyIsolines();

                // Lets the frame the rebuild produced reach the screen before the next
                // one is started, so a drag animates instead of landing in one jump.
                await Task.Yield();
            }
            while (_moved);
        }
        finally
        {
            _rebuilding = false;
        }
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
            LineWidthInPixel = LineWidth,
            FillOpacity = _fillBands ? 0.45f : 0f,
            ShowLabels = ShowLabels,
            LabelEveryNthLevel = 2,
            LabelColor = GeoColor.FromHtml("#E7E9EC"),
            LabelHaloColor = GeoColor.FromHtml("#1E2329"),
        };

        // The document did not change, so SetStyleAsync only re-sends the isoline
        // registration; no tile is rebuilt.
        if (_isolineSource != null) _style.RemoveIsolines(_isolineSource);

        _isolineSource = new PointValueSource(_points) { Power = Power, SearchRadius = SearchRadius };
        _style.AddIsolines(_isolineSource, display);
        _ = _basemap.SetStyleAsync(_style);

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
