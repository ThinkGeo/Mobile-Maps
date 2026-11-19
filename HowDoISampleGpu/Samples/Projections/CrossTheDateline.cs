using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using SkiaSharp;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A flight from Los Angeles to Shanghai, across the 180th meridian, on a globe that
/// has no seam and on a Web Mercator map that does. The route and its corridor are
/// stored once, with longitudes running past -180, and nothing is ever split by hand:
/// the tile cut folds them back into the world, one part each side.
/// </summary>
public class CrossTheDateline : SamplePage, IDisposable
{
    private const double HalfWorld = 20037508.342789244;
    private const double EarthRadius = 6378137.0;
    private const double CorridorHalfWidthMeters = 100 * 1852.0;
    private const int RouteSamples = 360;
    private const double IntroTurnDegrees = 34.0;

    private static readonly TimeSpan MorphDuration = TimeSpan.FromMilliseconds(900);
    private static readonly TimeSpan FlightDuration = TimeSpan.FromSeconds(45);
    private static readonly TimeSpan IntroHold = TimeSpan.FromMilliseconds(900);
    private static readonly TimeSpan IntroTurn = TimeSpan.FromSeconds(7);

    private static readonly Airport Origin = new("LAX", "Los Angeles", -118.408, 33.942);
    private static readonly Airport Destination = new("PVG", "Shanghai Pudong", 121.805, 31.143);

    private const string StyleJson = """
    {
      "layers": [
        { "type": "line",
          "source-layer": "corridor",
          "paint": {
              "line-color": "#6f9bff",
              "line-width": 1.2,
              "line-opacity": 0.8,
              "line-dasharray": [3, 3] }
        },

        { "type": "line",
          "source-layer": "route",
          "layout": { "line-cap": "round", "line-join": "round" },
          "paint": {
              "line-color": "#ffffff",
              "line-width": 5,
              "line-opacity": 0.9 }
        },

        { "type": "line",
          "source-layer": "route",
          "layout": { "line-cap": "round", "line-join": "round" },
          "paint": {
              "line-color": "#1f4fd6",
              "line-width": 2.5 }
        }
      ]
    }
    """;

    // The route and the corridor's edges, in Web Mercator metres with the western half
    // past the world's west edge (x < -20037508). The tile cut folds it back into the
    // world, one part each side of the seam; the sample never splits anything itself.
    private readonly FeatureSourceVectorTileSource _flight = new();
    private readonly DynamicSource _airports = new();
    private readonly DynamicSource _aircraft = new();
    private readonly Stopwatch _flightClock = new();

    // The route, sampled evenly along the arc. Longitudes are continuous from the
    // origin westward: -118 at Los Angeles, -180 at the seam, -238 (= 122 E) at
    // Shanghai. Nothing here is ever wrapped by hand.
    private readonly (double Lon, double Lat)[] _route;
    private readonly double _routeLengthMeters;

    private IDispatcherTimer _clock;
    private CancellationTokenSource _intro;
    private MapView _map;
    private Button _flyButton;
    private Label _positionText;
    private Label _progressText;
    private Label _seamText;
    private Picker _projectionPicker;

    private double _flownMeters;
    private double _flightStartMeters;
    private double _hudShownAtSeconds;
    private bool _flying;
    private bool _globe = true;
    private bool _introSwitching;

    public CrossTheDateline()
    {
        _route = GreatCircle(Origin, Destination, RouteSamples, out _routeLengthMeters);
    }

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        _flight.FeatureSources.Add("route", new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(),
            new Collection<Feature> { new(RouteLine()) }));
        _flight.FeatureSources.Add("corridor", new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(),
            new Collection<Feature> { new(CorridorEdges()) }));

        ShowAirports();
        ShowAircraft(0);

        // The two airports and the aircraft are drawn over everything, and move without
        // a tile being cut.
        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddStyle(StyleJson, _flight);
        style.Images.Add("airport", RenderDotImage());
        style.Images.Add("plane", RenderPlaneImage());
        style.AddDynamic(_airports);
        style.AddDynamic(_aircraft);

        map.Basemap = new GpuBasemap(style);
        map.Basemap.DisplayProjection = DisplayProjection.Globe;

        _projectionPicker = AddChoicePicker(new[] { "Globe", "Web Mercator" }, 0, index => _ = SetProjectionAsync(index == 0));

        var buttons = new HorizontalStackLayout { Spacing = 10 };
        _flyButton = new Button { Text = "Fly", FontSize = 12, Padding = new Thickness(10, 4) };
        _flyButton.Clicked += (_, _) =>
        {
            CancelIntro();
            if (_flying)
            {
                StopTicking();
                _flying = false;
                _flyButton.Text = "Fly";
                return;
            }

            StartFlight();
        };
        var reset = new Button { Text = "Reset", FontSize = 12, Padding = new Thickness(10, 4) };
        reset.Clicked += (_, _) => Reset();
        buttons.Add(_flyButton);
        buttons.Add(reset);
        AddToPanel(buttons);

        _positionText = AddStatusLabel();
        _progressText = AddStatusLabel();
        _seamText = AddStatusLabel();

        _clock = Dispatcher.CreateTimer();

        // WPF advances the flight on CompositionTarget.Rendering, the frame the
        // compositor is about to present. MAUI has no such hook, so this one ticks on a
        // dispatcher timer at about sixty a second. The flight is a function of the
        // stopwatch rather than of the tick count, so an uneven tick changes how smooth
        // it looks, never where the aircraft is.
        _clock.Interval = TimeSpan.FromMilliseconds(16);
        _clock.Tick += (_, _) => Advance();

        ShowFlight();
        FrameRoute();
        ShowSeam();

        // Anything the user does is more interesting than the opening, so it ends the
        // moment a finger arrives.
        map.TouchDown += (_, _) => CancelIntro();

        _ = _map.RefreshAsync();
        _ = PlayIntroAsync();
    }

    // ------------------------------------------------------------------- intro ----

    /// <summary>
    /// The opening move, made of the two animations this sample already has: the globe
    /// turning (a camera move in pure longitude, so the sphere keeps its size) and the
    /// switch to Web Mercator, which is exactly what the projection picker does. Runs
    /// once on load.
    /// </summary>
    private async Task PlayIntroAsync()
    {
        _intro = new CancellationTokenSource();
        var cancel = _intro.Token;

        try
        {
            // The turn ENDS on the route's own framing, so what the flattening leaves on
            // screen is the view this sample opens with - the seam down the middle, both
            // airports in frame. It therefore starts that far east of it.
            var middle = _route[_route.Length / 2];
            var (cx, cy) = Meters(middle.Lon, middle.Lat);
            var destination = new PointShape(cx, cy);
            var turn = IntroTurnDegrees / 180.0 * HalfWorld;

            var width = _map.CurrentExtent.Width;
            var height = _map.CurrentExtent.Height;
            _map.CurrentExtent = new RectangleShape(
                cx + turn - (width / 2), cy + (height / 2), cx + turn + (width / 2), cy - (height / 2));
            await _map.RefreshAsync();
            await Task.Delay(IntroHold, cancel);

            // No easing: a planet turns at one speed.
            await _map.ZoomToExtentAsync(destination, _map.MapScale, 0,
                new AnimationSettings
                {
                    Duration = (uint)IntroTurn.TotalMilliseconds,
                    Easing = Easing.Linear,
                    Type = MapAnimationType.DrawWithAnimation,
                },
                cancellationToken: cancel);

            if (cancel.IsCancellationRequested) return;

            _introSwitching = true;
            _projectionPicker.SelectedIndex = 1;   // the same path the picker takes
            _introSwitching = false;

            // Once the map is flat the flight takes off by itself.
            await Task.Delay(MorphDuration + IntroHold, cancel);
            StartFlight();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _introSwitching = false;
        }
    }

    private void CancelIntro()
    {
        if (_introSwitching) return;
        _intro?.Cancel();
    }

    // -------------------------------------------------------------- projection ----

    private async Task SetProjectionAsync(bool globe)
    {
        if (globe == _globe) return;

        CancelIntro();
        _globe = globe;

        // The camera does not move: the screen centre keeps the same ground point and
        // only the shape under it changes. Zoom-out on the globe stops where the world
        // fills the window; mercator keeps its freedom.
        var before = _map.CurrentExtent;
        _map.MaxScale = _globe ? WorldFillScale() : double.MaxValue;
        _map.CurrentExtent = before;
        await _map.RefreshAsync();

        try
        {
            await _map.Basemap.AnimateDisplayProjectionAsync(
                _globe ? DisplayProjection.Globe : DisplayProjection.WebMercator, MorphDuration);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        ShowSeam();
    }

    private double WorldFillScale()
    {
        var world = new RectangleShape(-HalfWorld, HalfWorld, HalfWorld, -HalfWorld);
        return MapUtil.GetScale(world, Math.Max(1, _map.CanvasWidth), GeographyUnit.Meter) * 1.05;
    }

    /// <summary>The whole Pacific, centred on the route's midpoint.</summary>
    private void FrameRoute()
    {
        var middle = _route[_route.Length / 2];
        var (cx, cy) = Meters(middle.Lon, middle.Lat);
        const double widthInMeters = 22000000;
        var height = widthInMeters * _map.CanvasHeight / Math.Max(1, _map.CanvasWidth);

        _map.MaxScale = _globe ? WorldFillScale() : double.MaxValue;
        _map.CurrentExtent = new RectangleShape(
            cx - (widthInMeters * 0.5), cy + (height * 0.5), cx + (widthInMeters * 0.5), cy - (height * 0.5));
    }

    // ------------------------------------------------------------------ flight ----

    private void StartFlight()
    {
        if (_flownMeters >= _routeLengthMeters) _flownMeters = 0;

        _flightStartMeters = _flownMeters;
        _flightClock.Restart();
        _hudShownAtSeconds = 0;
        _flying = true;
        _flyButton.Text = "Pause";
        StartTicking();
    }

    private void StartTicking()
    {
        if (!_clock.IsRunning) _clock.Start();
    }

    private void StopTicking()
    {
        if (_clock.IsRunning) _clock.Stop();
    }

    private void Reset()
    {
        CancelIntro();
        StopTicking();
        _flightClock.Reset();
        _flying = false;
        _flyButton.Text = "Fly";
        _flownMeters = 0;
        ShowAircraft(0);
        ShowFlight();
        FrameRoute();
        _ = _map.RefreshAsync();
    }

    private void Advance()
    {
        var seconds = _flightClock.Elapsed.TotalSeconds;
        _flownMeters = Math.Min(_routeLengthMeters,
            _flightStartMeters + (seconds / FlightDuration.TotalSeconds * _routeLengthMeters));
        ShowAircraft(_flownMeters);

        // The readout has to keep up with the eye, not with the frame.
        if (seconds - _hudShownAtSeconds >= 0.1)
        {
            _hudShownAtSeconds = seconds;
            ShowFlight();
        }

        if (_flownMeters >= _routeLengthMeters)
        {
            StopTicking();
            _flying = false;
            _flyButton.Text = "Fly";
            ShowFlight();
        }
    }

    /// <summary>Puts the aircraft at a distance along the route, nose along the arc.</summary>
    private void ShowAircraft(double meters)
    {
        var (lon, lat, heading) = PoseAt(meters);
        var (x, y) = Meters(lon, lat);
        _aircraft.SetPoint("plane", new Feature(x, y), (float)heading, GeoColors.White, 44f, "plane");
    }

    private void ShowAirports()
    {
        _airports.Clear();
        foreach (var airport in new[] { Origin, Destination })
        {
            var (x, y) = Meters(airport.Lon, airport.Lat);

            // The marker and its label share one feature under one id; the label reads
            // its text from the feature's column.
            var feature = new Feature(new PointShape(x, y),
                new Dictionary<string, string> { { "NAME", airport.Code + "  " + airport.Name } });
            _airports.SetPoint(airport.Code, feature, 0f, GeoColors.White, 14f, "airport");
            _airports.SetLabel(airport.Code, feature, "NAME", GeoColor.FromHtml("#101317"), 13f, 0f, -14f);
        }
    }

    /// <summary>Position and compass heading at a distance along the arc.</summary>
    private (double Lon, double Lat, double Heading) PoseAt(double meters)
    {
        var fraction = _routeLengthMeters <= 0 ? 0 : Math.Max(0, Math.Min(1, meters / _routeLengthMeters));
        var position = fraction * (_route.Length - 1);
        var index = Math.Min(_route.Length - 2, (int)Math.Floor(position));
        var t = position - index;
        var a = _route[index];
        var b = _route[index + 1];
        return (a.Lon + ((b.Lon - a.Lon) * t), a.Lat + ((b.Lat - a.Lat) * t), Bearing(a, b));
    }

    // --------------------------------------------------------------------- HUD ----

    private void ShowFlight()
    {
        var (lon, lat, heading) = PoseAt(_flownMeters);
        var canonical = ((((lon + 180) % 360) + 360) % 360) - 180;

        _positionText.Text = string.Create(CultureInfo.InvariantCulture,
            $"lon {lon,8:F2}  ({Math.Abs(canonical):F2} {(canonical < 0 ? "W" : "E")})   lat {lat,7:F2}   hdg {heading,3:F0}");
        _progressText.Text = string.Create(CultureInfo.InvariantCulture,
            $"{_flownMeters / 1000:N0} of {_routeLengthMeters / 1000:N0} km  ({_flownMeters / Math.Max(1, _routeLengthMeters):P0})");
    }

    private void ShowSeam() => _seamText.Text = _globe
        ? "Globe: a sphere has no seam. The route is one line and the corridor two edges, stored with longitudes running past -180."
        : "Web Mercator: the seam is the 180th meridian. The same data, longitudes past -180 and all, is folded back into the world at the tile cut - one part each side.";

    // ---------------------------------------------------------------- geometry ----

    /// <summary>The route as one line with continuous longitudes, in metres.</summary>
    private LineShape RouteLine()
    {
        var line = new LineShape();
        foreach (var (lon, lat) in _route)
        {
            var (x, y) = Meters(lon, lat);
            line.Vertices.Add(new Vertex(x, y));
        }

        return line;
    }

    /// <summary>
    /// The two edges of a band 100 nautical miles either side of the route. The offsets
    /// are geodesic and the longitudes stay continuous, like the route's.
    /// </summary>
    private MultilineShape CorridorEdges()
    {
        var left = new LineShape();
        var right = new LineShape();

        for (var i = 0; i < _route.Length; i++)
        {
            var here = _route[i];
            var heading = i < _route.Length - 1 ? Bearing(here, _route[i + 1]) : Bearing(_route[i - 1], here);

            var leftPoint = DestinationFrom(here, heading - 90, CorridorHalfWidthMeters);
            var (lx, ly) = Meters(leftPoint.Lon, leftPoint.Lat);
            left.Vertices.Add(new Vertex(lx, ly));

            var rightPoint = DestinationFrom(here, heading + 90, CorridorHalfWidthMeters);
            var (rx, ry) = Meters(rightPoint.Lon, rightPoint.Lat);
            right.Vertices.Add(new Vertex(rx, ry));
        }

        return new MultilineShape(new[] { left, right });
    }

    /// <summary>
    /// Points along the great circle from a to b, evenly spaced in arc length, with
    /// longitudes made continuous from a onward (no jump at the antimeridian).
    /// </summary>
    private static (double Lon, double Lat)[] GreatCircle(Airport a, Airport b, int count, out double lengthMeters)
    {
        var p = ToUnit(a.Lon, a.Lat);
        var q = ToUnit(b.Lon, b.Lat);
        var dot = Math.Max(-1, Math.Min(1, (p.X * q.X) + (p.Y * q.Y) + (p.Z * q.Z)));
        var omega = Math.Acos(dot);
        var sinOmega = Math.Sin(omega);
        lengthMeters = omega * EarthRadius;

        var result = new (double Lon, double Lat)[count + 1];
        var previousLon = a.Lon;

        for (var i = 0; i <= count; i++)
        {
            var t = (double)i / count;
            var k1 = Math.Sin((1 - t) * omega) / sinOmega;
            var k2 = Math.Sin(t * omega) / sinOmega;
            var x = (k1 * p.X) + (k2 * q.X);
            var y = (k1 * p.Y) + (k2 * q.Y);
            var z = (k1 * p.Z) + (k2 * q.Z);

            var lat = Math.Asin(Math.Max(-1, Math.Min(1, z))) * 180 / Math.PI;
            var lon = Math.Atan2(y, x) * 180 / Math.PI;
            while (lon - previousLon > 180) lon -= 360;
            while (lon - previousLon < -180) lon += 360;

            result[i] = (lon, lat);
            previousLon = lon;
        }

        return result;
    }

    private static (double X, double Y, double Z) ToUnit(double lon, double lat)
    {
        var la = lat * Math.PI / 180;
        var lo = lon * Math.PI / 180;
        return (Math.Cos(la) * Math.Cos(lo), Math.Cos(la) * Math.Sin(lo), Math.Sin(la));
    }

    /// <summary>Initial compass bearing from a to b, degrees clockwise from north.</summary>
    private static double Bearing((double Lon, double Lat) a, (double Lon, double Lat) b)
    {
        var la1 = a.Lat * Math.PI / 180;
        var la2 = b.Lat * Math.PI / 180;
        var dLon = (b.Lon - a.Lon) * Math.PI / 180;
        var y = Math.Sin(dLon) * Math.Cos(la2);
        var x = (Math.Cos(la1) * Math.Sin(la2)) - (Math.Sin(la1) * Math.Cos(la2) * Math.Cos(dLon));
        return ((Math.Atan2(y, x) * 180 / Math.PI) + 360) % 360;
    }

    /// <summary>The point a distance along a bearing, longitude kept continuous with the start.</summary>
    private static (double Lon, double Lat) DestinationFrom((double Lon, double Lat) from, double bearingDegrees, double meters)
    {
        var la1 = from.Lat * Math.PI / 180;
        var brng = bearingDegrees * Math.PI / 180;
        var delta = meters / EarthRadius;
        var la2 = Math.Asin((Math.Sin(la1) * Math.Cos(delta)) + (Math.Cos(la1) * Math.Sin(delta) * Math.Cos(brng)));
        var dLon = Math.Atan2(Math.Sin(brng) * Math.Sin(delta) * Math.Cos(la1),
            Math.Cos(delta) - (Math.Sin(la1) * Math.Sin(la2)));
        return (from.Lon + (dLon * 180 / Math.PI), la2 * 180 / Math.PI);
    }

    /// <summary>Web Mercator metres; a longitude past +/-180 lands past the world edge on purpose.</summary>
    private static (double X, double Y) Meters(double lon, double lat)
    {
        var clamped = Math.Max(-85.05, Math.Min(85.05, lat));
        return (lon / 180 * HalfWorld, EarthRadius * Math.Log(Math.Tan((Math.PI / 4) + (clamped * Math.PI / 360))));
    }

    // ------------------------------------------------------------------ images ----

    // The desktop sample rasterizes these through WPF, which a phone has no counterpart
    // for. SkiaSharp comes in with ThinkGeo.Core on every platform, and takes the same
    // SVG path data, so the aircraft is the same aircraft.

    /// <summary>A top-down airliner, nose up, drawn at 2x for crisp rotation.</summary>
    private static GeoImage RenderPlaneImage()
    {
        const string data = "M32,2 C35,2 37,6 37,12 L37,26 L61,40 L61,45 L37,38 L36,52 L45,58 L45,61 L32,58 L19,61 L19,58 L28,52 L27,38 L3,45 L3,40 L27,26 L27,12 C27,6 29,2 32,2 Z";
        return Rasterize(64, 64, canvas =>
        {
            using var path = SKPath.ParseSvgPathData(data);
            using var fill = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };
            using var stroke = new SKPaint
            {
                Color = SKColor.Parse("#101317"),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                StrokeJoin = SKStrokeJoin.Round,
            };
            canvas.DrawPath(path, fill);
            canvas.DrawPath(path, stroke);
        });
    }

    private static GeoImage RenderDotImage() => Rasterize(20, 20, canvas =>
    {
        using var fill = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };
        using var stroke = new SKPaint
        {
            Color = SKColor.Parse("#1f4fd6"),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3,
        };
        canvas.DrawCircle(10, 10, 7, fill);
        canvas.DrawCircle(10, 10, 7, stroke);
    });

    private static GeoImage Rasterize(int width, int height, Action<SKCanvas> draw)
    {
        using var surface = SKSurface.Create(new SKImageInfo(width * 2, height * 2, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Scale(2);
        draw(canvas);
        canvas.Flush();

        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return new GeoImage(encoded.ToArray());
    }

    private sealed record Airport(string Code, string Name, double Lon, double Lat);

    public void Dispose()
    {
        _intro?.Cancel();
        StopTicking();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
