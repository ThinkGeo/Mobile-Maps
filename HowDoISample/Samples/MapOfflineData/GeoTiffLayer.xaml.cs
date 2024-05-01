using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GeoTiffLayer
{
    private bool _initialized;
    public GeoTiffLayer()
    {
        InitializeComponent();
    }

    private async void GeoTiffLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.DecimalDegree;

        // Create a new overlay that will hold our new layer and add it to the map.
        var layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);

        // Create the new layer and dd the layer to the overlay we created earlier.
        var geoTiffRasterLayer = new GeoTiffRasterLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "GeoTiff", "World.tif"));
        layerOverlay.Layers.Add(geoTiffRasterLayer);

        // Set the map scale and center point
        MapView.MapScale = 140_000_000;
        MapView.CenterPoint = new PointShape(-10, 8);
        await MapView.RefreshAsync();
    }
}