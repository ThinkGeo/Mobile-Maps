using System.Collections.ObjectModel;
using System.IO.Compression;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;
using Microsoft.Maui.Storage;

namespace HowDoISample.MapOfflineData;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayRasterFileTiles
{
    public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();
    private XyzFileTilesAsyncLayer fileTilesAsyncLayer;
    private int _logIndex = 0;
    public DisplayRasterFileTiles()
    {
        InitializeComponent();

        BindingContext = this;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        var appDataDirectory = FileSystem.AppDataDirectory;
        var targetFileDirectory = Path.Combine(appDataDirectory, "Data", "OSM_Tiles_z0-z5_Created_By_QGIS");
        if (!Directory.Exists(targetFileDirectory))
        {
            var zipFilePath = Path.Combine(appDataDirectory, "Data", "OSM_Tiles_z0-z5_Created_By_QGIS.zip");
            ZipFile.ExtractToDirectory(zipFilePath, targetFileDirectory);
        }

        var layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);
        fileTilesAsyncLayer = new XyzFileTilesAsyncLayer(targetFileDirectory);
        fileTilesAsyncLayer.MaxZoom = 5; // The MaxZoom with data

        layerOverlay.TileType = TileType.SingleTile;
        layerOverlay.Layers.Add(fileTilesAsyncLayer);

        var cachePath = Path.Combine(appDataDirectory, "FileTilesLayerCache");

        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }

        fileTilesAsyncLayer.TileCache = new FileRasterTileCache(cachePath, "raw");
        fileTilesAsyncLayer.ProjectedTileCache = new FileRasterTileCache(cachePath, "projected")
        { EnableDebugInfo = true };

        fileTilesAsyncLayer.TileCache.GottenCacheTile += TileCache_GottenCacheTile;
        fileTilesAsyncLayer.ProjectedTileCache.GottenCacheTile += ProjectedTileCache_GottenCacheTile;

        await fileTilesAsyncLayer.CloseAsync();
        await fileTilesAsyncLayer.OpenAsync();
        var boundingBox = fileTilesAsyncLayer.GetBoundingBox();
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
        if (fileTilesAsyncLayer == null) return;

        if (e.Value)
        {
            fileTilesAsyncLayer.RenderBeyondMaxZoom = e.Value;
        }

        if (MapView != null)
        {
            await MapView.RefreshAsync();
        }
    }

    private async void Projection_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (fileTilesAsyncLayer == null) return;

        var radioButton = sender as RadioButton;
        if (radioButton?.Content == null) return;

        switch (radioButton.Content.ToString())
        {
            case "EPSG 3857":
                MapView.MapUnit = GeographyUnit.Meter;
                fileTilesAsyncLayer.ProjectionConverter = null;
                break;

            case "EPSG 4326":
                MapView.MapUnit = GeographyUnit.DecimalDegree;
                fileTilesAsyncLayer.ProjectionConverter = new ProjectionConverter(3857, 4326);
                break;

            default:
                return;
        }

        await fileTilesAsyncLayer.CloseAsync();
        await fileTilesAsyncLayer.OpenAsync();
        var boundingBox = fileTilesAsyncLayer.GetBoundingBox();
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

class XyzFileTilesAsyncLayer : RasterXyzTileAsyncLayer
{
    private string _root;

    public XyzFileTilesAsyncLayer(string tilesFolder)
    {
        _root = tilesFolder;
    }

    protected override Task<RasterTile> GetTileAsyncCore(int zoomLevel, long x, long y, float resolutionFactor, CancellationToken cancellationToken)
    {
        var path = $"{_root}\\{zoomLevel}\\{x}\\{y}.jpg";
        if (!File.Exists(path))
            return Task.FromResult(new RasterTile(null, zoomLevel, x, y));

        var bytes = File.ReadAllBytes(path);
        return Task.FromResult(new RasterTile(bytes, zoomLevel, x, y));
    }
}