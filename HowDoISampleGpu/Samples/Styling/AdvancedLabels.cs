using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Seven lessons in labelling, each a style document you can edit on the spot: text
/// along a curve, the nine anchors, priority when they will not all fit, wrapping and
/// transforming, text-optional, CJK down a north-running line, and several columns in
/// one label.
/// </summary>
public class AdvancedLabels : SamplePage
{
    private sealed record Lesson(string Title, string Kind, (double X, double Y, double Scale) Frame, string Json);

    private static readonly Lesson[] Lessons =
    {
        new("Curved text along a line", "curve", (-10786000, 3916000, 45000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "line",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "curve"],
      "paint": { "line-color": "#A1887F", "line-width": 5 } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "curve"],
      "layout": {
          "symbol-placement": "line",
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Bold"],
          "text-size": 15 },
      "paint": {
          "text-color": "#D7CCC8",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.4 }
    }
  ]
}
"""),
        new("Anchors", "anchor", (-10770000, 3916000, 22000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "circle",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "anchor"],
      "paint": { "circle-radius": 4, "circle-color": "#1E88E5" } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "anchor"],
      "layout": {
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Regular"],
          "text-anchor": ["get", "ANCHOR"],
          "text-offset": [0, 0],
          "text-size": 12 },
      "paint": {
          "text-color": "#CFD8DC",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.2 }
    }
  ]
}
"""),
        new("Priority, when they will not all fit", "rank", (-10786000, 3904000, 40000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "circle",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "rank"],
      "paint": { "circle-radius": 3, "circle-color": "#90A4AE" } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "rank"],
      "layout": {
          "symbol-sort-key": ["to-number", ["get", "RANK"]],
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Bold"],
          "text-size": 15 },
      "paint": {
          "text-color": ["step", ["to-number", ["get", "RANK"]],
              "#EF5350",
              4, "#CFD8DC"],
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.4 }
    }
  ]
}
"""),
        new("Wrapping and transforming", "wrap", (-10770000, 3904000, 30000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "circle",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "wrap"],
      "paint": { "circle-radius": 4, "circle-color": "#43A047" } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "wrap"],
      "layout": {
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Regular"],
          "text-max-width": 8,
          "text-transform": "uppercase",
          "text-anchor": "top",
          "text-offset": [0, 0.5],
          "text-size": 12 },
      "paint": {
          "text-color": "#A5D6A7",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.2 }
    }
  ]
}
"""),
        new("text-optional: keep the icon, drop the name", "poi", (-10786000, 3892000, 20000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "poi"],
      "layout": {
          "icon-image": "dot",
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Regular"],
          "text-optional": true,
          "text-anchor": "top",
          "text-offset": [0, 0.7],
          "text-size": 13 },
      "paint": {
          "text-color": "#B39DDB",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.2 }
    }
  ]
}
"""),
        new("CJK down a north-running line", "cjk", (-10770000, 3892000, 45000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "line",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "cjk"],
      "paint": { "line-color": "#80CBC4", "line-width": 5 } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "cjk"],
      "layout": {
          "symbol-placement": "line",
          "text-field": ["get", "NAME"],
          "text-font": ["Noto Sans Regular"],
          "text-size": 18 },
      "paint": {
          "text-color": "#B2DFDB",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.4 }
    }
  ]
}
"""),
        new("Several columns in one label", "town", (-10786000, 3880000, 30000), """
{
  "layers": [
    { "type": "background", "paint": { "background-color": "#1F2226" } },

    { "type": "circle",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "town"],
      "paint": { "circle-radius": 4, "circle-color": "#BCAAA4" } },

    { "type": "symbol",
      "source-layer": "lessons",
      "filter": ["==", ["get", "KIND"], "town"],
      "layout": {
          "text-field": ["concat",
              ["get", "NAME"], "\n",
              "Population: ", ["get", "POPULATION"]],
          "text-font": ["Noto Sans Bold"],
          "text-anchor": "top",
          "text-offset": [0, 0.6],
          "text-size": 12 },
      "paint": {
          "text-color": "#D7CCC8",
          "text-halo-color": "#1A1A1A",
          "text-halo-width": 1.2 }
    }
  ]
}
"""),
    };

    // Every restyle passes this same source: a document on its own has no data to draw.
    private FeatureSourceVectorTileSource _tiles;
    private IDispatcherTimer _applyTimer;
    private Editor _editor;
    private MapView _map;
    private Label _status;
    private Lesson _lesson = Lessons[0];

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        _tiles = new FeatureSourceVectorTileSource();
        _tiles.FeatureSources.Add("lessons", BuildLessons());

        AddChoicePicker(Lessons.Select(lesson => lesson.Title).ToList(), 0, index =>
        {
            _lesson = Lessons[index];
            _editor.Text = _lesson.Json;
            FrameLesson();
        });

        // The desktop gallery puts the document in a tall editor down the right-hand
        // edge. Here it is a short one on the card - enough to reach in and change a
        // size, a colour or an anchor and watch the map follow.
        _editor = new Editor
        {
            Text = Lessons[0].Json,
            FontFamily = "Courier",
            FontSize = 11,
            HeightRequest = 150,
            AutoSize = EditorAutoSizeOption.Disabled,
        };
        _editor.TextChanged += (_, _) =>
        {
            _applyTimer.Stop();
            _applyTimer.Start();
        };
        AddToPanel(_editor);

        _status = AddStatusLabel();

        _applyTimer = Dispatcher.CreateTimer();
        _applyTimer.Interval = TimeSpan.FromMilliseconds(400);
        _applyTimer.Tick += async (_, _) =>
        {
            _applyTimer.Stop();
            await ApplyEditorStyleAsync();
        };

        map.Basemap = new GpuBasemap(Compose(_editor.Text, _tiles));
        FrameLesson();
    }

    // A document that does not compile changes nothing; the map keeps the last one
    // that worked.
    private async Task ApplyEditorStyleAsync()
    {
        try
        {
            await _map.Basemap.SetStyleAsync(Compose(_editor.Text, _tiles));
            _status.TextColor = Colors.SeaGreen;
            _status.Text = FormattableString.Invariant($"applied at {DateTime.Now:HH:mm:ss}");
        }
        catch (Exception exception)
        {
            _status.TextColor = Colors.IndianRed;
            _status.Text = exception.Message.Split('\n')[0];
        }
    }

    private static MapStyle Compose(string json, FeatureSourceVectorTileSource tiles)
    {
        var style = new MapStyle(json, tiles);
        style.Images.Add("dot", PointStyle.CreateSimpleCircleStyle(GeoColor.FromHtml("#5E35B1"), 10, GeoColors.White, 2));
        return style;
    }

    /// <summary>Each lesson has its own patch of ground, framed to show it whole.</summary>
    private void FrameLesson()
    {
        _map.CenterPoint = new PointShape(_lesson.Frame.X, _lesson.Frame.Y);
        _map.MapScale = _lesson.Frame.Scale;
        _ = _map.RefreshAsync();
    }

    /// <summary>
    /// The lessons' data, crafted rather than loaded: winding lines for the text to
    /// bend along, a grid of points whose positions match the anchors they name, a
    /// crowd for priority to thin, long names to wrap, close-set places for
    /// text-optional, a north-running line with a Chinese name, and towns with a
    /// population column.
    /// </summary>
    private static InMemoryFeatureSource BuildLessons()
    {
        var features = new List<Feature>();

        void Add(BaseShape shape, string kind, string name, string rank = "", string anchor = "", string population = "")
        {
            var feature = new Feature(shape);
            feature.ColumnValues["KIND"] = kind;
            feature.ColumnValues["NAME"] = name;
            feature.ColumnValues["RANK"] = rank;
            feature.ColumnValues["ANCHOR"] = anchor;
            feature.ColumnValues["POPULATION"] = population;
            features.Add(feature);
        }

        // Curved text: two parkways winding east-west.
        Add(Winding(-10786000, 3917500, 1500, 0), "curve", "Meandering Creek Parkway");
        Add(Winding(-10786000, 3914000, 1800, 1.2), "curve", "Old Mill Heritage Trail");

        // Anchors: each point sits in the grid where its own anchor points.
        var anchors = new[]
        {
            new[] { "top-left", "top", "top-right" },
            new[] { "left", "center", "right" },
            new[] { "bottom-left", "bottom", "bottom-right" },
        };
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                var anchor = anchors[row][col];
                Add(new PointShape(-10770000 + ((col - 1) * 1400), 3916000 - ((row - 1) * 1100)),
                    "anchor", anchor, anchor: anchor);
            }
        }

        // Priority: a spiral of ranked labels, far too many for the space.
        for (var i = 1; i <= 12; i++)
        {
            var angle = i * 2.399963;
            var radius = 350 + (i * 230);
            Add(new PointShape(-10786000 + (radius * Math.Cos(angle)), 3904000 + (radius * Math.Sin(angle))),
                "rank", "Rank " + i, rank: i.ToString(CultureInfo.InvariantCulture));
        }

        // Wrapping: names longer than anyone wants on one line, and one that is not.
        Add(new PointShape(-10772000, 3904800), "wrap", "Frisco Heritage Botanical Conservatory and Community Arboretum");
        Add(new PointShape(-10768000, 3903600), "wrap", "Panther Creek Environmental Education Center");
        Add(new PointShape(-10770000, 3901800), "wrap", "City Hall");

        // text-optional: places set close enough that not every name can fit.
        var pois = new[]
        {
            "Coffee Roastery", "Corner Bakery", "Book Cellar", "Night Market", "Tea House",
            "Vinyl Shop", "Print Studio", "Flower Cart", "Cheese Shop",
        };
        for (var i = 0; i < pois.Length; i++)
        {
            var angle = i * 2.399963;
            var radius = 200 + (i * 160);
            Add(new PointShape(-10786000 + (radius * Math.Cos(angle)), 3892000 + (radius * Math.Sin(angle))),
                "poi", pois[i]);
        }

        // CJK vertical: the line runs north, so the name writes downward.
        Add(WindingNorth(-10770000, 3892000), "cjk", "\u94f6\u674f\u6cb3\u6ee8\u7eff\u9053");

        // Several columns: each town's label is its name and its population.
        Add(new PointShape(-10788000, 3881200), "town", "Cedar Hill", population: "48200");
        Add(new PointShape(-10783800, 3881600), "town", "Maple Grove", population: "12750");
        Add(new PointShape(-10788400, 3878400), "town", "Stone Ridge", population: "3900");
        Add(new PointShape(-10784000, 3878800), "town", "Willow Bend", population: "27600");

        var columns = new[]
        {
            new FeatureSourceColumn("NAME"), new FeatureSourceColumn("KIND"),
            new FeatureSourceColumn("RANK"), new FeatureSourceColumn("ANCHOR"),
            new FeatureSourceColumn("POPULATION"),
        };

        return new InMemoryFeatureSource(columns, features);
    }

    private static LineShape Winding(double centerX, double centerY, double amplitude, double phase)
    {
        var vertices = new Collection<Vertex>();
        for (var i = 0; i <= 48; i++)
        {
            var t = i / 48.0;
            vertices.Add(new Vertex(
                centerX - 4200 + (t * 8400),
                centerY + (amplitude * Math.Sin(phase + (t * Math.PI * 3)))));
        }

        return new LineShape(vertices);
    }

    private static LineShape WindingNorth(double centerX, double centerY)
    {
        var vertices = new Collection<Vertex>();
        for (var i = 0; i <= 48; i++)
        {
            var t = i / 48.0;
            vertices.Add(new Vertex(
                centerX + (1200 * Math.Sin(t * Math.PI * 2.5)),
                centerY - 4200 + (t * 8400)));
        }

        return new LineShape(vertices);
    }
}
