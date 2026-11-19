using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Layers whose paint and filter are code rather than a document: a population
/// density ramp over the American states, and the world's capitals lit as day or
/// night by a clock.
/// </summary>
public class RenderBasedOnCode : SamplePage, IDisposable
{
    private const string CapitalsLayer = "WorldCapitals";
    private const string StatesLayer = "states";
    private const string RampFillId = "states-fill";
    private const string RampOutlineId = "states-outline";

    private readonly List<(double Offset, string DayId, string NightId, bool? IsDay)> _zones = new();

    private IDispatcherTimer _clock;
    private MapStyle _style;
    private MapView _map;
    private Label _hourLabel;
    private Slider _hourSlider;
    private bool _ramp = true;
    private bool _settingSlider;
    private double _utcHour = DateTime.UtcNow.Hour + (DateTime.UtcNow.Minute / 60.0);

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var capitals = new FeatureSourceVectorTileSource();
        capitals.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile("WorldCapitals.shp"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        });

        var states = new FeatureSourceVectorTileSource();
        states.FeatureSources.Add(StatesLayer, new ShapeFileFeatureSource(SampleShared.Shapefile("USStates_3857.shp")));

        // A paint is baked into a tile when the tile is built, so a paint that read
        // the clock would freeze at the hour each tile was cut. Each zone gets a fixed
        // day layer and a fixed night layer instead, and the clock only switches which
        // one is visible.
        var layers = new List<StyleLayer>();
        foreach (var offset in ReadTimeZoneOffsets())
        {
            var zoneOffset = offset;
            var dayId = FormattableString.Invariant($"utc{zoneOffset:+0.0;-0.0}-day");
            var nightId = FormattableString.Invariant($"utc{zoneOffset:+0.0;-0.0}-night");
            _zones.Add((zoneOffset, dayId, nightId, null));

            layers.Add(StyleLayer.CreateCircle(
                dayId, CapitalsLayer,
                paint: (_, _) => new CirclePaint(GeoColor.FromArgb(255, 255, 215, 0), GeoColors.Black, 8f, 2f, 1f),
                filter: (feature, _) => TryReadTimezone(feature, out var value)
                    && Math.Abs(value - zoneOffset) < 0.01));

            layers.Add(StyleLayer.CreateCircle(
                nightId, CapitalsLayer,
                paint: (_, _) => new CirclePaint(GeoColor.FromArgb(255, 70, 80, 95), GeoColors.Black, 8f, 2f, 1f),
                filter: (feature, _) => TryReadTimezone(feature, out var value)
                    && Math.Abs(value - zoneOffset) < 0.01));
        }

        _style = new MapStyle(ThinkGeoVectorStyles.Light, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });

        _style.AddVectorTiles(states, "usstates", StyleLayerSlot.AboveFills,
            StyleLayer.CreateFill(RampFillId, StatesLayer,
                paint: (feature, _) => new FillPaint(DensityColor(feature), opacity: 0.60f)),
            StyleLayer.CreateLine(RampOutlineId, StatesLayer,
                paint: (_, zoom) => new LinePaint(GeoColors.White, zoom < 5d ? 0.6f : 1.4f, opacity: 0.9f)));

        _style.AddVectorTiles(capitals, "features", layers.ToArray());

        AddChoicePicker(new[] { "Population density ramp", "Capitals by day and night" }, 0, index =>
        {
            _ramp = index == 0;
            ShowMode();
            FrameMode();
        });

        _hourLabel = AddStatusLabel();
        _hourSlider = new Slider { Minimum = 0, Maximum = 24, Value = _utcHour, IsVisible = false };
        _hourSlider.ValueChanged += (_, e) =>
        {
            if (_settingSlider) return;
            _utcHour = e.NewValue;
            ShowHour();
            ApplyHour();
        };
        AddToPanel(_hourSlider);

        _clock = Dispatcher.CreateTimer();
        _clock.Interval = TimeSpan.FromMilliseconds(50);
        _clock.Tick += (_, _) =>
        {
            _utcHour = (_utcHour + 0.05) % 24;
            _settingSlider = true;
            _hourSlider.Value = _utcHour;
            _settingSlider = false;
            ShowHour();
            ApplyHour();
        };

        map.Basemap = new GpuBasemap(_style);
        ShowMode();
        FrameMode();
    }

    /// <summary>The lower 48 for the ramp, the world for the clock.</summary>
    private void FrameMode()
    {
        if (_ramp)
        {
            _map.CenterPoint = new PointShape(-10800000, 4700000);
            _map.MapScale = 30000000;
        }
        else
        {
            _map.CenterPoint = new PointShape(450060, 1074670);
            _map.MapScale = 147914800;
        }

        _ = _map.RefreshAsync();
    }

    /// <summary>Shows one group of code layers and hides the other; the clock only
    /// runs while its circles are on screen.</summary>
    private void ShowMode()
    {
        _hourSlider.IsVisible = !_ramp;
        _style.SetLayerVisibility(RampFillId, _ramp);
        _style.SetLayerVisibility(RampOutlineId, _ramp);

        if (_ramp)
        {
            _clock.Stop();
            for (var i = 0; i < _zones.Count; i++)
            {
                var zone = _zones[i];
                _style.SetLayerVisibility(zone.DayId, false);
                _style.SetLayerVisibility(zone.NightId, false);
                _zones[i] = (zone.Offset, zone.DayId, zone.NightId, null);
            }

            _hourLabel.Text = "Blue ramp: people per square mile, five steps, painted in code";
        }
        else
        {
            ShowHour();
            ApplyHour();
            _clock.Start();
        }
    }

    private void ApplyHour()
    {
        if (_style == null || _ramp) return;

        for (var i = 0; i < _zones.Count; i++)
        {
            var zone = _zones[i];
            var localHour = (((_utcHour + zone.Offset) % 24) + 24) % 24;
            var isDay = localHour >= 7 && localHour <= 19;
            if (zone.IsDay == isDay) continue;

            _style.SetLayerVisibility(zone.DayId, isDay);
            _style.SetLayerVisibility(zone.NightId, !isDay);
            _zones[i] = (zone.Offset, zone.DayId, zone.NightId, isDay);
        }

        _ = _map.RefreshAsync();
    }

    /// <summary>Five-step blue ramp over people per square mile.</summary>
    private static GeoColor DensityColor(Feature feature)
    {
        var density = 0d;
        if (feature.ColumnValues.TryGetValue("POP90_SQMI", out var raw))
        {
            double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out density);
        }

        if (density < 10d) return GeoColor.FromHtml("#EFF3FF");
        if (density < 50d) return GeoColor.FromHtml("#BDD7E7");
        if (density < 150d) return GeoColor.FromHtml("#6BAED6");
        if (density < 500d) return GeoColor.FromHtml("#3182BD");
        return GeoColor.FromHtml("#08519C");
    }

    /// <summary>The offsets the data actually uses - half-hour zones included.</summary>
    private static IEnumerable<double> ReadTimeZoneOffsets()
    {
        var source = new ShapeFileFeatureSource(SampleShared.Shapefile("WorldCapitals.shp"));
        source.Open();
        var features = source.GetAllFeatures(new[] { "TIMEZONE" });
        source.Close();

        return features
            .Select(feature => TryReadTimezone(feature, out var value) ? value : double.NaN)
            .Where(value => !double.IsNaN(value))
            .Distinct()
            .OrderBy(value => value)
            .ToList();
    }

    private static bool TryReadTimezone(Feature feature, out double value)
    {
        value = 0;
        return feature?.ColumnValues != null
            && feature.ColumnValues.TryGetValue("TIMEZONE", out var raw)
            && double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private void ShowHour() => _hourLabel.Text = FormattableString.Invariant(
        $"{(int)_utcHour:00}:{(int)((_utcHour - (int)_utcHour) * 60):00} UTC · gold is daylight");

    public void Dispose()
    {
        _clock?.Stop();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
