using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a shape can be asked to become: buffered, simplified, rotated, scaled or
/// translated - each of them running, not standing still.
/// <para>
/// Every operation here takes a number, and the number sweeps between its ends by
/// itself and never settles: the buffer grows and shrinks, the shape turns through a
/// full circle, the simplification coarsens and recovers. That is the whole point of
/// the sample. A single frozen result at one arbitrary distance says very little
/// about what an operation does, which is why the ones that take no number - the
/// hull, the envelope, the intersection, the difference, the union - are not here:
/// each of them is one picture, and one picture of a set operation is a screenshot,
/// not a sample.
/// </para>
/// <para>
/// A finger on the slider takes the sweep over for as long as it is there, and the
/// sweep resumes a moment after it is let go, so the sample is never found stopped.
/// </para>
/// </summary>
public class ShapeOperations : SamplePage
{
    private static readonly GeoColor SourceColor = new(128, GeoColors.LightOrange);
    private static readonly GeoColor ResultColor = new(96, GeoColors.Green);

    /// <summary>An operation and the range its number moves over.</summary>
    private sealed record Operation(string Name, Parameter Parameter);

    private sealed record Parameter(double Min, double Max, double Start, Func<double, string> Caption);

    /// <summary>How long one sweep from one end of a parameter to the other takes.</summary>
    private static readonly TimeSpan SweepDuration = TimeSpan.FromSeconds(2.5);

    /// <summary>
    /// How often the swept value moves. Not how often the map is redrawn: a buffer of
    /// this polygon takes about fifty milliseconds, and Reapply coalesces, so what
    /// actually arrives is one rebuild per computation with the newest value.
    /// </summary>
    private static readonly TimeSpan SweepInterval = TimeSpan.FromMilliseconds(50);

    /// <summary>How long after a finger leaves the slider the sweep takes over again.</summary>
    private static readonly TimeSpan ResumeDelay = TimeSpan.FromSeconds(1.5);

    private static readonly Operation[] Operations =
    {
        new("Buffer", new Parameter(100, 3000, 1000,
            value => FormattableString.Invariant($"Distance {value:N0} m"))),
        new("Simplify", new Parameter(10, 2000, 500,
            value => FormattableString.Invariant($"Tolerance {value:N0} m"))),
        new("Rotate", new Parameter(0, 360, 90,
            value => FormattableString.Invariant($"Angle {value:0}°"))),
        new("Scale", new Parameter(10, 200, 50,
            value => FormattableString.Invariant($"Scale {value:0}%"))),
        new("Translate", new Parameter(0, 5000, 3000,
            value => FormattableString.Invariant($"Offset {value:N0} m"))),
    };

    // What the operation reads, and what it returned.
    private readonly DynamicSource _sourceArea = new();
    private readonly DynamicSource _resultArea = new();

    private Collection<Feature> _pieces;
    private MapView _map;
    private Label _caption;
    private Slider _slider;
    private IDispatcherTimer _sweep;
    private IDispatcherTimer _resume;
    private int _sweepDirection = 1;
    private bool _sweeping;
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
        style.AddDynamic(_sourceArea).AddDynamic(_resultArea);
        map.Basemap = new GpuBasemap(style);

        // Read once and kept: one polygon handed to whichever operation is chosen does
        // not need a tile pipeline.
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
            _caption.Text = Operations[_operation].Parameter.Caption(_value);
            Reapply();
        };

        // A finger on the slider holds the sweep, and letting go hands it back. It is
        // never left stopped: a stopped sweep is a sample with no animation in it,
        // which is the one thing this sample is for.
        _slider.DragStarted += (_, _) => StopSweep();
        _slider.DragCompleted += (_, _) => _resume.Start();

        _resume = Dispatcher.CreateTimer();
        _resume.Interval = ResumeDelay;
        _resume.Tick += (_, _) =>
        {
            _resume.Stop();
            _sweep?.Start();
        };

        _sweep = Dispatcher.CreateTimer();
        _sweep.Interval = SweepInterval;
        _sweep.Tick += (_, _) => Step();

        AddToPanel(new VerticalStackLayout { Children = { _caption, _slider } });
        ShowParameter();

        // Room for the widest the sweep can get, not just for the shape as it is.
        // The buffer grows to its maximum every few seconds, and an extent fitted to
        // the polygon alone sends the result off the sides of the screen exactly when
        // there is most to see.
        var reach = Operations[0].Parameter.Max;
        OpenOn(map, new RectangleShape(
            bounds.UpperLeftPoint.X - reach, bounds.UpperLeftPoint.Y + reach,
            bounds.LowerRightPoint.X + reach, bounds.LowerRightPoint.Y - reach));

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

        _slider.Minimum = 0;                  // below every range here, so the next line is legal
        _slider.Maximum = parameter.Max;
        _slider.Minimum = parameter.Min;
        _slider.Value = parameter.Start;
        _value = parameter.Start;
        _caption.Text = parameter.Caption(_value);

        _sweepDirection = 1;
        _sweep?.Start();
    }

    /// <summary>
    /// Moves the parameter one step along and turns it round at either end, so the
    /// operation runs back and forth instead of stopping at an extreme.
    /// </summary>
    private void Step()
    {
        var parameter = Operations[_operation].Parameter;
        var step = (parameter.Max - parameter.Min) * (SweepInterval / SweepDuration);
        var next = _value + (step * _sweepDirection);

        if (next >= parameter.Max)
        {
            next = parameter.Max;
            _sweepDirection = -1;
        }
        else if (next <= parameter.Min)
        {
            next = parameter.Min;
            _sweepDirection = 1;
        }

        // Through the slider, so the control shows where the operation has got to -
        // which is also how the slider's own event does the rest of the work.
        _sweeping = true;
        _slider.Value = next;
        _sweeping = false;
    }

    private void StopSweep()
    {
        _resume?.Stop();
        _sweep?.Stop();
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
            _ => BaseShape.TranslateByOffset(shape, value, value, GeographyUnit.Meter, DistanceUnit.Meter),
        };
    }

    /// <summary>Puts the shape it worked on and the shape it returned on the map.</summary>
    private void Show(int operation, BaseShape result)
    {
        _sourceArea.Clear();
        _sourceArea.SetArea("source", new Feature(_pieces[0].GetShape()), SourceColor);

        _resultArea.Clear();
        if (result != null)
        {
            _resultArea.SetArea("result", new Feature(result), ResultColor, GeoColors.Green, 2f);
        }

        _ = _map.RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopSweep();
    }
}
