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
/// </summary>
public class GpsNavigation : SamplePage, IDisposable
{
    private const double DefaultScale = 5000;

    private readonly List<Vertex> _visitedVertices = new();

    private Collection<Vertex> _gpsPoints;
    private ThinkGeoVectorTileSource _tileSource;
    private DynamicSource _paths;
    private DynamicSource _vehicle;
    private CancellationTokenSource _drive;
    private CancellationTokenSource _segment;

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
    /// the vehicle on its own clock.
    /// </summary>
    private async Task DriveAsync(CancellationToken token)
    {
        await _map.RefreshAsync();

        for (_currentGpsPointIndex = 0; _currentGpsPointIndex < _gpsPoints.Count; _currentGpsPointIndex++)
        {
            _segment = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                if (_showOverview)
                {
                    var clock = Stopwatch.StartNew();
                    while (clock.ElapsedMilliseconds < 1000)
                    {
                        UpdateRoutesAndMarker(clock.ElapsedMilliseconds / 1000.0);
                        await Task.Delay(16, _segment.Token);
                    }

                    UpdateRoutesAndMarker(1);
                }
                else
                {
                    await FollowVehicleAsync(_currentGpsPointIndex, _segment.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _segment.Dispose();
                _segment = null;
            }

            ShowProgress();
            if (token.IsCancellationRequested) return;
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
        var angle = GetRotationAngle(gpsPointIndex, _gpsPoints);

        // The GPS point sits below the centre, so more of the road ahead is in view.
        // The desktop offsets by a fixed 200 pixels; a phone's screen is a different
        // size, so this is a share of its height.
        var offset = _map.CanvasHeight * 0.27;
        var centerPoint = MapUtil.OffsetPointWithScreenOffset(
            new PointShape(_gpsPoints[gpsPointIndex]), 0, offset, angle, DefaultScale, _map.MapUnit);

        await _map.ZoomToExtentAsync(centerPoint, DefaultScale, angle, cancellationToken: cancellationToken);
        if (!cancellationToken.IsCancellationRequested) await _map.RefreshAsync();
    }

    private void Map_CurrentExtentChangedInAnimation(object sender, CurrentExtentChangedInAnimationMapViewEventArgs e)
    {
        if (!MapUtil.IsSameDouble(e.FromResolution, e.ToResolution)) return;
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
        _segment?.Cancel();

        try
        {
            if (_showOverview)
            {
                var boundingBox = new LineShape(_gpsPoints).GetBoundingBox();

                // 1.5 times the scale that fits the route, so the whole drive has room
                // around it.
                var scale = MapUtil.GetScale(_map.MapUnit, boundingBox, _map.CanvasWidth, _map.CanvasHeight) * 1.5;
                await _map.ZoomToExtentAsync(boundingBox.GetCenterPoint(), scale, 0);
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
