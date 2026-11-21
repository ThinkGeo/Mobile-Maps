using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Markers are views on the map, not drawings in it: a pin whose tip points at its
/// ground point, an animated one made of a pulsing circle, tap to add or remove, and
/// a red dot drawn by the renderer at the same place for contrast.
/// </summary>
/// <remarks>
/// A MAUI Marker is a ContentView, so anything MAUI can draw can be a marker - the
/// animated icon here is a MAUI animation rather than WPF's storyboard. Dragging is
/// the one thing that does not carry over: the desktop overlay has a DragMode and
/// MarkerDragging events, and the MAUI SimpleMarkerOverlay has add, remove and tap.
/// </remarks>
public class Markers : SamplePage
{
    // Geometry drawn AT the centre point, for contrast with the marker image whose tip
    // points at it.
    private readonly DynamicSource _centerDot = new();
    private readonly SimpleMarkerOverlay _markerOverlay = new();

    private MapView _map;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.Images.Add("dot", PointStyle.CreateSimpleCircleStyle(GeoColors.Red, 10));
        style.AddDynamic(_centerDot);

        map.Basemap = new GpuBasemap(style);
        map.CenterPoint = new PointShape(-10777290, 3908740);
        map.MapScale = 9030;

        // A marker stays on its ground point under perspective, so the sample opens
        // tilted.
        map.TiltAngle = 40;

        _centerDot.SetPoint("center", new Feature(map.CenterPoint), sizeInPixel: 10f, iconName: "dot");
        _markerOverlay.AddMarker(Guid.NewGuid().ToString(), NewMarker(map.CenterPoint));
        _markerOverlay.AddMarker(Guid.NewGuid().ToString(), NewAnimatedMarker(new PointShape(-10777000, 3908900)));
        map.Overlays.Add(_markerOverlay);

        // Tap the map to add one, tap a marker to take it away, and that is the whole
        // sample. There was a two-cell strip to choose between the two and a running
        // log of what had been added; both were the card telling you what you had
        // just watched happen on the map.
        //
        // The ground point is asked for through the camera - ToWorldCoordinate with a
        // height of zero - because a tap on a leaning map means where the ray meets
        // the ground, not where a flat view of the same scale would put it.
        map.SingleTap += (_, e) =>
        {
            var world = _map.ToWorldCoordinate(e.X, e.Y, 0);
            if (world == null) return;   // above the horizon

            _markerOverlay.AddMarker(Guid.NewGuid().ToString(), NewMarker(world));
            _ = _markerOverlay.RefreshAsync();
        };

        _ = map.RefreshAsync();
    }

    /// <summary>The pin: its tip, not its middle, sits on the ground point.</summary>
    private Marker NewMarker(PointShape position)
    {
        var marker = new Marker
        {
            Position = position,
            WidthRequest = 20,
            HeightRequest = 34,

            // The image is 20 by 34 with the point at the bottom, so the view is
            // lifted by half its height to put the tip on the ground point - the
            // counterpart of the desktop's YOffset.
            TranslationY = -17,
            Content = new Image
            {
                Source = ImageSource.FromFile(Path.Combine(SampleShared.DataFolder, "Images", "marker.png")),
                WidthRequest = 20,
                HeightRequest = 34,
            },
        };

        marker.Tapped += (sender, _) => Remove(sender as Marker);
        return marker;
    }

    /// <summary>
    /// The animated one. WPF hands the marker a UserControl with a storyboard; a MAUI
    /// marker is a ContentView, so the animation is a MAUI one on an ordinary ellipse.
    /// </summary>
    private Marker NewAnimatedMarker(PointShape position)
    {
        var ring = new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            Fill = new SolidColorBrush(Color.FromRgba(30, 136, 229, 120)),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2,
            WidthRequest = 28,
            HeightRequest = 28,
        };

        var marker = new Marker
        {
            Position = position,
            WidthRequest = 28,
            HeightRequest = 28,
            Content = ring,
        };

        marker.Loaded += async (_, _) =>
        {
            while (marker.IsLoaded)
            {
                await ring.ScaleTo(1.6, 700, Easing.SinOut);
                await ring.ScaleTo(1.0, 700, Easing.SinIn);
            }
        };

        marker.Tapped += (sender, _) => Remove(sender as Marker);
        return marker;
    }

    private void Remove(Marker marker)
    {
        if (marker == null) return;

        _markerOverlay.Children.Remove(marker);
        _ = _markerOverlay.RefreshAsync();
    }
}
