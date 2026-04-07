using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CommonRasterLayers
{
    private bool _initialized;
    public CommonRasterLayers()
    {
        InitializeComponent();
    }

    private async void CommonRasterLayers_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        mapView.MapUnit = GeographyUnit.Meter;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        mapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var layerOverlay = new LayerOverlay();
        mapView.Overlays.Add(layerOverlay);

        // Create the new layer and dd the layer to the overlay we created earlier.
        var commonRasterLayer = new SkiaRasterLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Jpg", "m_3309650_sw_14_1_20160911_20161121.jpg"));
        layerOverlay.Layers.Add(commonRasterLayer);

        // Set the map scale and center point
        mapView.MapScale = 80_000;
        mapView.CenterPoint = new PointShape(-10780623, 3916194);
        await mapView.RefreshAsync();
    }
}
