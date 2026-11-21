using System.Globalization;
using SkiaSharp;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Coyote sightings clustered into tappable discs, rebuilt for the view each time it
/// settles. Tapping a cluster zooms to what is inside it.
/// </summary>
/// <remarks>
/// Drawn as geometry, not as views.
/// <para>
/// Every cluster used to be a MAUI Marker - a ContentView holding an Ellipse and a
/// Label - and reclustering threw all of them away and built new ones. The clustering
/// itself is one pass over a few thousand points and takes under a millisecond; the
/// cost was several hundred native views being constructed, given handlers and laid
/// out on the UI thread, which is the hitch after every pan.
/// </para>
/// <para>
/// The gallery's other answer to "a lot of points that move" is a
/// <see cref="DynamicSource"/>, and Live Flights measures it on this hardware at two
/// thousand points published in under twenty milliseconds, sixty times a second. A
/// cluster is a point with a size, a colour and a number on it, which is exactly what
/// that source draws. So the sample uses it, and loses nothing but the freedom to put
/// arbitrary MAUI content in a bubble - a freedom the Markers and Popups samples
/// already demonstrate twice over, and the one freedom a clustering sample has no use
/// for: you cluster because there are too many things to draw one at a time.
/// </para>
/// </remarks>
public class ClusterMarkers : SamplePage
{
    /// <summary>
    /// Cluster cell size, in screen pixels - wider than the widest disc it can
    /// produce, or neighbouring cells always overlap and the grid shows through the
    /// markers as a pattern.
    /// </summary>
    private const double CellPixels = 86;

    /// <summary>How near a tap has to land, in pixels, to count as hitting a cluster.</summary>
    private const double TapRadiusPixels = 28;

    private readonly DynamicSource _clusters = new();

    /// <summary>What is inside each drawn cluster, by the id it was drawn under.</summary>
    private readonly Dictionary<string, List<PointShape>> _members = new(StringComparer.Ordinal);

    private IDispatcherTimer _recluster;
    private List<PointShape> _sightings;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var style = new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) });

        // Two images and nothing else: a white disc the cluster colour tints, and a
        // small amber dot for a sighting that is on its own. One image drawn at any
        // size is what lets a cluster grow with its count without a second asset.
        style.Images.Add("cluster", ClusterDisc(128));
        style.Images.Add("sighting", SightingDot(40));
        style.AddDynamic(_clusters);

        map.Basemap = new GpuBasemap(style);

        _sightings = ReadSightings();
        map.CenterPoint = new PointShape(-10780320, 3915120);
        map.MapScale = 288900;

        _status = AddStatusLabel();

        // A drag or zoom fires the extent event continuously; recluster once it lands.
        _recluster = Dispatcher.CreateTimer();
        _recluster.Interval = TimeSpan.FromMilliseconds(150);
        _recluster.Tick += (_, _) =>
        {
            _recluster.Stop();
            Recluster();
        };

        map.CurrentExtentChanged += (_, _) =>
        {
            _recluster.Stop();
            _recluster.Start();
        };

        // Geometry has no Tapped event of its own, so the tap is matched against the
        // clusters by distance - which is what a marker's own hit test does, only
        // written where the sample can see it.
        map.SingleTap += (_, e) => TapAt(e.X, e.Y);

        Recluster();
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// Buckets every sighting into a cell of the current screen grid and draws one
    /// marker per bucket - a push of geometry, with no view built or destroyed.
    /// </summary>
    private void Recluster()
    {
        var worldPerPixel = _map.CurrentExtent.Width / Math.Max(1, _map.CanvasWidth);
        var cell = CellPixels * worldPerPixel;

        var buckets = new Dictionary<(int Column, int Row), List<PointShape>>();
        foreach (var sighting in _sightings)
        {
            var key = ((int)Math.Floor(sighting.X / cell), (int)Math.Floor(sighting.Y / cell));
            if (!buckets.TryGetValue(key, out var bucket)) buckets[key] = bucket = new List<PointShape>();
            bucket.Add(sighting);
        }

        _clusters.Clear();
        _members.Clear();

        foreach (var (key, bucket) in buckets)
        {
            var id = FormattableString.Invariant($"{key.Column}:{key.Row}");
            var centre = new PointShape(bucket.Average(p => p.X), bucket.Average(p => p.Y));
            _members[id] = bucket;

            if (bucket.Count == 1)
            {
                _clusters.SetPoint(id, new Feature(centre), sizeInPixel: 13f, iconName: "sighting");
                continue;
            }

            var (size, color, textSize) = StepFor(bucket.Count);
            var feature = new Feature(centre);
            feature.ColumnValues["count"] = bucket.Count.ToString(CultureInfo.InvariantCulture);

            _clusters.SetPoint(id, feature, color: color, sizeInPixel: size, iconName: "cluster");
            _clusters.SetLabel(id, feature, "count", GeoColors.White, textSize);
        }

        _status.Text = FormattableString.Invariant(
            $"{_sightings.Count:N0} sightings in {buckets.Count:N0} clusters");

    }

    /// <summary>
    /// Three steps, not a gradient.
    /// <para>
    /// A continuous ramp gives every cluster on screen its own slightly different
    /// colour, which reads as noise rather than as information - the eye cannot tell
    /// a 40 from a 55 by hue, and trying makes the map busy. Every clustering library
    /// people recognise does the same thing: a few buckets, a few sizes.
    /// </para>
    /// </summary>
    private static (float Size, GeoColor Color, float TextSize) StepFor(int count) => count switch
    {
        < 10 => (44f, GeoColor.FromHtml("#2FB8A8"), 12f),
        < 50 => (58f, GeoColor.FromHtml("#F2A33C"), 14f),
        _ => (74f, GeoColor.FromHtml("#E8604C"), 16f),
    };

    /// <summary>
    /// The cluster nearest the tap, if one is near enough, zoomed to.
    /// </summary>
    private void TapAt(double screenX, double screenY)
    {
        var tapped = _map.ToWorldCoordinate(screenX, screenY, 0);
        if (tapped == null) return;

        var worldPerPixel = _map.CurrentExtent.Width / Math.Max(1, _map.CanvasWidth);
        var reach = TapRadiusPixels * worldPerPixel;

        string nearestId = null;
        var nearest = double.MaxValue;

        foreach (var (id, members) in _members)
        {
            if (members.Count == 1) continue;

            var centre = new PointShape(members.Average(p => p.X), members.Average(p => p.Y));
            var distance = Math.Sqrt(((centre.X - tapped.X) * (centre.X - tapped.X))
                + ((centre.Y - tapped.Y) * (centre.Y - tapped.Y)));

            if (distance > reach || distance >= nearest) continue;

            nearest = distance;
            nearestId = id;
        }

        if (nearestId != null) ZoomToCluster(_members[nearestId]);
    }

    private void ZoomToCluster(List<PointShape> members)
    {
        var minX = members.Min(p => p.X);
        var maxX = members.Max(p => p.X);
        var minY = members.Min(p => p.Y);
        var maxY = members.Max(p => p.Y);

        // A cluster of near-coincident points has no area to zoom to; step the scale
        // instead.
        if (maxX - minX < 1 && maxY - minY < 1)
        {
            _map.CenterPoint = new PointShape((minX + maxX) / 2, (minY + maxY) / 2);
            _map.MapScale /= 4;
        }
        else
        {
            var pad = Math.Max(maxX - minX, maxY - minY) * 0.3;
            _map.CurrentExtent = new RectangleShape(minX - pad, maxY + pad, maxX + pad, minY - pad);
        }

        _ = _map.RefreshAsync();
    }

    /// <summary>
    /// A solid disc inside a wider translucent halo of the same colour, rasterized
    /// once and tinted per cluster.
    /// <para>
    /// This is the shape every clustering library settled on, and the reason is what
    /// happens when two clusters overlap: hard-edged discs with a white ring collide
    /// and argue, while a soft outer ring lets one sit over another and still read as
    /// two. It also gives the marker an edge on a dark basemap without drawing a line
    /// on it.
    /// </para>
    /// <para>
    /// Both rings are drawn in white at different alphas, because the marker's colour
    /// multiplies the image: paint them in colour and the tint lands on top of it. A
    /// difference in alpha is the one thing that survives tinting.
    /// </para>
    /// </summary>
    private static GeoImage ClusterDisc(int pixels)
    {
        using var surface = SKSurface.Create(new SKImageInfo(pixels, pixels, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var centre = pixels / 2f;

        using (var halo = new SKPaint { Color = new SKColor(255, 255, 255, 90), IsAntialias = true })
        {
            canvas.DrawCircle(centre, centre, centre - 1, halo);
        }

        using (var body = new SKPaint { Color = new SKColor(255, 255, 255, 255), IsAntialias = true })
        {
            canvas.DrawCircle(centre, centre, centre * 0.68f, body);
        }

        canvas.Flush();
        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return new GeoImage(encoded.ToArray());
    }

    /// <summary>A lone sighting: a small amber dot with a dark edge, drawn in its own
    /// colours rather than tinted.</summary>
    private static GeoImage SightingDot(int pixels)
    {
        using var surface = SKSurface.Create(new SKImageInfo(pixels, pixels, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var centre = pixels / 2f;
        var radius = centre - 3;

        using (var body = new SKPaint { Color = new SKColor(255, 193, 7, 235), IsAntialias = true })
        {
            canvas.DrawCircle(centre, centre, radius, body);
        }

        using (var edge = new SKPaint
        {
            Color = new SKColor(26, 26, 26, 220),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3,
        })
        {
            canvas.DrawCircle(centre, centre, radius, edge);
        }

        canvas.Flush();
        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return new GeoImage(encoded.ToArray());
    }

    private static List<PointShape> ReadSightings()
    {
        var source = new ShapeFileFeatureSource(SampleShared.Shapefile("Frisco_Coyote_Sightings.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        source.Open();
        var features = source.GetAllFeatures(ReturningColumnsType.NoColumns);
        source.Close();

        return features.Select(f => f.GetShape()).OfType<PointShape>().ToList();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _recluster?.Stop();
    }
}
