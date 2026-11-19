using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A small map in the corner showing where the big one is looking: a red frame over
/// the ground the main view covers, following every move of its camera, and a tap on
/// the overview takes the main map there.
/// </summary>
/// <remarks>
/// Not a SamplePage: this one needs two maps. The desktop gallery sets the overview
/// beside the main map; a phone has no room beside anything, so it sits in the
/// corner of the main map, which is where a phone puts an overview anyway.
/// </remarks>
public class OverviewMap : ContentPage
{
    private readonly DynamicSource _frame = new();
    private readonly MapView _mainMap;
    private readonly MapView _previewMap;

    private IDispatcherTimer _follow;
    private (double X, double Y, double Scale, double Rotation, double Tilt) _shown;
    private bool _initialized;

    public OverviewMap()
    {
        _mainMap = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Behaviors = { new AutoLoadMapViewBehavior() },
        };

        _previewMap = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            WidthRequest = 150,
            HeightRequest = 150,
            Behaviors = { new AutoLoadMapViewBehavior() },
        };

        var inset = new Border
        {
            Content = _previewMap,
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Colors.White),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Margin = new Thickness(0, 0, 12, 12),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
        };

        // A tap on the overview takes the main map there. The preview is never panned or
        // zoomed itself, so the gesture has the map to itself.
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, e) =>
        {
            if (!_initialized) return;

            var point = e.GetPosition(_previewMap);
            if (point == null) return;

            var ground = _previewMap.ToWorldCoordinate(point.Value.X, point.Value.Y);
            _ = _mainMap.ZoomToExtentAsync(ground, _mainMap.MapScale, _mainMap.MapRotation,
                new AnimationSettings { Duration = 250 });
        };
        _previewMap.GestureRecognizers.Add(tap);

        Content = new Grid { Children = { _mainMap, inset } };

        _mainMap.SizeChanged += (_, _) => Build();
        _previewMap.SizeChanged += (_, _) => Build();
    }

    private void Build()
    {
        if (_initialized) return;
        if (_mainMap.Width <= 0 || _mainMap.Height <= 0 || _previewMap.Width <= 0 || _previewMap.Height <= 0) return;
        _initialized = true;

        _mainMap.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));
        _mainMap.CenterPoint = new PointShape(-9338030, 3300450);
        _mainMap.MapScale = 4622340;

        var previewStyle = new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) });
        previewStyle.AddDynamic(_frame);
        _previewMap.Basemap = new GpuBasemap(previewStyle);
        _previewMap.CenterPoint = new PointShape(-10502930, 4330070);
        _previewMap.MapScale = _mainMap.MapScale * 50;

        // The frame follows every move of the main map's camera, mid-drag included: a
        // frame drawn only when an event says the extent has changed waits for the
        // finger to come off.
        _follow = Dispatcher.CreateTimer();
        _follow.Interval = TimeSpan.FromMilliseconds(16);
        _follow.Tick += (_, _) => FollowCamera();
        _follow.Start();

        _ = _mainMap.RefreshAsync();
        _ = _previewMap.RefreshAsync();
        UpdateFrame();
    }

    private void FollowCamera()
    {
        if (_mainMap.CenterPoint == null) return;

        var now = (_mainMap.CenterPoint.X, _mainMap.CenterPoint.Y, _mainMap.MapScale, _mainMap.MapRotation, _mainMap.TiltAngle);
        if (now == _shown) return;

        _shown = now;
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        var centerPoint = _mainMap.CenterPoint;
        var width = _mainMap.CanvasWidth;
        var height = _mainMap.CanvasHeight;
        if (centerPoint == null || width <= 0 || height <= 0) return;

        var resolution = MapUtil.GetResolutionFromScale(_mainMap.MapScale, _mainMap.MapUnit);
        var halfWidth = width / 2 * resolution;
        var halfHeight = height / 2 * resolution;

        var ring = new RingShape();

        if (_mainMap.TiltAngle > 0)
        {
            // Past the horizon a corner has no ground under it; the frame then stops a
            // few views out rather than running off the overview.
            var reach = 3 * Math.Sqrt((halfWidth * halfWidth) + (halfHeight * halfHeight));
            foreach (var (x, y) in new[] { (0d, 0d), (width, 0d), (width, height), (0d, height), (0d, 0d) })
            {
                // The three-argument overload goes through the camera that is drawing,
                // so a tilted view answers with the ground the ray meets.
                var ground = _mainMap.ToWorldCoordinate(x, y, 0) ?? centerPoint;
                var dx = ground.X - centerPoint.X;
                var dy = ground.Y - centerPoint.Y;
                var distance = Math.Sqrt((dx * dx) + (dy * dy));
                var clip = distance > reach ? reach / distance : 1;
                ring.Vertices.Add(new Vertex(centerPoint.X + (dx * clip), centerPoint.Y + (dy * clip)));
            }
        }
        else
        {
            var radians = _mainMap.MapRotation * Math.PI / 180.0;
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);
            foreach (var (x, y) in new[]
            {
                (-halfWidth, halfHeight), (halfWidth, halfHeight),
                (halfWidth, -halfHeight), (-halfWidth, -halfHeight), (-halfWidth, halfHeight),
            })
            {
                ring.Vertices.Add(new Vertex(
                    centerPoint.X + (x * cos) - (y * sin),
                    centerPoint.Y + (x * sin) + (y * cos)));
            }
        }

        _frame.SetArea("frame", new Feature(new PolygonShape(ring)), new GeoColor(32, GeoColors.Red), GeoColors.Red, 1f);

        // The near edge, the bottom of the screen, drawn thick: the main map looks from
        // it toward the thin far edge.
        var v = ring.Vertices;
        _frame.SetLine("near", new Feature(new LineShape(new[] { v[3], v[2] })), GeoColors.Red, 5f);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _follow?.Stop();
    }
}
