using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class OfflineCloudMapsVectorLayer
{
    private bool _initialized;
    public OfflineCloudMapsVectorLayer()
    {
        InitializeComponent();
    }

    private async void OfflineCloudMapsVectorLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a new overlay that will hold our new layer
        var layerOverlay = new LayerOverlay();
        layerOverlay.TileType = TileType.MultiTile;
        layerOverlay.TileWidth = 512;
        layerOverlay.TileHeight = 512;

        // Create the background world maps using vector tiles stored locally in our MBTiles file and also set the styling though a json file
        var mbTilesLayer = new ThinkGeoMBTilesLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "Frisco.mbtiles"),
           new Uri(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Json", "thinkgeo-world-streets-dark.json"), UriKind.Relative));

        layerOverlay.Layers.Add(mbTilesLayer);

        // Add the overlay to the map
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 40_000;
        MapView.CenterPoint = new PointShape(-10782502, 3911777);
        await MapView.RefreshAsync();
    }
}