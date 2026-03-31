using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.XYZBasedLayers;

public partial class WMTSAsXYZLayer : IDisposable
{
    private WmtsAsyncLayer _wmtsAsyncLayer;
    private LayerOverlay _layerOverlay;
    private bool _initialized;

    public WMTSAsXYZLayer()
    {
        InitializeComponent();
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized) return;
        _initialized = true;

        _layerOverlay = new LayerOverlay
        {
            TileType = TileType.SingleTile
        };

        Map.Overlays.Add(_layerOverlay);

        _wmtsAsyncLayer = new WmtsAsyncLayer(new Uri("https://wmts.geo.admin.ch/1.0.0"))
        {
            DrawingExceptionMode = DrawingExceptionMode.DrawException,
            ActiveLayerName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale",
            ActiveStyleName = "default",
            OutputFormat = "image/png",
            TileMatrixSetName = "21781_26"
        };

        _layerOverlay.Layers.Add(_wmtsAsyncLayer);

        var cachePath = Path.Combine(FileSystem.AppDataDirectory, "wmtsAsyncLayerCache");

        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }

        _wmtsAsyncLayer.TileCache = new FileRasterTileCache(cachePath, "raw");

        await _wmtsAsyncLayer.CloseAsync();
        await _wmtsAsyncLayer.OpenAsync();
        var boundingBox = _wmtsAsyncLayer.GetBoundingBox();
        await Map.ZoomToAsync(boundingBox);

        await Map.RefreshAsync();
    }

    private async void RenderBeyondMaxZoom_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!_initialized || _wmtsAsyncLayer == null)
            return;

        if (_wmtsAsyncLayer.RenderBeyondMaxZoom == e.Value)
            return;

        _wmtsAsyncLayer.RenderBeyondMaxZoom = e.Value;

        await _layerOverlay.RefreshAsync();
    }

    public void Dispose()
    {
        Map?.Dispose();
        GC.SuppressFinalize(this);
    }
}
