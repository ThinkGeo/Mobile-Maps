using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MbTilesFile
{
    private bool _initialized;
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

        var layerOverlay = new LayerOverlay();
        layerOverlay.TileType = TileType.MultiTile;
        MapView.Overlays.Add(layerOverlay);

        var dataFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "maplibre.mbtiles");
        var jsonFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "style.json");

        var openstackMbtiles = new MbTilesLayer(dataFilePath, jsonFilePath);
        layerOverlay.Layers.Add(openstackMbtiles);


        await openstackMbtiles.OpenAsync();
        // set up the MapScale of Center Point
        var bbox = openstackMbtiles.GetBoundingBox();
        MapView.MapScale = MapUtil.GetScale(bbox, MapView.CanvasWidth, GeographyUnit.Meter);
        MapView.CenterPoint = bbox.GetCenterPoint();

        // Set up the background Color
        var bgColor = openstackMbtiles.BackgroundColor;
        MapView.BackgroundColor = new Color(bgColor.R, bgColor.G, bgColor.B, bgColor.A);

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
        if (MapView == null)
            return;

        if (sender is not RadioButton radioButton)
            return;

        if (!e.Value)
            return;

        if (MapView.Overlays[0] is not LayerOverlay layerOverlay)
            return;

        var tileSize = (string)radioButton.Content == "512 * 512" ? 512 : 256;
        layerOverlay.ZoomLevelSet = new SphericalMercatorZoomLevelSet(tileSize);

        if (layerOverlay.Layers[0] is MbTilesLayer mbTilesLayer)
        {
            mbTilesLayer.ZoomLevelSet = new SphericalMercatorZoomLevelSet(tileSize, MaxExtents.SphericalMercator);
        }

        await MapView.RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThinkGeoDebugger.DisplayTileId = false;
    }
}