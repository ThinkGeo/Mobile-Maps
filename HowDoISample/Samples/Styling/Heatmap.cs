using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// Coyote sightings as a heatmap, with the ramp, radius and intensity changed at
/// run time - a paint change, not a restyle, so no tile is rebuilt.
/// </summary>
public class Heatmap : SamplePage
{
    private const string HeatLayerId = "coyote-heat";

    private const string HeatStyle = """
    {
      "layers": [
        { "id": "coyote-heat",
          "type": "heatmap",
          "source-layer": "Frisco_Coyote_Sightings",
          "paint": {
              "heatmap-radius": 22,
              "heatmap-intensity": 0.55,
              "heatmap-opacity": 0.85 }
        },
        { "type": "circle",
          "source-layer": "Frisco_Coyote_Sightings",
          "minzoom": 13,
          "paint": {
              "circle-radius": 4,
              "circle-color": "#E0915A" }
        }
      ]
    }
    """;

    /// <summary>
    /// heatmap-color as density stops, 0 (no data) to 1 (densest). Stop 0 is
    /// transparent in every ramp - it leaves the basemap visible where nothing was
    /// seen.
    /// </summary>
    private static readonly (string Name, ColorStop[] Stops)[] ColorRamps =
    {
        ("MapLibre", new[]
        {
            new ColorStop(0.0, GeoColor.FromArgb(0, 0, 0, 255)),
            new ColorStop(0.1, GeoColor.FromArgb(255, 65, 105, 225)),
            new ColorStop(0.3, GeoColor.FromArgb(255, 0, 255, 255)),
            new ColorStop(0.5, GeoColor.FromArgb(255, 0, 255, 0)),
            new ColorStop(0.7, GeoColor.FromArgb(255, 255, 255, 0)),
            new ColorStop(1.0, GeoColor.FromArgb(255, 255, 0, 0)),
        }),
        ("Warm", new[]
        {
            new ColorStop(0.0, GeoColor.FromArgb(0, 255, 255, 178)),
            new ColorStop(0.15, GeoColor.FromArgb(180, 255, 255, 178)),
            new ColorStop(0.4, GeoColor.FromArgb(255, 254, 204, 92)),
            new ColorStop(0.7, GeoColor.FromArgb(255, 253, 141, 60)),
            new ColorStop(1.0, GeoColor.FromArgb(255, 189, 0, 38)),
        }),
        ("Viridis", new[]
        {
            new ColorStop(0.0, GeoColor.FromArgb(0, 68, 1, 84)),
            new ColorStop(0.25, GeoColor.FromArgb(255, 59, 82, 139)),
            new ColorStop(0.5, GeoColor.FromArgb(255, 33, 145, 140)),
            new ColorStop(0.75, GeoColor.FromArgb(255, 94, 201, 98)),
            new ColorStop(1.0, GeoColor.FromArgb(255, 253, 231, 37)),
        }),
        ("Blue to red", new[]
        {
            new ColorStop(0.0, GeoColor.FromArgb(0, 33, 102, 172)),
            new ColorStop(0.2, GeoColor.FromArgb(255, 103, 169, 207)),
            new ColorStop(0.4, GeoColor.FromArgb(255, 209, 229, 240)),
            new ColorStop(0.6, GeoColor.FromArgb(255, 253, 219, 199)),
            new ColorStop(0.8, GeoColor.FromArgb(255, 239, 138, 98)),
            new ColorStop(1.0, GeoColor.FromArgb(255, 178, 24, 43)),
        }),
    };

    private readonly FeatureSourceVectorTileSource _coyote = new();
    private MapStyle _style;
    private MapView _map;

    private (string Name, ColorStop[] Stops) _ramp = ColorRamps[0];
    private float _radius = 22f;
    private float _intensity = 0.55f;

    /// <summary>
    /// Fixed: opacity fades the whole layer, which every layer can do. Radius and
    /// intensity are the heatmap's own numbers - how far a point spreads and how much
    /// it counts - so they get the sliders.
    /// </summary>
    private const float HeatOpacity = 0.85f;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(-10778000, 3912500);
        map.MapScale = 68000;

        _coyote.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile("Frisco_Coyote_Sightings.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        });

        var cloud = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        _style = new MapStyle(ThinkGeoVectorStyles.Dark, cloud).AddStyle(HeatStyle, _coyote);

        AddChoiceStrip(ColorRamps.Select(r => r.Name).ToList(), 0, index =>
        {
            _ramp = ColorRamps[index];
            ApplyPaint();
        });
        // Both numbers on one row.
        AddSliderRow(
            new SliderSpec("Radius", 5, 60, _radius,
                value => { _radius = (float)value; ApplyPaint(); },
                value => FormattableString.Invariant($"Radius {value:0} px")),
            new SliderSpec("Intensity", 0.05, 2.0, _intensity,
                value => { _intensity = (float)value; ApplyPaint(); },
                value => FormattableString.Invariant($"Intensity {value:0.00}")));

        ApplyPaint();
        map.Basemap = new GpuBasemap(_style);
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// A paint change, not a restyle: the heatmap layer keeps its tiles and only the
    /// numbers the renderer reads change.
    /// </summary>
    private void ApplyPaint()
    {
        _style?.SetHeatmapPaint(HeatLayerId, radius: _radius, intensity: _intensity,
            opacity: HeatOpacity, colorStops: _ramp.Stops);
        _ = _map?.RefreshAsync();
    }
}
