using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayRasterMBTilesFile
{
    private RasterMbTilesAsyncLayer rasterMbTilesLayer;
    private LayerOverlay layerOverlay;

    public DisplayRasterMBTilesFile()
    {
        InitializeComponent();
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        layerOverlay = new LayerOverlay();
        Map.Overlays.Add(layerOverlay);
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "Data", "Mbtiles", "test.mbtiles");
        rasterMbTilesLayer = new RasterMbTilesAsyncLayer(filePath);
        layerOverlay.TileType = TileType.MultiTile;
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
        await Map.ZoomToAsync(boundingBox);

        await Map.RefreshAsync();
    }

    private async void RenderBeyondMaxZoom_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rasterMbTilesLayer == null) return;

        if (rasterMbTilesLayer.RenderBeyondMaxZoom == e.Value)
            return;

        rasterMbTilesLayer.RenderBeyondMaxZoom = e.Value;

        await layerOverlay.RefreshAsync();
    }

    public void Dispose()
    {
        Map?.Dispose();
        GC.SuppressFinalize(this);
    }
}
