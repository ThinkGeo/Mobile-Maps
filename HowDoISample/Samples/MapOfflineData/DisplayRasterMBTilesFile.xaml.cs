using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayRasterMBTilesFile
{
    private RasterMbTilesAsyncLayer rasterMbTilesLayer;
    public DisplayRasterMBTilesFile()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        var layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "Data", "Mbtiles", "test.mbtiles");
        rasterMbTilesLayer = new RasterMbTilesAsyncLayer(filePath);
        layerOverlay.TileType = TileType.SingleTile;
        layerOverlay.Layers.Add(rasterMbTilesLayer);

        string cachePath = Path.Combine(FileSystem.AppDataDirectory, "rasterMbTilesLayerCache");

        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }

        rasterMbTilesLayer.TileCache = new FileRasterTileCache(cachePath, "raw");

        await rasterMbTilesLayer.CloseAsync();
        await rasterMbTilesLayer.OpenAsync();
        var boundingBox = rasterMbTilesLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);

        await MapView.RefreshAsync();
    }

    private async void RenderBeyondMaxZoom_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!(sender is CheckBox checkBox)) return;
        if (e == null) return;
        if (rasterMbTilesLayer == null) return;
        
        if (e.Value)
        {
            rasterMbTilesLayer.RenderBeyondMaxZoom = e.Value;
        }

        if (MapView != null)
        {
            await MapView.RefreshAsync();
        }
    }

    public void Dispose()
    {
        MapView?.Dispose();
        GC.SuppressFinalize(this);
    }
}