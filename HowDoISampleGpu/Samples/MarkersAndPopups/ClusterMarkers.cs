using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Coyote sightings clustered into tappable discs, rebuilt for the view each time it
/// settles. Tapping a cluster zooms to what is inside it.
/// </summary>
public class ClusterMarkers : SamplePage
{
    /// <summary>Cluster cell size, in screen pixels.</summary>
    private const double CellPixels = 64;

    private readonly SimpleMarkerOverlay _markers = new();

    private IDispatcherTimer _recluster;
    private List<PointShape> _sightings;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        _sightings = ReadSightings();
        map.Overlays.Add(_markers);
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

        Recluster();
        _ = map.RefreshAsync();
    }

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

        _markers.ClearMarker();

        foreach (var bucket in buckets.Values)
        {
            var center = new PointShape(bucket.Average(p => p.X), bucket.Average(p => p.Y));
            _markers.AddMarker(Guid.NewGuid().ToString(),
                bucket.Count == 1 ? SightingMarker(center) : ClusterMarker(center, bucket));
        }

        _status.Text = $"{_sightings.Count} sightings in {buckets.Count} clusters";
        _ = _markers.RefreshAsync();
    }

    private static Marker SightingMarker(PointShape at) => new()
    {
        Position = at,
        WidthRequest = 14,
        HeightRequest = 14,
        Content = new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            Fill = new SolidColorBrush(Color.FromRgba(255, 193, 7, 230)),
            Stroke = new SolidColorBrush(Color.FromArgb("#1A1A1A")),
            StrokeThickness = 1.5,
            WidthRequest = 14,
            HeightRequest = 14,
        },
    };

    private Marker ClusterMarker(PointShape at, List<PointShape> members)
    {
        var size = Math.Min(56, 26 + (Math.Sqrt(members.Count) * 4));

        // The desktop pulls a styled disc out of the XAML resources and rebinds it;
        // here the disc is built in code, which is the same two shapes.
        var disc = new Grid { WidthRequest = size, HeightRequest = size };
        disc.Add(new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            Fill = new SolidColorBrush(Color.FromRgba(30, 136, 229, 220)),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2,
        });
        disc.Add(new Label
        {
            Text = members.Count.ToString(),
            TextColor = Colors.White,
            FontSize = size > 40 ? 15 : 12,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        });

        var marker = new Marker
        {
            Position = at,
            WidthRequest = size,
            HeightRequest = size,
            Content = disc,
        };

        marker.Tapped += (_, _) => ZoomToCluster(members);
        return marker;
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
