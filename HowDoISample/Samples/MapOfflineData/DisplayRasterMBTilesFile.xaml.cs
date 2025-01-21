using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayRasterMBTilesFile
{
    public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();
    private RasterMbTilesAsyncLayer rasterMbTilesLayer;
    private int _logIndex = 0;
    public DisplayRasterMBTilesFile()
    {
        InitializeComponent();

        BindingContext = this;
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
        rasterMbTilesLayer.ProjectedTileCache = new FileRasterTileCache(cachePath, "projected")
        { EnableDebugInfo = true };

        rasterMbTilesLayer.TileCache.GottenCacheTile += TileCache_GottenCacheTile;
        rasterMbTilesLayer.ProjectedTileCache.GottenCacheTile += ProjectedTileCache_GottenCacheTile;

        await rasterMbTilesLayer.CloseAsync();
        await rasterMbTilesLayer.OpenAsync();
        var boundingBox = rasterMbTilesLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);

        await MapView.RefreshAsync();
    }

    private void ProjectedTileCache_GottenCacheTile(object sender, GottenCacheImageBitmapTileCacheEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var message = e.Tile.Content == null ? "Projection Cache Not Hit:" : "Projection Cache Hit:";
            message += $"{e.Tile.ZoomIndex}-{e.Tile.Column}-{e.Tile.Row}";

            AppendLog(message);
        });
    }

    private void TileCache_GottenCacheTile(object sender, GottenCacheImageBitmapTileCacheEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var message = e.Tile.Content == null ? "Cache Not Hit:" : "Cache Hit:";
            message += $"{e.Tile.ZoomIndex}-{e.Tile.Column}-{e.Tile.Row}";

            AppendLog(message);
        });
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

    private async void Projection_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rasterMbTilesLayer == null) return;

        var radioButton = sender as RadioButton;
        if (radioButton?.Content == null) return;

        switch (radioButton.Content.ToString())
        {
            case "EPSG 3857":
                MapView.MapUnit = GeographyUnit.Meter;
                rasterMbTilesLayer.ProjectionConverter = null;
                break;

            case "EPSG 4326":
                MapView.MapUnit = GeographyUnit.DecimalDegree;
                rasterMbTilesLayer.ProjectionConverter = new ProjectionConverter(3857, 4326);
                break;

            default:
                return;
        }

        await rasterMbTilesLayer.CloseAsync();
        await rasterMbTilesLayer.OpenAsync();
        var boundingBox = rasterMbTilesLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);
        await MapView.RefreshAsync();
    }

    public void AppendLog(string message)
    {
        // Add log message to the observable collection
        LogMessages.Add($"{_logIndex++}: {message}");
        LogCollectionView.ScrollTo(LogMessages[LogMessages.Count - 1], animate: true);
    }

    public void Dispose()
    {
        MapView?.Dispose();
        GC.SuppressFinalize(this);
    }
}