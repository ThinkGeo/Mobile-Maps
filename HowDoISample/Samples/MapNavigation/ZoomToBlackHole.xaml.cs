using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapNavigation;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ZoomToBlackHole
{
    private List<(PointShape centerPoint, double scale)> _zoomingExtents;
    private int _currentPointIndex;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _initialized;

    public ZoomToBlackHole()
    {
        InitializeComponent();
    }
    private void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        _cancellationTokenSource = new CancellationTokenSource();
        // The DefaultAnimationSettings affects the animation in all operations such as double click
        MapView.DefaultAnimationSettings.Length = 2000;

        ZoomToBlackHoleButton.Clicked += async (_, _) =>
        {
            await StopCurrentAnimationAsync();
            await ZoomToBlackHoleAsync(_cancellationTokenSource.Token);
        };

        DefaultExtentButton.Clicked += async (_, _) =>
        {
            await StopCurrentAnimationAsync();

            try { await MapView.ZoomToExtentAsync(_zoomingExtents[0].centerPoint, _zoomingExtents[0].scale, 0, cancellationToken: _cancellationTokenSource.Token); }
            catch (TaskCanceledException) { }
        };

        // stop the auto zooming whenever touching the map
        MapView.TouchDown += async (_, _) => await StopCurrentAnimationAsync();

        ZoomMapTool.ZoomInButton.Clicked += async (_, _) => await StopCurrentAnimationAsync();
        ZoomMapTool.ZoomOutButton.Clicked += async (_, _) => await StopCurrentAnimationAsync();

        MapView.CurrentExtentChanged += MapViewOnCurrentExtentChanged;
        _zoomingExtents = GetZoomingExtents();
    }

    public async Task StopCurrentAnimationAsync()
    {
        // _cancellationTokenSource.CancelAsync stops all the Async Methods which are using _cancellationTokenSource.Token
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void MapViewOnCurrentExtentChanged(object sender, CurrentExtentChangedMapViewEventArgs e)
    {
        if (!e.IsMapScaleChanged)
            return;

        foreach (var overlay in MapView.Overlays)
        {
            if (overlay is not LayerOverlay layerOverlay)
                continue;
            if (layerOverlay.Layers[0] is not GeoImageLayer geoImageLayer)
                continue;
            if (MapView.MapScale < geoImageLayer.LowerThreshold)
            {
                layerOverlay.Opacity = 0;
                continue;
            }
            if (MapView.MapScale > geoImageLayer.UpperThreshold)
            {
                layerOverlay.Opacity = 0;
                continue;
            }

            var upperRatio = 1 - MapView.MapScale / geoImageLayer.UpperThreshold;
            var lowerRatio = MapView.MapScale / geoImageLayer.LowerThreshold;

            if (upperRatio < 0.4)
                layerOverlay.Opacity = upperRatio * 2.5;
            else if (lowerRatio < 2)
                layerOverlay.Opacity = lowerRatio / 2;
            else
                layerOverlay.Opacity = 1;
        }
    }

    public async Task ZoomToBlackHoleAsync(CancellationToken cancellationToken)
    {
        for (_currentPointIndex = 1; _currentPointIndex < _zoomingExtents.Count; _currentPointIndex++)
        {
            var (centerPoint, scale) = _zoomingExtents[_currentPointIndex];
            var animationSettings = new AnimationSettings { Length = 3000, Type = MapAnimationType.DrawWithAnimation };

            try
            {
                await MapView.ZoomToExtentAsync(centerPoint, scale, 0, animationSettings, cancellationToken: cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private List<(PointShape CenterPoint, double Scale)> GetZoomingExtents()
    {
        var zoomingExtents = new List<(PointShape CenterPoint, double Scale)>();

        var firstLayer = (GeoImageLayer)((LayerOverlay)MapView.Overlays[0]).Layers[0];
        zoomingExtents.Add((firstLayer.CenterPoint, firstLayer.Scale));

        for (var i = 1; i < MapView.Overlays.Count; i++)
        {
            var overlay = MapView.Overlays[i];
            if (overlay is not LayerOverlay layerOverlay)
                continue;
            if (layerOverlay.Layers.Count < 0)
                continue;
            var geoImageLayer = (GeoImageLayer)layerOverlay.Layers[0];
            if (geoImageLayer == null)
                continue;
            zoomingExtents.Add((geoImageLayer.CenterPoint, geoImageLayer.UpperThreshold));
        }

        var lastLayer = (GeoImageLayer)((LayerOverlay)MapView.Overlays[^1]).Layers[0];
        zoomingExtents.Add((lastLayer.CenterPoint, lastLayer.Scale));
        return zoomingExtents;
    }
}