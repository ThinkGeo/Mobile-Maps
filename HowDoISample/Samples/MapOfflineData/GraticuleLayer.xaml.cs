using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GraticuleLayer
{
    private bool _initialized;
    public GraticuleLayer()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);

        // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
        var graticuleFeatureLayer = new GraticuleFeatureLayer
        {
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(4326, 3857)
            }
        };

        // We set the pen color to the graticule layer.
        graticuleFeatureLayer.GraticuleLineStyle.OuterPen.Color = GeoColor.FromArgb(125, GeoColors.Navy);

        // Add the layer to the overlay we created earlier.
        layerOverlay.Layers.Add("graticule", graticuleFeatureLayer);

        // Set the map scale and center point
        MapView.MapScale = 35_000;
        MapView.CenterPoint = new PointShape(-10777884, 3914992);
        await MapView.RefreshAsync();
    }
}