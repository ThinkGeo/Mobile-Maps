using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a shape can be asked to become: buffered, simplified, rotated, scaled,
/// translated, hulled, enveloped, intersected, differenced or unioned.
/// <para>
/// The five that take a number take it from a slider, and the result is rebuilt on
/// every move - the buffer grows under the finger rather than appearing at one
/// fixed distance, which is what the operation actually looks like. The five that
/// take no number hide the slider rather than show a dead one.
/// </para>
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

    /// <summary>
    /// The operations, and for the five that take a number, the range that number
    /// moves over. A null <c>Parameter</c> is an operation with nothing to set.
    /// </summary>
    private sealed record Operation(string Name, Parameter Parameter = null);

    private sealed record Parameter(double Min, double Max, double Start, Func<double, string> Caption);

    private static readonly Operation[] Operations =
    {
        new("Buffer", new Parameter(100, 5000, 1000,
            value => FormattableString.Invariant($"Distance {value:N0} m"))),
        new("Simplify", new Parameter(10, 2000, 500,
            value => FormattableString.Invariant($"Tolerance {value:N0} m"))),
        new("Rotate", new Parameter(0, 360, 90,
            value => FormattableString.Invariant($"Angle {value:0}°"))),
        new("Scale", new Parameter(10, 200, 50,
            value => FormattableString.Invariant($"Scale {value:0}%"))),
        new("Translate", new Parameter(0, 10000, 3000,
            value => FormattableString.Invariant($"Offset {value:N0} m"))),
        new("Convex hull"),
        new("Envelope"),
        new("Intersection"),
        new("Difference"),
        new("Union"),
    };

    // What the operation reads, what it works against, and what it returned.
    private readonly DynamicSource _sourceArea = new();
    private readonly DynamicSource _regionArea = new();
    private readonly DynamicSource _resultArea = new();

    private readonly PolygonShape _westRegion = new(WestRegionWkt);
    private Collection<Feature> _pieces;
    private MapView _map;
    private Label _caption;
    private Slider _slider;
    private int _operation;
    private double _value = Operations[0].Parameter.Start;
    private bool _rebuilding;
    private bool _moved;

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

        AddChoiceStrip(Operations.Select(operation => operation.Name).ToList(), 0, index =>
        {
            _operation = index;
            ShowParameter();
            Reapply();
        });

        _caption = new Label { FontSize = 11, TextColor = Colors.Gray };
        _slider = new Slider();
        _slider.ValueChanged += (_, e) =>
        {
            _value = e.NewValue;
            _caption.Text = Operations[_operation].Parameter?.Caption(_value);
            Reapply();
        };

        AddToPanel(new VerticalStackLayout { Children = { _caption, _slider } });
        ShowParameter();

        bounds.ScaleUp(50);
        map.CurrentExtent = bounds;

        Reapply();
        _ = map.RefreshAsync();

    }

    /// <summary>
    /// Points the one slider at the chosen operation's number, or takes it away when
    /// there is none. Setting the range before the value matters: a Slider clamps what
    /// it is given to the range it currently has, so a value set first can be quietly
    /// pulled back to the old range's edge.
    /// </summary>
    private void ShowParameter()
    {
        var parameter = Operations[_operation].Parameter;

        _caption.IsVisible = parameter != null;
        _slider.IsVisible = parameter != null;
        if (parameter == null) return;

        _slider.Minimum = 0;                  // below every range here, so the next line is legal
        _slider.Maximum = parameter.Max;
        _slider.Minimum = parameter.Min;
        _slider.Value = parameter.Start;
        _value = parameter.Start;
        _caption.Text = parameter.Caption(_value);
    }

    /// <summary>
    /// Runs the chosen operation for the slider where it now is, and - if it moved
    /// again while that was happening - once more for where it ended up.
    /// <para>
    /// The operation itself runs off the UI thread. Buffering this polygon takes about
    /// fifty milliseconds, which on the thread the slider lives on is three frames the
    /// slider cannot answer the finger in: the shape kept up and the control it was
    /// dragged from did not. Everything that touches the map is back on the UI thread
    /// by the time it is drawn.
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

                // Read once: by the time the result comes back the slider may be
                // somewhere else, and the drawing has to match the shape that was asked
                // for, not the one that is about to be.
                var operation = _operation;
                var value = _value;

                var result = await Task.Run(() => Compute(operation, value));
                Show(operation, result);
            }
            while (_moved);
        }
        finally
        {
            _rebuilding = false;
        }
    }

    /// <summary>The operation itself - the line this sample is about.</summary>
    private BaseShape Compute(int operation, double value)
    {
        var first = _pieces[0];
        var shape = first.GetShape();

        return operation switch
        {
            0 => shape.Buffer(value, GeographyUnit.Meter, DistanceUnit.Meter),
            1 => AreaBaseShape.Simplify((AreaBaseShape)shape, value, SimplificationType.DouglasPeucker),
            2 => BaseShape.Rotate(shape, shape.GetCenterPoint(), (float)value),
            3 => BaseShape.ScaleTo(shape, value / 100),
            4 => BaseShape.TranslateByOffset(shape, value, value, GeographyUnit.Meter, DistanceUnit.Meter),
            5 => first.GetConvexHull().GetShape(),
            6 => shape.GetBoundingBox(),
            7 => ((AreaBaseShape)shape).GetIntersection(_westRegion),
            8 => ((AreaBaseShape)shape).GetDifference(_westRegion),
            _ => AreaBaseShape.Union(_pieces),
        };
    }

    /// <summary>Puts the operand, the second operand and the result on the map.</summary>
    private void Show(int operation, BaseShape result)
    {
        var shape = _pieces[0].GetShape();
        var union = operation == 9;
        var paired = operation is 7 or 8;

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

        _ = _map.RefreshAsync();
    }
}
