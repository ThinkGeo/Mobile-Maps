using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The spatial relations, asked of Frisco's zoning polygons: tap the map and the
/// ones containing, intersecting, disjoint from or within a distance of that point
/// light up.
/// </summary>
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
        // Short enough for a cell a third of a phone wide; what they are all
        // relative to - the point you tapped - is the sample.
        "Containing",
        "Intersecting",
        "Within 500 m",
        "Disjoint",
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

        _zoning.Open();
        var (found, verb) = _relation switch
        {
            0 => (_zoningQuery.GetFeaturesContaining(point, ReturningColumnsType.NoColumns), "containing"),
            1 => (_zoningQuery.GetFeaturesIntersecting(point, ReturningColumnsType.NoColumns), "intersecting"),
            2 => (_zoningQuery.GetFeaturesWithinDistanceOf(
                    point, GeographyUnit.Meter, DistanceUnit.Meter, 500, ReturningColumnsType.NoColumns),
                  "within 500 m of"),
            _ => (_zoningQuery.GetFeaturesDisjointed(point, ReturningColumnsType.NoColumns), "disjoint from"),
        };
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

        _status.Text = $"{matches.Count} zoning areas {verb} the point";
        _ = _map.RefreshAsync();
    }
}
