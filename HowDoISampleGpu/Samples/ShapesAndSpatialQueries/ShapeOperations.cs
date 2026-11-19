using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a shape can be asked to become: buffered, simplified, rotated, scaled,
/// translated, hulled, enveloped, intersected, differenced or unioned.
/// </summary>
public class ShapeOperations : SamplePage
{
    /// <summary>The area the intersection and difference work against, drawn in blue.</summary>
    private const string WestRegionWkt =
        "POLYGON((-10780139 3918539, -10780206 3915600, -10780037 3913978, -10779176 3913336, " +
        "-10778280 3911934, -10778263 3910684, -10778382 3909569, -10778280 3907356, " +
        "-10785595 3904045, -10786034 3904822, -10786001 3908150, -10785933 3909315, " +
        "-10786001 3911275, -10785832 3914485, -10785832 3917728, -10785460 3919012, " +
        "-10782233 3918995, -10780139 3918539))";

    private static readonly GeoColor SourceColor = new(128, GeoColors.LightOrange);
    private static readonly GeoColor ResultColor = new(96, GeoColors.Green);
    private static readonly GeoColor RegionColor = new(64, GeoColors.DodgerBlue);

    private static readonly GeoColor[] PieceColors =
    {
        new(140, GeoColors.Crimson), new(140, GeoColors.Gold), new(140, GeoColors.MediumPurple),
        new(140, GeoColors.LightCoral), new(140, GeoColors.Teal), new(140, GeoColors.Orange),
    };

    private static readonly string[] Operations =
    {
        "Buffer 1000 m", "Simplify 500 m", "Rotate 90°", "Scale 50%",
        "Translate 3 km NE", "Convex hull", "Envelope",
        "Intersection with the blue area", "Difference from the blue area", "Union of the pieces",
    };

    // What the operation reads, what it works against, and what it returned.
    private readonly DynamicSource _sourceArea = new();
    private readonly DynamicSource _regionArea = new();
    private readonly DynamicSource _resultArea = new();

    private readonly PolygonShape _westRegion = new(WestRegionWkt);
    private Collection<Feature> _pieces;
    private MapView _map;
    private Label _status;
    private int _operation;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddDynamic(_sourceArea).AddDynamic(_regionArea).AddDynamic(_resultArea);
        map.Basemap = new GpuBasemap(style);

        // The city limits arrive cut into pieces, which is what gives Union something
        // to do. Read once and kept, then handed to whichever operation is chosen -
        // ten polygons do not need a tile pipeline.
        var source = new ShapeFileFeatureSource(SampleShared.Shapefile("FriscoCityLimitsDivided.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };
        source.Open();
        _pieces = source.GetAllFeatures(ReturningColumnsType.NoColumns);
        var bounds = source.GetBoundingBox();
        source.Close();

        AddChoicePicker(Operations, 0, index => { _operation = index; Apply(); });
        _status = AddStatusLabel();

        bounds.ScaleUp(50);
        map.CurrentExtent = bounds;

        Apply();
        _ = map.RefreshAsync();
    }

    private void Apply()
    {
        var first = _pieces[0];
        var shape = first.GetShape();
        var union = _operation == 9;
        var paired = _operation is 7 or 8;

        BaseShape result = _operation switch
        {
            0 => shape.Buffer(1000, GeographyUnit.Meter, DistanceUnit.Meter),
            1 => AreaBaseShape.Simplify((AreaBaseShape)shape, 500, SimplificationType.DouglasPeucker),
            2 => BaseShape.Rotate(shape, shape.GetCenterPoint(), 90f),
            3 => BaseShape.ScaleTo(shape, 0.5),
            4 => BaseShape.TranslateByOffset(shape, 3000, 3000, GeographyUnit.Meter, DistanceUnit.Meter),
            5 => first.GetConvexHull().GetShape(),
            6 => shape.GetBoundingBox(),
            7 => ((AreaBaseShape)shape).GetIntersection(_westRegion),
            8 => ((AreaBaseShape)shape).GetDifference(_westRegion),
            _ => AreaBaseShape.Union(_pieces),
        };

        // Union is the only one that reads more than one shape, and so the only one
        // drawn with more than the polygon it worked on: its pieces get a colour each,
        // because two shapes look like one blob otherwise and merging them would then
        // look like nothing at all. The blue area appears for the two operations that
        // have a second operand, and not otherwise.
        _sourceArea.Clear();
        if (union)
        {
            for (var i = 0; i < _pieces.Count; i++)
            {
                _sourceArea.SetArea("piece-" + i, _pieces[i], PieceColors[i % PieceColors.Length]);
            }
        }
        else
        {
            _sourceArea.SetArea("source", new Feature(shape), SourceColor);
        }

        _regionArea.Clear();
        if (paired)
        {
            _regionArea.SetArea("region", new Feature(_westRegion), RegionColor);
        }

        _resultArea.Clear();
        if (result != null)
        {
            _resultArea.SetArea("result", new Feature(result), ResultColor, GeoColors.Green, 2f);
        }

        _status.Text = Operations[_operation];
        _ = _map.RefreshAsync();
    }
}
