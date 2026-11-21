using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Two questions asked of Frisco's zoning polygons: tap the map and either the one
/// the point falls inside or everything within five hundred metres of it lights up,
/// with what the highlighted zoning is named underneath.
/// </summary>
/// <remarks>
/// Two, not four. A tap is a point, and a point intersecting a polygon is a point
/// inside it, so Intersecting had the same answer as Containing every time - and
/// Disjoint is "all the rest", which lights up the whole city and says nothing. What
/// is worth showing is the one you are standing in and the ones you are near.
/// <para>
/// The names come from the same query: asking for the columns is a flag on the call,
/// and once a feature is highlighted its data is already in hand - which is why
/// there is no separate sample for reading a feature's attributes, and why only the
/// one line of it that answers "what did I just tap" is shown rather than a table a
/// phone has no room for.
/// </para>
/// </remarks>
public class FindFeaturesBySpatialRelation : SamplePage
{
    private const string ZoningStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "zoning",
          "paint": {
              "fill-color": "rgba(147,112,219,0.35)",
              "fill-outline-color": "#B39DDB" }
        }
      ]
    }
    """;

    private static readonly string[] Relations =
    {
        // Short enough for a cell a third of a phone wide; what they are both
        // relative to - the point you tapped - is the sample.
        "Containing",
        "Within 500 m",
    };

    // Zoning is thousands of static polygons - the tile pipeline's case. The query
    // shape and the matches are a handful of features that change on every tap,
    // which is the geometry source's case.
    private readonly DynamicSource _query = new();
    private readonly DynamicSource _matches = new();

    private QueryTools _zoningQuery;
    private FeatureSource _zoning;
    private MapView _map;
    private Label _status;
    private int _relation;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var zoning = new ShapeFileFeatureSource(SampleShared.Shapefile("Zoning.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        var tiles = new FeatureSourceVectorTileSource();
        tiles.FeatureSources.Add("zoning", zoning);

        // The tile cutter reads the source it is given on its own threads, so a
        // source that is not safe to read from two threads at once is queried
        // through a copy.
        _zoning = zoning.ConcurrentAccess == ConcurrentAccess.Safe ? zoning : zoning.CloneDeep();
        _zoningQuery = new QueryTools(_zoning);

        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddStyle(ZoningStyle, tiles);

        // A ringed circle is no shape the renderer has of its own, so it is drawn
        // once and registered under the name the point marker asks for.
        style.Images.Add("pin", PointStyle.CreateSimpleCircleStyle(GeoColors.White, 8, GeoColors.Black, 2));
        style.AddDynamic(_matches);
        style.AddDynamic(_query);
        map.Basemap = new GpuBasemap(style);

        AddChoiceStrip(Relations, 0, index => { _relation = index; Run(_seed); });
        _status = AddStatusLabel();
        AddToPanel(new Label { Text = "Tap the map to ask again.", FontSize = 11, TextColor = Colors.Gray });

        map.SingleTap += (_, e) => Run(map.ToWorldCoordinate(e.X, e.Y));

        map.CenterPoint = new PointShape(-10779200, 3912500);
        map.MapScale = 60000;
        _ = map.RefreshAsync();

        Run(map.CenterPoint);
    }

    private PointShape _seed;

    private void Run(PointShape point)
    {
        if (point == null) return;
        _seed = point;

        // With the columns, not without them: the data behind a feature comes back
        // from the query that found it, and costs a flag rather than a second pass.
        _zoning.Open();
        var found = _relation == 0
            ? _zoningQuery.GetFeaturesContaining(point, ReturningColumnsType.AllColumns)
            : _zoningQuery.GetFeaturesWithinDistanceOf(
                point, GeographyUnit.Meter, DistanceUnit.Meter, 500, ReturningColumnsType.AllColumns);
        var verb = _relation == 0 ? "containing" : "within 500 m of";
        var matches = found.ToList();
        _zoning.Close();

        _matches.Clear();
        for (var i = 0; i < matches.Count; i++)
        {
            _matches.SetArea($"match{i}", matches[i],
                new GeoColor(120, GeoColors.Orange), GeoColors.OrangeRed, 2f);
        }

        _query.Clear();
        _query.SetPoint("seed", new Feature(point), sizeInPixel: 16f, iconName: "pin");

        _status.Text = matches.Count == 0
            ? $"no zoning areas {verb} the point"
            : $"{matches.Count} zoning areas {verb} the point - {Describe(matches[0])}";
        _ = _map.RefreshAsync();
    }

    /// <summary>
    /// The one line of a feature's data worth a phone's screen: what the zoning is
    /// called, from whichever column the file happens to carry it in.
    /// </summary>
    private static string Describe(Feature feature)
    {
        foreach (var column in new[] { "ZONE_DESC", "ZONING", "ZONE", "DESCRIPT", "NAME" })
        {
            if (feature.ColumnValues.TryGetValue(column, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return feature.ColumnValues.Count + " columns";
    }
}
