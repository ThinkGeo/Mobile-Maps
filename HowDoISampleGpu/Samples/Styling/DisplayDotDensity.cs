using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A dot map: one dot for every twenty-five thousand housing units, scattered inside
/// the state that owns them - owner-occupied in blue, renter-occupied in orange.
/// </summary>
public class DisplayDotDensity : SamplePage
{
    /// <summary>Housing units one dot stands for.</summary>
    private const int UnitsPerDot = 25_000;

    private const string StyleJson = """
    {
      "layers": [

        { "type": "background",
          "paint": { "background-color": "#1F2226" } },

        { "type": "fill",
          "source-layer": "USStates_3857",
          "paint": {
              "fill-color": "#FCFCFA",
              "fill-outline-color": "#787878" }
        },

        { "type": "circle",
          "source-layer": "owner",
          "paint": {
              "circle-radius": 1.9,
              "circle-color": "#1F78B4" }
        },

        { "type": "circle",
          "source-layer": "renter",
          "paint": {
              "circle-radius": 1.9,
              "circle-color": "#F07814" }
        }
      ]
    }
    """;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // Frame the lower 48; Alaska would stretch the box to the Aleutians.
        map.CurrentExtent = new RectangleShape(-13_900_000, 6_400_000, -7_400_000, 2_800_000);

        var status = AddStatusLabel();
        status.Text = "Scattering the dots…";

        _ = BuildAsync(map, status);
    }

    /// <summary>
    /// The scatter is rejection sampling - a point in the state's bounding box, kept
    /// only if the state contains it - so it is thousands of point-in-polygon tests,
    /// and it runs off the UI thread. Doing it inline leaves the page blank and frozen
    /// while it works: fast enough not to notice on an iPhone, half a minute of empty
    /// screen on an Android emulator.
    /// </summary>
    private async Task BuildAsync(MapView map, Label status)
    {
        var tiles = await Task.Run(() =>
        {
            // USStates_3857.shp is already in the map's projection - no converter needed.
            var states = new ShapeFileFeatureSource(SampleShared.Shapefile("USStates_3857.shp"));
            states.Open();
            var features = states.GetAllFeatures(new[] { "OWNER_OCC", "RENTER_OCC" });
            states.Close();

            // Every restyle passes this same source: a document on its own has no data
            // to draw.
            var source = new FeatureSourceVectorTileSource();
            source.FeatureSources.Add(states);
            source.FeatureSources.Add("owner", Scatter(features, "OWNER_OCC", seed: 1));
            source.FeatureSources.Add("renter", Scatter(features, "RENTER_OCC", seed: 2));
            return source;
        });

        map.Basemap = new GpuBasemap(new MapStyle(StyleJson, tiles));
        status.Text = FormattableString.Invariant(
            $"One dot = {UnitsPerDot:N0} housing units · blue owner-occupied · orange renter-occupied");

        await map.RefreshAsync();
    }

    /// <summary>
    /// Dots thrown at a state's bounding box and kept only where they land inside it.
    /// The seed is per state, so the same dots come back on every run.
    /// </summary>
    private static FeatureSource Scatter(IEnumerable<Feature> states, string column, int seed)
    {
        var dots = new Collection<Feature>();
        var stateIndex = 0;

        foreach (var state in states)
        {
            stateIndex++;
            if (state.GetShape() is not AreaBaseShape area
                || !state.ColumnValues.TryGetValue(column, out var raw)
                || !double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var units))
            {
                continue;
            }

            var wanted = (int)Math.Round(units / UnitsPerDot);
            if (wanted <= 0) continue;

            var box = area.GetBoundingBox();
            var random = new Random((seed * 7919) + stateIndex);
            var placed = 0;

            // The attempt ceiling keeps a thin or ragged state from spinning forever.
            var attempts = 0;
            var attemptCeiling = wanted * 60;

            while (placed < wanted && attempts < attemptCeiling)
            {
                attempts++;
                var candidate = new PointShape(
                    box.LowerLeftPoint.X + (random.NextDouble() * box.Width),
                    box.LowerLeftPoint.Y + (random.NextDouble() * box.Height));

                if (!area.Contains(candidate)) continue;

                dots.Add(new Feature(candidate.GetWellKnownBinary(),
                    FormattableString.Invariant($"{seed}-{stateIndex}-{placed}")));
                placed++;
            }
        }

        return new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(), dots);
    }
}
