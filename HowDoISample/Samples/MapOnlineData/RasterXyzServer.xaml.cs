using System.IO.Compression;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class RasterXyzServer
{
    private ThinkGeoRasterMapsAsyncLayer thinkGeoRasterMapsAsyncLayer;
    private LayerOverlay layerOverlay;

    public RasterXyzServer()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        layerOverlay = new LayerOverlay();
        layerOverlay.TileType = TileType.MultiTile;
        MapView.Overlays.Add(layerOverlay);

        // Add Cloud Maps as a background overlay
        thinkGeoRasterMapsAsyncLayer = new ThinkGeoRasterMapsAsyncLayer
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X1,
        };

        layerOverlay.Layers.Add(thinkGeoRasterMapsAsyncLayer);

        var appDataDirectory = FileSystem.AppDataDirectory;
        var cachePath = Path.Combine(appDataDirectory, "thinkGeoRasterMapsOnlineLayerCache");
        if (!Directory.Exists(cachePath))
            Directory.CreateDirectory(cachePath);
        thinkGeoRasterMapsAsyncLayer.TileCache = new FileRasterTileCache(cachePath, "raw");

        await thinkGeoRasterMapsAsyncLayer.CloseAsync();
        await thinkGeoRasterMapsAsyncLayer.OpenAsync();
        var boundingBox = thinkGeoRasterMapsAsyncLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);

        await MapView.RefreshAsync();
    }

    private async void RenderBeyondMaxZoom_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (thinkGeoRasterMapsAsyncLayer == null) return;

        if (thinkGeoRasterMapsAsyncLayer.RenderBeyondMaxZoom == e.Value)
            return;

        thinkGeoRasterMapsAsyncLayer.RenderBeyondMaxZoom = e.Value;

        await layerOverlay.RefreshAsync();
    }

    public void Dispose()
    {
        MapView?.Dispose();
        GC.SuppressFinalize(this);
    }
}