using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Two centuries of American state populations as circles that grow and change
/// colour - geometry rewritten on every frame, with no tile cut.
/// </summary>
public class DynamicRendering : SamplePage
{
    private const string StatesStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "states",
          "paint": { "fill-color": "rgba(46,50,56,1)" } },
        { "type": "line",
          "source-layer": "states",
          "paint": {
              "line-color": "rgba(120,126,134,1)",
              "line-width": 1 }
        }
      ]
    }
    """;

    private readonly DynamicSource _circles = new();
    private List<(double X, double Y, string Abbreviation)> _stateCentres;
    private Dictionary<(string State, int Year), int> _population;
    private MapView _map;
    private Label _yearLabel;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var shapefilePath = SampleShared.Shapefile("USStates_3857.shp");
        _stateCentres = ReadStateCentres(shapefilePath);
        _population = ReadPopulation(Path.Combine(SampleShared.DataFolder, "historical_state_population_by_year.csv"));

        var states = new FeatureSourceVectorTileSource();
        states.FeatureSources.Add("states", new ShapeFileFeatureSource(shapefilePath));

        // One white disc image is every circle; the marker's colour tints it.
        var style = new MapStyle()
            .SetBackground(GeoColor.FromHtml("#1F2226"))
            .AddStyle(StatesStyle, states)
            .AddDynamic(_circles);
        style.Images.Add("disc", new PointStyle(PointSymbolType.Circle, 62, GeoBrushes.White));
        map.Basemap = new GpuBasemap(style);

        map.CenterPoint = new PointShape(-10650000, 4770000);
        map.MapScale = 37000000;

        _yearLabel = AddToPanel(new Label { FontSize = 16, FontAttributes = FontAttributes.Bold });
        var years = _population.Keys.Select(key => key.Year).ToList();
        var slider = new Slider { Minimum = years.Min(), Maximum = years.Max(), Value = years.Min() };
        slider.ValueChanged += (_, e) => ShowYear(e.NewValue);
        AddToPanel(slider);

        ShowYear(slider.Value);
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// Every circle rewritten for one moment in time, fractional years interpolated
    /// between the two censuses around them. A Clear and a fresh set of calls land as
    /// one rebuild, on screen the next frame, with nothing re-cut.
    /// </summary>
    private void ShowYear(double moment)
    {
        var year = (int)Math.Floor(moment);
        var fraction = moment - year;

        foreach (var (x, y, abbreviation) in _stateCentres)
        {
            if (!_population.TryGetValue((abbreviation, year), out var before))
            {
                _circles.Remove(abbreviation);
                continue;
            }

            var after = _population.TryGetValue((abbreviation, year + 1), out var next) ? next : before;
            var population = before + ((after - before) * fraction);

            var strength = (float)Math.Min(population / 300000d, 150d);
            var color = GeoColor.FromArgb(150, (byte)(255 * (1 - (strength / 150))), (byte)(255 * strength / 150), 0);
            _circles.SetPoint(abbreviation, new Feature(x, y), 0f, color, Math.Max(4f, strength), "disc");
        }

        _yearLabel.Text = year.ToString();
        _ = _map.RefreshAsync();
    }

    private static List<(double, double, string)> ReadStateCentres(string shapefilePath)
    {
        var source = new ShapeFileFeatureSource(shapefilePath);
        source.Open();
        try
        {
            var centres = new List<(double, double, string)>();
            foreach (var feature in source.GetAllFeatures(ReturningColumnsType.AllColumns))
            {
                var centre = feature.GetShape().GetCenterPoint();
                centres.Add((centre.X, centre.Y, feature.ColumnValues["STATE_ABBR"]));
            }

            return centres;
        }
        finally
        {
            source.Close();
        }
    }

    private static Dictionary<(string, int), int> ReadPopulation(string csvPath)
    {
        var population = new Dictionary<(string, int), int>();
        foreach (var line in File.ReadAllLines(csvPath))
        {
            var fields = line.Split(',');
            population[(fields[0], int.Parse(fields[1]))] = int.Parse(fields[2]);
        }

        return population;
    }
}
