using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// A drive along a recorded GPS track: the camera flies to each point and the vehicle
/// rides the flight's own progress, the road behind turns green as it is covered, and
/// either theme can be swapped under the whole thing without losing any of it.
/// <para>
/// The map stays free to pan, pinch and rotate during the drive. A finger on the map
/// takes the camera; the vehicle keeps its pace on a clock meanwhile, and a moment
/// after the finger lifts the camera flies back to it, at the scale it was left at.
/// </para>
/// </summary>
public class GpsNavigation : SamplePage, IDisposable
{
    private const double DefaultScale = 5000;

    // How long the vehicle takes from one GPS point to the next: the length of a
    // camera flight while tracking, and a shorter hop in the overview.
    private const int FlightMilliseconds = 1500;
    private const int OverviewMilliseconds = 1000;

    // How long the camera is left alone after the last touch. A pan glides on for up
    // to a second and a half after the finger lifts, and a double-tap zoom takes as
    // long as a flight; this covers both.
    private const int HandsOffMilliseconds = 2000;

    private readonly List<Vertex> _visitedVertices = new();

    private Collection<Vertex> _gpsPoints;
    private ThinkGeoVectorTileSource _tileSource;
    private DynamicSource _paths;
    private DynamicSource _vehicle;
    private CancellationTokenSource _drive;

    // The flight to the next GPS point, while there is one in the air.
    private CancellationTokenSource _flight;
    private PointShape _flightTarget;
    private double _flightProgress;

    private readonly Stopwatch _sinceLastTouch = new();
    private bool _userMovedCamera;
    private double _followScale = DefaultScale;

    private MapView _map;
    private Button _overviewButton;
    private Button _themeButton;
    private Label _status;

    private int _currentGpsPointIndex;
    private bool _showOverview;
    private bool _isDark = true;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        _paths = new DynamicSource();
        _vehicle = new DynamicSource();
        _tileSource = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };

        map.Basemap = new GpuBasemap(CreateStyle(ThinkGeoVectorStyles.Dark));
        map.TiltAngle = 45;
        map.DefaultAnimationSettings = new AnimationSettings
        {
            Type = MapAnimationType.DrawWithAnimation,
            Duration = 1500,
            Easing = Easing.Linear,
        };

        _gpsPoints = CollectGpsData();
        _paths.SetLine("route", new Feature(new LineShape(_gpsPoints)), GeoColor.FromHtml("#FFD400"), 6f);

        // A translucent circle around the destination.
        var end = _gpsPoints[_gpsPoints.Count - 1];
        var ring = new List<Vertex>();
        for (var i = 0; i < 36; i++)
        {
            var angle = i * 2 * Math.PI / 36;
            ring.Add(new Vertex(end.X + (55 * Math.Cos(angle)), end.Y + (55 * Math.Sin(angle))));
        }

        ring.Add(ring[0]);
        _paths.SetArea("destination", new Feature(new PolygonShape(new RingShape(ring))), GeoColor.FromArgb(70, 0, 150, 60));

        _vehicle.SetPoint("vehicle", new Feature(_gpsPoints[0].X, _gpsPoints[0].Y), 0f, GeoColors.White, 26f, "vehicle", lieOnMap: true);

        map.CurrentExtentChangedInAnimation += Map_CurrentExtentChangedInAnimation;
        map.TouchDown += (_, _) => UserTouchedMap();
        map.TouchMove += (_, _) => UserTouchedMap();
        map.TouchUp += (_, _) => UserTouchedMap();
        map.CenterPoint = new PointShape(_gpsPoints[0]);
        map.MapScale = DefaultScale;

        var buttons = new HorizontalStackLayout { Spacing = 10 };
        _overviewButton = new Button { Text = "Overview mode", FontSize = 12, Padding = new Thickness(10, 4) };
        _overviewButton.Clicked += async (_, _) => await ToggleOverviewAsync();
        _themeButton = new Button { Text = "Light theme", FontSize = 12, Padding = new Thickness(10, 4) };
        _themeButton.Clicked += async (_, _) => await ToggleThemeAsync();
        buttons.Add(_overviewButton);
        buttons.Add(_themeButton);
        AddToPanel(buttons);

        _status = AddStatusLabel();
        ShowProgress();

        _drive = new CancellationTokenSource();
        _ = DriveAsync(_drive.Token);
    }

    /// <summary>
    /// One segment per GPS point. Tracking mode flies the camera to each point and the
    /// vehicle follows the flight's progress; overview mode holds the camera and moves
    /// the vehicle on a clock.
    /// <para>
    /// The clock also finishes any segment the flight did not. ZoomToExtentAsync
    /// returns when its flight ends, and a flight ends early - without an exception -
    /// whenever something else moves the camera: every step of a pan or a pinch does
    /// it. So "the call returned" does not mean "the vehicle has arrived". Taking it to
    /// mean that moved the drive on one GPS point for every touch event, dozens a
    /// second, and the vehicle shot down the route under a moving finger.
    /// </para>
    /// </summary>
    private async Task DriveAsync(CancellationToken token)
    {
        await _map.RefreshAsync();

        try
        {
            for (_currentGpsPointIndex = 0; _currentGpsPointIndex < _gpsPoints.Count; _currentGpsPointIndex++)
            {
                _flightProgress = 0;
                if (!_showOverview && !UserHasCamera)
                {
                    await FollowVehicleAsync(_currentGpsPointIndex, token);
                }

                // Whatever is left of the segment, at the pace it would have had.
                var duration = _showOverview ? OverviewMilliseconds : FlightMilliseconds;
                var flown = _flightProgress;
                var clock = Stopwatch.StartNew();
                for (var progress = flown; progress < 1; progress = flown + (clock.Elapsed.TotalMilliseconds / duration))
                {
                    UpdateRoutesAndMarker(progress);
                    await Task.Delay(16, token);
                }

                UpdateRoutesAndMarker(1);
                ShowProgress();
            }
        }
        catch (OperationCanceledException)
        {
            // The page was closed.
        }
    }

    /// <summary>
    /// Flies the camera to one GPS point, then asks for the view it landed on.
    /// <para>
    /// That second step is what keeps the road on screen. An animated move draws its
    /// overlays once, at the start, as DrawIfNotDrawn and concurrently with the
    /// animation about to carry the camera away - so the tiles cut for it are the ones
    /// under the camera when the flight BEGAN, and nothing asks for the ones ahead.
    /// Measured over a minute of driving without this line: one tile scene loaded, and
    /// fifteen hundred frames that drew nothing, with the vehicle still travelling
    /// across an empty background. A landed flight is a committed view, so it is asked
    /// for as one.
    /// </para>
    /// </summary>
    private async Task FollowVehicleAsync(int gpsPointIndex, CancellationToken cancellationToken)
    {
        // Follow at the scale the user left the map at, not the one the drive began with.
        if (_userMovedCamera)
        {
            _followScale = _map.MapScale;
            _userMovedCamera = false;
        }

        var angle = GetRotationAngle(gpsPointIndex, _gpsPoints);

        // The GPS point sits below the centre, so more of the road ahead is in view.
        // The desktop offsets by a fixed 200 pixels; a phone's screen is a different
        // size, so this is a share of its height.
        var offset = _map.CanvasHeight * 0.27;
        var centerPoint = MapUtil.OffsetPointWithScreenOffset(
            new PointShape(_gpsPoints[gpsPointIndex]), 0, offset, angle, _followScale, _map.MapUnit);

        using var flight = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _flight = flight;
        _flightTarget = centerPoint;
        _flightProgress = 0;
        try
        {
            await _map.ZoomToExtentAsync(centerPoint, _followScale, angle, cancellationToken: flight.Token);

            if (_flightProgress >= 1)
            {
                await _map.RefreshAsync();
            }
            else if (!flight.IsCancellationRequested)
            {
                // Cut short, and not from here: a mouse wheel or a double tap took the
                // camera, which raise no touch event of their own.
                UserTouchedMap();
            }
        }
        catch (OperationCanceledException)
        {
            // The flight was called off; the clock takes the vehicle the rest of the way.
        }
        finally
        {
            _flight = null;
            _flightTarget = null;
        }
    }

    /// <summary>
    /// A finger on the map takes the camera at once, and keeps it for a moment after
    /// the last touch.
    /// </summary>
    private void UserTouchedMap()
    {
        if (_showOverview) return;

        _sinceLastTouch.Restart();
        _userMovedCamera = true;
        _flight?.Cancel();
    }

    private bool UserHasCamera => _sinceLastTouch.IsRunning && _sinceLastTouch.ElapsedMilliseconds < HandsOffMilliseconds;

    private void Map_CurrentExtentChangedInAnimation(object sender, CurrentExtentChangedInAnimationMapViewEventArgs e)
    {
        // Every animated camera move raises this - the flight to the overview and a
        // double-tap zoom too. Only the flight to the next GPS point carries the vehicle.
        if (!ReferenceEquals(e.ToCenterPoint, _flightTarget)) return;

        _flightProgress = e.Progress;
        UpdateRoutesAndMarker(e.Progress);
    }

    private void UpdateRoutesAndMarker(double progress)
    {
        if (_currentGpsPointIndex == 0 || _currentGpsPointIndex >= _gpsPoints.Count) return;

        var fromPoint = _gpsPoints[_currentGpsPointIndex - 1];
        var toPoint = _gpsPoints[_currentGpsPointIndex];
        var x = ((toPoint.X - fromPoint.X) * progress) + fromPoint.X;
        var y = ((toPoint.Y - fromPoint.Y) * progress) + fromPoint.Y;

        if (_visitedVertices.Count > 0 && !MapUtil.IsSamePoint(_visitedVertices[^1], fromPoint))
        {
            _visitedVertices.RemoveAt(_visitedVertices.Count - 1);
        }

        _visitedVertices.Add(new Vertex(x, y));

        if (_visitedVertices.Count >= 2)
        {
            _paths.SetLine("visited", new Feature(new LineShape(_visitedVertices)), GeoColors.Green, 6f);
        }

        // The marker takes the true bearing of the segment and counters the view
        // rotation itself.
        var heading = Math.Atan2(toPoint.X - fromPoint.X, toPoint.Y - fromPoint.Y) * 180 / Math.PI;
        _vehicle.SetPoint("vehicle", new Feature(x, y), (float)heading, GeoColors.White, 26f, "vehicle", lieOnMap: true);
    }

    private async Task ToggleOverviewAsync()
    {
        _showOverview = !_showOverview;
        _overviewButton.Text = _showOverview ? "Tracking mode" : "Overview mode";
        _flight?.Cancel();

        // Back in tracking mode the camera returns to the vehicle straight away, at the
        // scale it last followed at.
        _sinceLastTouch.Reset();
        _userMovedCamera = false;

        try
        {
            if (_showOverview)
            {
                var boundingBox = new LineShape(_gpsPoints).GetBoundingBox();

                // 1.5 times the scale that fits the route, so the whole drive has room
                // around it.
                var scale = MapUtil.GetScale(_map.MapUnit, boundingBox, _map.CanvasWidth, _map.CanvasHeight) * 1.5;
                await _map.ZoomToExtentAsync(boundingBox.GetCenterPoint(), scale, 0);
                await _map.RefreshAsync();
            }
            else if (_currentGpsPointIndex >= _gpsPoints.Count)
            {
                // Once the drive is over nothing moves the camera, so bring it back to
                // the vehicle.
                await FollowVehicleAsync(_gpsPoints.Count - 1, CancellationToken.None);
            }
        }
        catch (OperationCanceledException)
        {
            // A newer camera move took over.
        }
    }

    /// <summary>
    /// The same tile source and the same dynamic sources under either theme, so the
    /// route, the destination and the vehicle carry over a switch.
    /// </summary>
    private MapStyle CreateStyle(string styleJson)
    {
        var style = new MapStyle(styleJson, _tileSource);
        style.Images.Add("vehicle", new GeoImage(Path.Combine(SampleShared.DataFolder, "Images", "vehicle_location.png")));
        style.AddDynamic(_paths).AddDynamic(_vehicle);
        return style;
    }

    private async Task ToggleThemeAsync()
    {
        _themeButton.IsEnabled = false;
        try
        {
            await _map.Basemap.SetStyleAsync(CreateStyle(_isDark ? ThinkGeoVectorStyles.Light : ThinkGeoVectorStyles.Dark));
            _isDark = !_isDark;
            _themeButton.Text = _isDark ? "Light theme" : "Dark theme";
        }
        catch (Exception)
        {
            // The style could not be loaded; the map keeps the theme it had.
        }
        finally
        {
            _themeButton.IsEnabled = true;
        }
    }

    private void ShowProgress() => _status.Text = string.Format(CultureInfo.InvariantCulture,
        "point {0} of {1}", Math.Min(_currentGpsPointIndex + 1, _gpsPoints.Count), _gpsPoints.Count);

    private static Collection<Vertex> CollectGpsData()
    {
        var gpsPoints = new Collection<Vertex>();
        var lines = File.ReadAllLines(Path.Combine(SampleShared.DataFolder, "Csv", "vehicle-route.csv"));

        // The GPS points are lat/lon (EPSG:4326); the basemap is Spherical Mercator
        // (EPSG:3857).
        var converter = new ProjectionConverter(4326, 3857);
        converter.Open();

        foreach (var location in lines)
        {
            var posItems = location.Split(',');
            var lat = double.Parse(posItems[0], CultureInfo.InvariantCulture);
            var lon = double.Parse(posItems[1], CultureInfo.InvariantCulture);
            gpsPoints.Add(converter.ConvertToExternalProjection(lon, lat));
        }

        converter.Close();
        return gpsPoints;
    }

    private static double GetRotationAngle(int currentIndex, IReadOnlyList<Vertex> gpsPoints)
    {
        Vertex currentLocation;
        Vertex nextLocation;

        if (currentIndex < gpsPoints.Count - 1)
        {
            currentLocation = gpsPoints[currentIndex];
            nextLocation = gpsPoints[currentIndex + 1];
        }
        else
        {
            currentLocation = gpsPoints[currentIndex - 1];
            nextLocation = gpsPoints[currentIndex];
        }

        if (nextLocation.X - currentLocation.X != 0)
        {
            var dx = nextLocation.X - currentLocation.X;
            var dy = nextLocation.Y - currentLocation.Y;
            return -Math.Atan2(dx, dy) / Math.PI * 180;
        }

        return nextLocation.Y - currentLocation.Y >= 0 ? 0 : 180;
    }

    public void Dispose()
    {
        _drive?.Cancel();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
