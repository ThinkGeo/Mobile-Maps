using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Nodes;
using SkiaSharp;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A drive through Wolfsburg at lane level: an OpenDRIVE HD map cut into vector
/// tiles, a follow camera flying one stretch of the trace at a time, and the city's
/// buildings standing around it.
/// </summary>
public class LaneLevelNavigation : SamplePage, IDisposable
{
    // The follow camera: ground height of the view and how far ahead of the vehicle
    // it is centred, the length of one camera flight, and the speed.
    private const double GroundHeightMeters = 60;
    private const double StretchMeters = 24;
    // The desktop sample drives at 12.7 m/s, about 46 km/h, and that is what a device
    // does here too. A simulator runs the app through the interpreter, and at full
    // speed the lane tiles cannot be cut as fast as the camera flies over them, so the
    // road ahead comes up empty - the geometry is right and the throughput is not.
    // Hence the speed picker: on a simulator, drop to a walk and the whole interchange
    // draws.
    private static readonly (string Label, double MetersPerSecond)[] Speeds =
    {
        ("46 km/h", 12.7),
        ("20 km/h", 5.6),
        ("6 km/h - for a simulator", 1.6),
    };

    private double _groundSpeedMetersPerSecond = Speeds[0].MetersPerSecond;

    private static readonly AnimationSettings ViewChange = new()
    {
        Type = MapAnimationType.DrawWithAnimation,
        Duration = 1800,
        Easing = Easing.CubicInOut,
    };

    private const string StyleJson = """
{
  "layers": [

    { "type": "fill",
      "source-layer": "drivable",
      "paint": { "fill-color": "#1b1f24" } },

    { "type": "fill",
      "source-layer": "lane",
      "paint": { "fill-color": "#23282e" } },

    { "type": "fill",
      "source-layer": "route_lane",
      "paint": { "fill-color": "#28395c" } },

    { "type": "line",
      "source-layer": "lane_edge",
      "paint": {
          "line-color": "#0a0c0f",
          "line-opacity": 0.95,
          "line-width": ["interpolate", ["exponential", 2], ["zoom"], 15, 1.2, 17, 2, 18, 3, 19, 5, 20, 9, 21, 15] }
    },

    { "type": "line",
      "source-layer": "lane_edge",
      "paint": {
          "line-color": "#b9c1c9",
          "line-width": ["interpolate", ["exponential", 2], ["zoom"], 15, 0.5, 17, 0.8, 18, 1.2, 19, 2, 20, 4, 21, 7] }
    },

    { "type": "line",
      "source-layer": "lane_marking_dashed",
      "paint": {
          "line-color": "#ffffff",
          "line-dasharray": [3, 3.5],
          "line-width": ["interpolate", ["exponential", 2], ["zoom"], 15, 0.6, 17, 1.0, 18, 1.6, 19, 3, 20, 6, 21, 11] }
    },

    { "type": "line",
      "source-layer": "lane_marking_solid",
      "paint": {
          "line-color": "#ffffff",
          "line-width": ["interpolate", ["exponential", 2], ["zoom"], 15, 0.7, 17, 1.1, 18, 1.8, 19, 3.5, 20, 7, 21, 12] }
    }
  ]
}
""";

    private readonly ThinkGeoVectorTileSource _cloud = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    private MapView _map;
    private Label _speedText;
    private Label _statsText;
    private Label _streetText;
    private Label _instructionText;
    private Label _nextText;
    private Label _progressText;
    private Button _overviewButton;
    private Slider _pitchSlider;

    private bool _disposed;
    private LaneMap _laneMap;
    private FeatureSourceVectorTileSource _laneTiles;
    private DynamicSource _vehicle;
    private GeoImage _vehicleImage;
    private RoutePlayback _playback;
    private RoutePose _pose;
    private bool _trackingMode = true;
    private double _cameraScale;
    private double _stretchStart;
    private double _stretchLength;
    private CancellationTokenSource _flight;
    private CancellationTokenSource _drive;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        _vehicleImage = RenderVehicleImage();
        map.CurrentExtentChangedInAnimation += Map_CurrentExtentChangedInAnimation;

        // A drag while driving leaves tracking mode where the map is, so the next
        // flight does not pull it back. WPF watches PreviewMouseMove with the left
        // button down; a phone has TouchMove.
        map.TouchMove += (_, _) =>
        {
            if (!_trackingMode || _playback == null) return;
            _trackingMode = false;
            _overviewButton.Text = "Tracking mode";
            _flight?.Cancel();
        };

        _streetText = AddStatusLabel();
        _instructionText = AddStatusLabel();
        _nextText = AddStatusLabel();
        _progressText = AddStatusLabel();
        _speedText = AddStatusLabel();
        _statsText = AddStatusLabel();

        AddChoicePicker(Speeds.Select(speed => speed.Label).ToList(), 0, index =>
        {
            _groundSpeedMetersPerSecond = Speeds[index].MetersPerSecond;
            _speedText.Text = string.Format(CultureInfo.InvariantCulture, "{0:0} km/h", _groundSpeedMetersPerSecond * 3.6);
            _flight?.Cancel();
        });

        _overviewButton = new Button { Text = "Overview mode", FontSize = 12, Padding = new Thickness(10, 4) };
        _overviewButton.Clicked += async (_, _) => await ToggleOverviewAsync();
        AddToPanel(_overviewButton);

        AddToPanel(new Label { Text = "Camera pitch", FontSize = 11, TextColor = Colors.Gray });

        // The desktop sample opens at 58 degrees, which a wide window can take: the
        // horizon lands near the top and the road fills the screen. A phone held
        // upright is the other shape, and the same pitch puts the horizon a quarter of
        // the way up with sky above it, so a tall view opens flatter.
        _pitchSlider = new Slider { Minimum = 0, Maximum = 60, Value = IsPortrait ? 34 : 58 };
        _pitchSlider.ValueChanged += (_, e) =>
        {
            if (_trackingMode) ApplyPitch(e.NewValue);
        };
        AddToPanel(_pitchSlider);

        _ = LoadAsync();
    }

    /// <summary>
    /// Builds the whole scene: the lanes as a tile source, the style over the dark
    /// basemap, the vehicle, and the camera at the start of the route - then drives.
    /// </summary>
    private async Task LoadAsync()
    {
        var folder = Path.Combine(SampleShared.DataFolder, "OpenDrive");
        _laneMap = LaneMap.Load(Path.Combine(folder, "XodrWolfsburgLanes.geojson"),
            Path.Combine(folder, "XodrWolfsburgRoute.json"));

        // Every source-layer the style names is one in-memory feature source, cut into
        // vector tiles on the fly - the same path a shapefile or a database would take.
        _laneTiles = new FeatureSourceVectorTileSource { SourceId = "features" };
        foreach (var pair in _laneMap.Sources)
        {
            _laneTiles.FeatureSources.Add(pair.Key, pair.Value);
        }

        _vehicle = new DynamicSource();
        _map.Basemap = new GpuBasemap(ComposeStyle());

        _playback = new RoutePlayback(_laneMap.RouteTrace);
        _pose = _playback.PoseAtDistance(0);
        _cameraScale = 0;

        ApplyPitch(_pitchSlider.Value);
        var scale = CameraScale();
        var (center, rotation) = CameraTarget(_pose, scale);
        _map.MapRotation = rotation;
        _map.CenterPoint = center;
        _map.MapScale = scale;

        ShowVehicle(_pose);
        _speedText.Text = string.Format(CultureInfo.InvariantCulture, "{0:0} km/h", _groundSpeedMetersPerSecond * 3.6);
        _statsText.Text = string.Format(CultureInfo.InvariantCulture, "{0} lane markings, {1:0.0} km route",
            _laneMap.MarkingCount, _laneMap.RouteLength * _laneMap.GroundScale / 1000);

        await _map.RefreshAsync();
        if (_disposed) return;

        _drive = new CancellationTokenSource();
        _ = DriveAsync(_drive.Token);
    }

    /// <summary>
    /// The basemap contributes its background and, after the lanes, its own 3D
    /// buildings - drawn with depth after the ground layers, so they stand in front of
    /// the road behind them rather than under it. No roads, no labels, no land use: the
    /// lanes come from the document above.
    /// </summary>
    private MapStyle ComposeStyle()
    {
        var style = new MapStyle();
        style.SetBackground(GeoColor.FromHtml("#101317"));
        style.AddStyle(LaneStyleForEveryLevel(), _laneTiles);
        style.AddStyle(ThinkGeoVectorStyles.Dark, _cloud, StyleLayerType.FillExtrusion);
        style.Images.Add("vehicle", _vehicleImage);
        style.AddDynamic(_vehicle);
        return style;
    }

    /// <summary>
    /// The lane layers above, once per vertical level of the map (0 ground, 1 bridges),
    /// ground first, so a road passing over another is drawn on top of it.
    /// </summary>
    private static string LaneStyleForEveryLevel()
    {
        var template = JsonNode.Parse(StyleJson)["layers"].AsArray();
        var layers = new JsonArray();
        foreach (var level in new[] { "0", "1" })
        {
            foreach (var layer in template)
            {
                var copy = layer.DeepClone();
                copy["filter"] = new JsonArray("==", new JsonArray("get", "level"), level);
                layers.Add(copy);
            }
        }

        return new JsonObject { ["layers"] = layers }.ToJsonString();
    }

    // ---------------------------------------------------------------- camera ----

    private void ApplyPitch(double degrees)
    {
        degrees = Math.Max(0, Math.Min(60, degrees));
        _map.TiltAngle = degrees;
    }

    private double DefaultCameraScale()
    {
        var height = GroundHeightMeters / _laneMap.GroundScale;
        var aspect = Math.Max(1.1, _map.CanvasWidth / Math.Max(1.0, _map.CanvasHeight));
        var extent = new RectangleShape(
            _pose.Position.X - (height * aspect * 0.5), _pose.Position.Y + (height * 0.5),
            _pose.Position.X + (height * aspect * 0.5), _pose.Position.Y - (height * 0.5));
        return MapUtil.GetScale(_map.MapUnit, extent, _map.CanvasWidth, _map.CanvasHeight);
    }

    // Owned by a pinch once the user has touched it.
    private double CameraScale() => _cameraScale > 0 ? _cameraScale : (_cameraScale = DefaultCameraScale());

    /// <summary>
    /// Where the camera goes for a pose: the map rotated so the heading points up
    /// (MapRotation is clockwise-positive with 0 = north, the heading
    /// counter-clockwise from east, hence heading - 90), and centred a quarter of the
    /// screen ahead of the vehicle. The rotation is taken the short way round from
    /// wherever the map is, so a heading crossing south never spins.
    /// </summary>
    private (PointShape Center, double Rotation) CameraTarget(RoutePose at, double scale)
    {
        var rotation = at.HeadingDegrees - 90;
        var current = _map.MapRotation;
        rotation = current + (((rotation - current + 540) % 360) - 180);

        // A quarter of the screen ahead on a wide window; less on a tall one, where a
        // quarter of a much greater height would leave the vehicle on the bottom edge.
        var ahead = _map.CanvasHeight * (IsPortrait ? 0.14 : 0.25);
        var center = MapUtil.OffsetPointWithScreenOffset(
            new PointShape(at.Position.X, at.Position.Y), 0, ahead, rotation, scale, _map.MapUnit);
        return (center, rotation);
    }

    /// <summary>Taller than wide - a phone held upright, and a different camera.</summary>
    private bool IsPortrait => _map.CanvasHeight > _map.CanvasWidth;

    /// <summary>
    /// The vehicle is one marker in a <see cref="DynamicSource"/>: the map draws it
    /// every frame from this one call, and the source counters the view rotation
    /// itself, so the heading is a plain compass bearing.
    /// </summary>
    private void ShowVehicle(RoutePose at)
    {
        _pose = at;
        var position = new Feature(at.Position.X, at.Position.Y);
        var heading = (float)(90 - at.HeadingDegrees);
        _vehicle.SetPoint("vehicle", position, heading, GeoColors.White, 100f, "vehicle", lieOnMap: true);
        UpdateHud(at);
    }

    private void Map_CurrentExtentChangedInAnimation(object sender, CurrentExtentChangedInAnimationMapViewEventArgs e)
    {
        // A flight in progress: the vehicle sits at the flight's progress along the
        // exact trace, whatever chord the camera itself is interpolating.
        if (!_trackingMode || _stretchLength <= 0 || _playback == null) return;
        ShowVehicle(_playback.PoseAtDistance(_stretchStart + (e.Progress * _stretchLength)));
    }

    /// <summary>
    /// One camera flight per stretch of the trace, at constant speed, the engine
    /// interpolating position, rotation and scale per frame. In overview mode the
    /// camera holds still and the clock moves the vehicle.
    /// </summary>
    private async Task DriveAsync(CancellationToken token)
    {
        var metersPerSecond = _groundSpeedMetersPerSecond / _laneMap.GroundScale;

        while (!token.IsCancellationRequested)
        {
            if (_trackingMode)
            {
                var from = _pose.DistanceMeters;
                if (from >= _playback.Length - 0.5) from = 0;

                var to = Math.Min(_playback.Length, from + StretchMeters);
                _stretchStart = from;
                _stretchLength = to - from;

                var target = _playback.PoseAtDistance(to);
                var scale = CameraScale();
                var (center, rotation) = CameraTarget(target, scale);

                var settings = new AnimationSettings
                {
                    Type = MapAnimationType.DrawWithAnimation,
                    Duration = (uint)Math.Max(16, _stretchLength / metersPerSecond * 1000),
                    Easing = Easing.Linear,
                };

                _flight = CancellationTokenSource.CreateLinkedTokenSource(token);
                try
                {
                    await _map.ZoomToExtentAsync(center, scale, rotation, settings, cancellationToken: _flight.Token);
                    if (!token.IsCancellationRequested) ShowVehicle(target);
                }
                catch (OperationCanceledException)
                {
                    // Re-zoomed or switched to overview mid-flight: the vehicle stays
                    // where the last progress event put it.
                }
                finally
                {
                    _stretchLength = 0;
                }
            }
            else
            {
                var clock = Stopwatch.StartNew();
                var start = _pose.DistanceMeters;
                while (!_trackingMode && !token.IsCancellationRequested)
                {
                    var distance = start + (clock.Elapsed.TotalSeconds * (_groundSpeedMetersPerSecond / _laneMap.GroundScale));
                    if (distance >= _playback.Length)
                    {
                        start = 0;
                        clock.Restart();
                        distance = 0;
                    }

                    ShowVehicle(_playback.PoseAtDistance(distance));
                    await Task.Delay(16, token).ContinueWith(_ => { });
                }
            }
        }
    }

    private async Task ToggleOverviewAsync()
    {
        if (_playback == null) return;

        _trackingMode = !_trackingMode;
        _overviewButton.Text = _trackingMode ? "Overview mode" : "Tracking mode";
        _flight?.Cancel();

        if (_trackingMode)
        {
            // The next flight brings the camera home.
            ApplyPitch(_pitchSlider.Value);
        }
        else
        {
            _map.TiltAngle = 45;
            var extent = _laneMap.OverviewExtent;
            var scale = MapUtil.GetScale(_map.MapUnit, extent, _map.CanvasWidth, _map.CanvasHeight);
            await _map.ZoomToExtentAsync(extent.GetCenterPoint(), scale, 0, ViewChange);
        }
    }

    // ------------------------------------------------------------------- HUD ----

    private void UpdateHud(RoutePose pose)
    {
        var scale = _laneMap.GroundScale;
        _progressText.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.0} of {1:0.0} km",
            pose.DistanceMeters * scale / 1000, _laneMap.RouteLength * scale / 1000);

        // The leg the vehicle is on names the lane; the leg after it is the manoeuvre,
        // with the distance to it.
        var legs = _laneMap.RouteLegs;
        var current = 0;
        while (current + 1 < legs.Count && pose.DistanceMeters >= legs[current + 1].StartDistance)
        {
            current++;
        }

        _streetText.Text = legs[current].Label;

        if (current + 1 < legs.Count)
        {
            var upcoming = legs[current + 1];
            var meters = Math.Max(0, upcoming.StartDistance - pose.DistanceMeters) * scale;
            _instructionText.Text = upcoming.Instruction;
            _nextText.Text = upcoming.Lane.Length == 0
                ? string.Format(CultureInfo.InvariantCulture, "In {0:0} m", meters)
                : string.Format(CultureInfo.InvariantCulture, "In {0:0} m, use the {1} lane", meters, upcoming.Lane);
        }
        else
        {
            var remaining = Math.Max(0, _laneMap.RouteLength - pose.DistanceMeters) * scale;
            _instructionText.Text = "Destination ahead";
            _nextText.Text = string.Format(CultureInfo.InvariantCulture, "In {0:0} m", remaining);
        }
    }

    // --------------------------------------------------------------- vehicle ----

    /// <summary>
    /// The vehicle seen from above - a light body with dark glass and a soft glow,
    /// rasterized once at 2x for the marker batch. Front is up.
    /// </summary>
    /// <remarks>
    /// The desktop sample draws this with WPF shapes. SkiaSharp comes in with
    /// ThinkGeo.Core on every platform and draws the same thing.
    /// </remarks>
    private static GeoImage RenderVehicleImage()
    {
        using var surface = SKSurface.Create(new SKImageInfo(120, 200, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Scale(2);

        using (var glow = new SKPaint { IsAntialias = true })
        {
            glow.Shader = SKShader.CreateRadialGradient(
                new SKPoint(30, 50), 45,
                new[] { new SKColor(0x3f, 0x7d, 0xff, 0x70), new SKColor(0x3f, 0x7d, 0xff, 0x00) },
                null, SKShaderTileMode.Clamp);
            canvas.DrawOval(new SKRect(4, 5, 56, 95), glow);
        }

        using (var body = new SKPaint { Color = SKColor.Parse("#d9dee4"), IsAntialias = true })
        using (var edge = new SKPaint
        {
            Color = SKColor.Parse("#f4f6f8"),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
        })
        {
            var shell = new SKRoundRect(new SKRect(15, 17, 45, 83), 9, 9);
            canvas.DrawRoundRect(shell, body);
            canvas.DrawRoundRect(shell, edge);
        }

        using (var glass = new SKPaint { Color = SKColor.Parse("#23282e"), IsAntialias = true })
        {
            canvas.DrawRoundRect(new SKRoundRect(new SKRect(19, 30, 41, 42), 4, 4), glass);
            canvas.DrawRoundRect(new SKRoundRect(new SKRect(20, 65, 40, 74), 3, 3), glass);
        }

        canvas.Flush();
        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return new GeoImage(encoded.ToArray());
    }

    public void Dispose()
    {
        _disposed = true;
        _drive?.Cancel();
        _flight?.Cancel();
        _laneTiles?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }

    // ------------------------------------------------------------------ data ----

    /// <summary>
    /// One HD map as the converter wrote it: features grouped by the
    /// source-layer the style draws them in, plus the drive.
    /// </summary>
    private sealed class LaneMap
    {
        public IReadOnlyDictionary<string, InMemoryFeatureSource> Sources { get; private set; }
        public IReadOnlyList<Pt> RouteTrace { get; private set; }
        public IReadOnlyList<RouteLeg> RouteLegs { get; private set; }
        public RectangleShape OverviewExtent { get; private set; }
        public double RouteLength { get; private set; }
        public int MarkingCount { get; private set; }
        public string Attribution { get; private set; }

        /// <summary>Ground meters per Web Mercator meter at the route's latitude.</summary>
        public double GroundScale { get; private set; }

        public static LaneMap Load(string lanesFile, string routeFile)
        {
            var lanes = JsonNode.Parse(File.ReadAllText(lanesFile));
            var byLayer = new Dictionary<string, List<Feature>>(StringComparer.Ordinal);
            var columnsByLayer = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (var node in lanes["features"].AsArray())
            {
                var properties = node["properties"].AsObject();
                var layer = properties["layer"].GetValue<string>();
                var geometry = node["geometry"];
                var values = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var pair in properties)
                {
                    values[pair.Key] = pair.Value?.ToString() ?? string.Empty;
                }

                BaseShape shape = geometry["type"].GetValue<string>() == "Polygon"
                    ? Polygon(ProjectCoordinates(geometry["coordinates"][0].AsArray()))
                    : Line(ProjectCoordinates(geometry["coordinates"].AsArray()));

                if (!byLayer.TryGetValue(layer, out var list))
                {
                    byLayer[layer] = list = new List<Feature>();
                    columnsByLayer[layer] = new HashSet<string>(StringComparer.Ordinal);
                }

                columnsByLayer[layer].UnionWith(values.Keys);
                list.Add(new Feature(shape, values));
            }

            var sources = new Dictionary<string, InMemoryFeatureSource>(StringComparer.Ordinal);
            foreach (var pair in byLayer)
            {
                var source = new InMemoryFeatureSource(columnsByLayer[pair.Key].Select(c => new FeatureSourceColumn(c)).ToArray(), pair.Value);
                source.BuildIndex();
                sources[pair.Key] = source;
            }

            // The drive: lane centerline points, each tagged with the lane it
            // lies in, plus that lane's position across the road.
            var routeJson = JsonNode.Parse(File.ReadAllText(routeFile));
            var points = new List<Pt>();
            var ordinals = new List<int>();
            foreach (var p in routeJson["points"].AsArray())
            {
                points.Add(Project(p[0].GetValue<double>(), p[1].GetValue<double>()));
                ordinals.Add(p[2].GetValue<int>());
            }

            var laneInfo = routeJson["lanes"].AsArray()
                .Select(l => (Index: l["index"].GetValue<int>(), Count: l["count"].GetValue<int>(), Intersection: l["intersection"].GetValue<bool>()))
                .ToList();

            // A leg per run of lanes with the same position on the road, so the
            // card names the lane to be in and warns before each intersection.
            var legs = new List<RouteLeg>();
            string previousLabel = null;
            for (var i = 0; i < points.Count; i++)
            {
                var lane = laneInfo[ordinals[i]];
                var label = lane.Intersection
                    ? "Intersection"
                    : string.Format(CultureInfo.InvariantCulture, "Lane {0} of {1}", lane.Index, lane.Count);
                if (label == previousLabel) continue;
                previousLabel = label;

                var position = lane.Count <= 1 ? 0.5 : (lane.Index - 1) / (double)(lane.Count - 1);
                var side = lane.Intersection ? string.Empty : position < 0.34 ? "left" : position > 0.66 ? "right" : "center";
                var instruction = legs.Count == 0 ? "Start in " + label.ToLowerInvariant()
                    : lane.Intersection ? "Cross the intersection"
                    : "Keep to " + label.ToLowerInvariant();
                legs.Add(new RouteLeg(label, PolylineLength(points.Take(i + 1).ToList()), instruction, side));
            }

            var trace = Densify(points, 8);

            var midY = trace[trace.Count / 2].Y;
            var attribution = routeJson["attribution"]?.GetValue<string>() ?? string.Empty;
            return new LaneMap
            {
                Sources = sources,
                RouteTrace = trace,
                RouteLegs = legs,
                OverviewExtent = ExtentFor(trace, 220),
                RouteLength = PolylineLength(trace),
                MarkingCount = byLayer.Where(p => p.Key.StartsWith("lane_marking", StringComparison.Ordinal)).Sum(p => p.Value.Count),
                Attribution = attribution,
                GroundScale = 1.0 / Math.Cosh(midY / 6378137.0)
            };
        }

        private static IReadOnlyList<Pt> ProjectCoordinates(JsonArray coordinates)
        {
            var result = new List<Pt>(coordinates.Count);
            foreach (var c in coordinates)
            {
                result.Add(Project(c[0].GetValue<double>(), c[1].GetValue<double>()));
            }

            return result;
        }

        private static Pt Project(double longitude, double latitude)
        {
            var meters = ProjectionConverter.DecimalDegreesToSphericalMercator.ConvertToExternalProjection(longitude, latitude);
            return new Pt(meters.X, meters.Y);
        }

        private static LineShape Line(IEnumerable<Pt> points) =>
            new LineShape(points.Select(p => new Vertex(p.X, p.Y)).ToArray());

        private static PolygonShape Polygon(IEnumerable<Pt> points)
        {
            var vertices = points.Select(p => new Vertex(p.X, p.Y)).ToList();
            if (vertices.Count > 0 && (vertices[0].X != vertices[vertices.Count - 1].X || vertices[0].Y != vertices[vertices.Count - 1].Y))
            {
                vertices.Add(vertices[0]);
            }

            return new PolygonShape(new RingShape(vertices));
        }

        private static IReadOnlyList<Pt> Densify(IReadOnlyList<Pt> points, double spacing)
        {
            var result = new List<Pt>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];
                var steps = Math.Max(1, (int)Math.Ceiling(Pt.Distance(a, b) / spacing));
                for (var s = 0; s < steps; s++)
                {
                    result.Add(Pt.Lerp(a, b, s / (double)steps));
                }
            }

            result.Add(points[points.Count - 1]);
            return result;
        }

        private static double PolylineLength(IReadOnlyList<Pt> points)
        {
            var total = 0d;
            for (var i = 0; i < points.Count - 1; i++)
            {
                total += Pt.Distance(points[i], points[i + 1]);
            }

            return total;
        }

        private static RectangleShape ExtentFor(IEnumerable<Pt> points, double padding)
        {
            var array = points.ToArray();
            return new RectangleShape(
                array.Min(p => p.X) - padding, array.Max(p => p.Y) + padding,
                array.Max(p => p.X) + padding, array.Min(p => p.Y) - padding);
        }
    }

    /// <summary>One stretch of the route: what the card calls it, where it
    /// starts, the instruction that gets the driver onto it, and the lane to be in.</summary>
    private readonly struct RouteLeg
    {
        public RouteLeg(string label, double startDistance, string instruction, string lane)
        {
            Label = label;
            StartDistance = startDistance;
            Instruction = instruction;
            Lane = lane;
        }

        public string Label { get; }
        public double StartDistance { get; }
        public string Instruction { get; }
        public string Lane { get; }
    }

    /// <summary>Position and heading at any distance along the trace.</summary>
    private sealed class RoutePlayback
    {
        private readonly IReadOnlyList<Pt> _route;

        public RoutePlayback(IReadOnlyList<Pt> route)
        {
            _route = route;
            var total = 0d;
            for (var i = 0; i < route.Count - 1; i++)
            {
                total += Pt.Distance(route[i], route[i + 1]);
            }

            Length = total;
        }

        public double Length { get; }

        public RoutePose PoseAtDistance(double targetDistance)
        {
            var walked = 0d;
            for (var i = 0; i < _route.Count - 1; i++)
            {
                var a = _route[i];
                var b = _route[i + 1];
                var segment = Pt.Distance(a, b);
                if (walked + segment >= targetDistance)
                {
                    var t = segment < 0.001 ? 0 : (targetDistance - walked) / segment;
                    var heading = Math.Atan2(b.Y - a.Y, b.X - a.X) * 180 / Math.PI;
                    return new RoutePose(Pt.Lerp(a, b, t), heading, targetDistance);
                }

                walked += segment;
            }

            var end = _route[_route.Count - 1];
            var previous = _route[Math.Max(0, _route.Count - 2)];
            return new RoutePose(end, Math.Atan2(end.Y - previous.Y, end.X - previous.X) * 180 / Math.PI, Length);
        }
    }

    private readonly struct RoutePose
    {
        public RoutePose(Pt position, double headingDegrees, double distanceMeters)
        {
            Position = position;
            HeadingDegrees = headingDegrees;
            DistanceMeters = distanceMeters;
        }

        public Pt Position { get; }
        public double HeadingDegrees { get; }
        public double DistanceMeters { get; }
    }

    private readonly struct Pt
    {
        public Pt(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public static double Distance(Pt a, Pt b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

        public static Pt Lerp(Pt a, Pt b, double t) => new Pt(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }
}
