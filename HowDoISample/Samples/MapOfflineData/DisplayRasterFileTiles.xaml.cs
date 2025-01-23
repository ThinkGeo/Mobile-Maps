using System.IO;
using System.IO.Compression;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

public partial class DisplayRasterFileTiles
{
    private XyzFileTilesAsyncLayer fileTilesAsyncLayer;
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

        await fileTilesAsyncLayer.CloseAsync();
        await fileTilesAsyncLayer.OpenAsync();
        var boundingBox = fileTilesAsyncLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);

        await MapView.RefreshAsync();
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
        var path = Path.Combine(_root, zoomLevel.ToString(), x.ToString(), $"{y}.jpg");
        if (!File.Exists(path))
            return Task.FromResult(new RasterTile(null, zoomLevel, x, y));

        var bytes = File.ReadAllBytes(path);
        return Task.FromResult(new RasterTile(bytes, zoomLevel, x, y));
    }
}