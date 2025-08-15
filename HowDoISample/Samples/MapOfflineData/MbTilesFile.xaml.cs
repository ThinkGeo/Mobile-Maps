using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MbTilesFile
{
    private bool _initialized;
    LayerOverlay _layerOverlay;

    public MbTilesFile()
    {
        InitializeComponent();
    }

    private async void MbTilesFile_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        MapView.MapUnit = GeographyUnit.Meter;

        _layerOverlay = new LayerOverlay
        {
            TileType = TileType.MultiTile,
            ZoomLevelSet = new SphericalMercatorZoomLevelSet(256)
        };
        MapView.Overlays.Add(_layerOverlay);

        var dataFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "maplibre.mbtiles");
        var jsonFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "style.json");

        var openstackMbtiles = new VectorMbTilesAsyncLayer(dataFilePath, jsonFilePath);
        _layerOverlay.Layers.Add(openstackMbtiles);

        await openstackMbtiles.OpenAsync();
        // set up the MapScale of Center Point
        var bbox = openstackMbtiles.GetBoundingBox();
        MapView.MapScale = MapUtil.GetScale(bbox, MapView.CanvasWidth, GeographyUnit.Meter);
        MapView.CenterPoint = bbox.GetCenterPoint();

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
    }

    private async void ShowDebugInfo_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        ThinkGeoDebugger.DisplayTileId = e.Value;
        if (MapView != null)
           await MapView.RefreshAsync();
    }

    private async void SwitchTileSize_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MapView == null) return;
        if (sender is not RadioButton radioButton) return;
        if (!e.Value) return;
        if (MapView.Overlays.Count <= 0) return;

        if (_layerOverlay.Layers[0] is not VectorMbTilesAsyncLayer mbTilesLayer)
            return;

        int tileSize=0;
        string content = radioButton.Content.ToString();
        switch (content)
        {
            case "256 * 256":
                tileSize = 256;
                break;
            case "512 * 512":
                tileSize = 512;
                break;
        }

        // Open if not already opened
        if (!mbTilesLayer.IsOpen)
            await mbTilesLayer.OpenAsync();

        // Get the bounding box from the MBTiles file
        var bbox = mbTilesLayer.GetBoundingBox();

        // Determine zoom count from existing TileMatrixSet (fallback to 20 if null)
        int zoomCount = mbTilesLayer.TileMatrixSet?.TileMatrices.Count ?? 20;

        // Create a new TileMatrixSet based on the MBTiles bounding box
        mbTilesLayer.TileMatrixSet = TileMatrixSet.CreateTileMatrixSet(
            tileSize,
            bbox,
            MapView.MapUnit, // Uses the map's current unit
            zoomCount
        );

        _layerOverlay.ZoomLevelSet = new SphericalMercatorZoomLevelSet(tileSize);

        // Clear any cached tiles from the overlay
        _layerOverlay.TileCache?.ClearCache();

        // Force ThinkGeo to reload tile layout
        await mbTilesLayer.CloseAsync();
        await mbTilesLayer.OpenAsync();
        
        await MapView.RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThinkGeoDebugger.DisplayTileId = false;
    }
}