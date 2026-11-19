using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// All twenty-five topology rules, each with data made to both pass and fail it:
/// what the rule checks in green, what it checks against in blue, and what breaks
/// it in red.
/// </summary>
public class ValidateTopology : SamplePage
{
    private sealed record Rule(
        string Name,
        string[] Subject,
        string[] Against,
        Func<Collection<Feature>, Collection<Feature>, Collection<Feature>> Check,
        string Note);

    // A rule needs data that both passes and fails it to show anything - a set of
    // polygons that never overlap says nothing about the overlap rule - so every
    // rule brings its own shapes.
    private static readonly Rule[] Rules =
    {
        new("Points must touch lines",
            new[] { "POINT(0 0)", "POINT(50 0)", "POINT(150 0)" },
            new[] { "LINESTRING(0 0,100 0)" },
            (s, a) => TopologyValidator.PointsMustTouchLines(s, a).InvalidFeatures,
            "Points that miss the line are red."),

        new("Points must touch line endpoints",
            new[] { "POINT(0 0)", "POINT(50 0)", "POINT(100 0)" },
            new[] { "LINESTRING(0 0,100 0,100 100,0 100)" },
            (s, a) => TopologyValidator.PointsMustTouchLineEndpoints(s, a).InvalidFeatures,
            "A point on the line but not at an end is still wrong."),

        new("Points must touch polygon boundaries",
            new[] { "POINT(150 0)", "POINT(50 50)", "POINT(0 0)" },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PointsMustTouchPolygonBoundaries(s, a).InvalidFeatures,
            "Inside is not on the boundary; nor is outside."),

        new("Points must be within polygons",
            new[] { "POINT(150 0)", "POINT(0 0)", "POINT(50 50)" },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PointsMustBeWithinPolygons(s, a).InvalidFeatures,
            "A point on the corner is not within."),

        new("Line endpoints must touch points",
            new[] { "LINESTRING(0 0,100 0,100 50)" },
            new[] { "POINT(0 0)" },
            (s, a) => TopologyValidator.LineEndPointsMustTouchPoints(s, a).InvalidFeatures,
            "One end has its point, the other does not."),

        new("Lines must overlap polygon boundaries",
            new[] { "LINESTRING(-50 0,150 0)" },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.LinesMustOverlapPolygonBoundaries(s, a).InvalidFeatures,
            "Only the stretch that runs along the boundary counts."),

        new("Lines must be single part",
            new[]
            {
                "MULTILINESTRING((0 -50,100 -50,100 -100,0 -100))",
                "MULTILINESTRING((0 0,100 0),(100 50,0 50))",
            },
            null,
            (s, _) => TopologyValidator.LinesMustBeSinglePart(s).InvalidFeatures,
            "One multiline is really one strand; the other is two."),

        new("Lines must form a closed polygon",
            new[] { "LINESTRING(0 0,100 0,100 100,20 100)", "LINESTRING(0 0,-50 0,-50 100,0 100)" },
            null,
            (s, _) => TopologyValidator.LinesMustFormClosedPolygon(s).InvalidFeatures,
            "The ends that do not meet are red."),

        new("Lines must not have pseudonodes",
            new[]
            {
                "LINESTRING(0 0,50 0,50 50,0 0)",
                "LINESTRING(-50 0,-50 50)",
                "LINESTRING(-100 0,-50 50)",
                "LINESTRING(-50 -50,-50 -100)",
                "LINESTRING(-100 -50,-50 -100)",
                "LINESTRING(-50 -100,0 -100)",
            },
            null,
            (s, _) => TopologyValidator.LinesMustNotHavePseudonodes(s).InvalidFeatures,
            "A node joining exactly two lines is a pseudonode."),

        new("Lines must not intersect",
            new[]
            {
                "LINESTRING(0 0,100 0,100 100)",
                "LINESTRING(0 -50,30 0,60 0,100 50)",
                "LINESTRING(20 50,20 -50)",
            },
            null,
            (s, _) => TopologyValidator.LinesMustNotIntersect(s).InvalidFeatures,
            "Each crossing comes back as a point."),

        new("Lines must not overlap",
            new[]
            {
                "LINESTRING(0 0,100 0,100 100)",
                "LINESTRING(0 -50,30 0,60 0,100 50)",
                "LINESTRING(20 50,20 -50)",
            },
            null,
            (s, _) => TopologyValidator.LinesMustNotOverlap(s).InvalidFeatures,
            "Only the shared stretches, not the crossings."),

        new("Lines must not self-intersect",
            new[] { "LINESTRING(0 0,100 0,100 100,50 100,50 -50)" },
            null,
            (s, _) => TopologyValidator.LinesMustNotSelfIntersect(s).InvalidFeatures,
            "One line, crossing itself once."),

        new("Lines must not self-overlap",
            new[] { "LINESTRING(0 0,100 0,100 100,0 100,20 0,40 0,40 -50)" },
            null,
            (s, _) => TopologyValidator.LinesMustNotSelfOverlap(s).InvalidFeatures,
            "One line, doubling back along itself."),

        new("Lines must not overlap lines",
            new[] { "LINESTRING(150 0,100 30,100 60,150 100)" },
            new[] { "LINESTRING(0 0,100 0,100 100,0 100)" },
            (s, a) => TopologyValidator.LinesMustNotOverlapLines(a, s).InvalidFeatures,
            "The stretch lying on the blue line is red."),

        new("Lines must be covered by lines",
            new[] { "LINESTRING(0 0,100 0,100 100,0 100)" },
            new[] { "LINESTRING(0 -50,50 0,100 0,100 150)" },
            (s, a) => TopologyValidator.LinesMustBeCoveredByLines(a, s).InvalidFeatures,
            "The parts with nothing under them are red."),

        new("Lines must not self-intersect or touch",
            new[]
            {
                "LINESTRING(0 0,100 0,100 100)",
                "LINESTRING(0 -50,30 0,60 0,100 50)",
                "LINESTRING(20 50,20 -50)",
            },
            null,
            (s, _) => TopologyValidator.LinesMustNotSelfIntersectOrTouch(s).InvalidFeatures,
            "Crossings and shared stretches both count."),

        new("Polygon boundaries must overlap boundaries",
            new[] { "POLYGON((0 0,50 0,50 50,0 50,0 0))" },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PolygonBoundariesMustOverlapPolygonBoundaries(a, s).InvalidFeatures,
            "Two corners are shared; the rest of the boundary is not."),

        new("Polygon boundaries must overlap lines",
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            new[] { "LINESTRING(-50 0,100 0,100 150)" },
            (s, a) => TopologyValidator.PolygonBoundariesMustOverlapLines(s, a).InvalidFeatures,
            "The line covers two sides; the other two are red."),

        new("Polygons must overlap polygons",
            new[]
            {
                "POLYGON((25 25,50 25,50 50,25 50,25 25))",
                "POLYGON((75 25,125 25,125 75,75 75,75 25))",
                "POLYGON((150 25,200 25,200 75,150 75,150 25))",
            },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PolygonsMustOverlapPolygons(a, s).InvalidFeatures,
            "The one that never meets the blue square is red."),

        new("Polygons must be within polygons",
            new[]
            {
                "POLYGON((25 25,50 25,50 50,25 50,25 25))",
                "POLYGON((75 25,125 25,125 75,75 75,75 25))",
                "POLYGON((150 25,200 25,200 75,150 75,150 25))",
            },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PolygonsMustBeWithinPolygons(a, s).InvalidFeatures,
            "Overlapping is not enough - it has to be all the way inside."),

        new("Polygons must contain a point",
            new[]
            {
                "POLYGON((150 0,250 0,250 100,150 100,150 0))",
                "POLYGON((0 0,100 0,100 100,0 100,0 0))",
            },
            new[] { "POINT(50 50)" },
            (s, a) => TopologyValidator.PolygonsMustContainPoint(s, a).InvalidFeatures,
            "The empty polygon is red."),

        new("Polygons must overlap each other",
            new[]
            {
                "POLYGON((0 0,100 0,100 100,0 100,0 0))",
                "POLYGON((-50 -50,50 -50,50 50,-50 50,-50 -50))",
            },
            null,
            (s, _) => TopologyValidator.PolygonsMustOverlapEachOther(
                Wkt("POLYGON((0 0,100 0,100 100,0 100,0 0))"),
                Wkt("POLYGON((-50 -50,50 -50,50 50,-50 50,-50 -50))")).InvalidFeatures,
            "This one reports from both sets: everything outside the shared corner."),

        new("Polygons must not have gaps",
            new[]
            {
                "POLYGON((0 0,40 0,40 40,0 40,0 0))",
                "POLYGON((30 30,70 30,70 70,30 70,30 30))",
                "POLYGON((60 0,100 0,100 40,60 40,60 0))",
                "POLYGON((30 10,70 10,70 -30,30 -30,30 10))",
            },
            null,
            (s, _) => TopologyValidator.PolygonsMustNotHaveGaps(s).InvalidFeatures,
            "The hole left between them comes back as its own shape."),

        new("Polygons must not overlap",
            new[]
            {
                "POLYGON((25 25,50 25,50 50,25 50,25 25))",
                "POLYGON((75 25,125 25,125 75,75 75,75 25))",
                "POLYGON((150 25,200 25,200 75,150 75,150 25))",
                "POLYGON((0 0,100 0,100 100,0 100,0 0))",
            },
            null,
            (s, _) => TopologyValidator.PolygonsMustNotOverlap(s).InvalidFeatures,
            "Red is the overlap itself, not the polygons that made it."),

        new("Polygons must not overlap polygons",
            new[]
            {
                "POLYGON((25 25,50 25,50 50,25 50,25 25))",
                "POLYGON((75 25,125 25,125 75,75 75,75 25))",
                "POLYGON((150 25,200 25,200 75,150 75,150 25))",
            },
            new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
            (s, a) => TopologyValidator.PolygonsMustNotOverlapPolygons(a, s).InvalidFeatures,
            "The reverse of the rule above: overlapping is the fault."),
    };

    // Three sets of shapes that change with every rule, so they are geometry rather
    // than an overlay's raster tiles, which a zoom between two levels would stretch.
    private readonly DynamicSource _against = new();
    private readonly DynamicSource _subject = new();
    private readonly DynamicSource _broken = new();

    private MapView _map;
    private Label _result;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle("""
            { "layers": [ { "type": "background", "paint": { "background-color": "#1F2226" } } ] }
            """)
            .AddDynamic(_against).AddDynamic(_subject).AddDynamic(_broken));

        AddChoicePicker(Rules.Select(r => r.Name).ToList(), 0, Run);
        _result = AddToPanel(new Label { FontSize = 12, MaxLines = 3 });

        Run(0);
        _ = map.RefreshAsync();
    }

    private void Run(int index)
    {
        var rule = Rules[index];
        var subject = Wkt(rule.Subject);
        var against = rule.Against == null ? new Collection<Feature>() : Wkt(rule.Against);
        var broken = rule.Check(subject, against);

        Paint(_against, against, GeoColors.DodgerBlue);
        Paint(_subject, subject, GeoColors.LimeGreen);
        Paint(_broken, broken, GeoColors.Red);

        _result.Text = broken.Count == 0
            ? "Nothing breaks this rule. " + rule.Note
            : broken.Count + " in red. " + rule.Note;

        // Every rule uses its own shapes, at its own size, so the view follows them.
        var bounds = Bounds(against, subject, broken);
        bounds.ScaleUp(50);
        _map.CurrentExtent = bounds;
        _ = _map.RefreshAsync();
    }

    private static Collection<Feature> Wkt(params string[] wellKnownText)
    {
        var features = new Collection<Feature>();
        foreach (var text in wellKnownText) features.Add(new Feature(text));
        return features;
    }

    private static void Paint(DynamicSource into, Collection<Feature> features, GeoColor color)
    {
        into.Clear();
        for (var i = 0; i < features.Count; i++)
        {
            var id = "feature-" + i;
            switch (features[i].GetShape())
            {
                case AreaBaseShape:
                    into.SetArea(id, features[i], new GeoColor(50, color), color, 2f);
                    break;
                case LineBaseShape:
                    into.SetLine(id, features[i], color, 3f);
                    break;
                default:
                    into.SetPoint(id, features[i], color: color, sizeInPixel: 12f);
                    break;
            }
        }
    }

    /// <summary>Around everything the rule put on screen.</summary>
    private static RectangleShape Bounds(params Collection<Feature>[] sets)
    {
        RectangleShape bounds = null;
        foreach (var feature in sets.Where(set => set != null).SelectMany(set => set))
        {
            var box = feature.GetBoundingBox();
            if (bounds == null) bounds = box;
            else bounds.ExpandToInclude(box);
        }

        return bounds ?? new RectangleShape(-100, 100, 100, -100);
    }
}
