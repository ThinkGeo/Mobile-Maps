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

    private async void GraticuleLayer_OnSizeChanged(object sender, EventArgs e)
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

        // Create adornment overlay
        var adornmentOverlay = new AdornmentOverlay();

        mapView.Overlays.Add(adornmentOverlay);

        // Create graticule adornment layer
        var graticuleLayer = new GraticuleAdornmentLayer
        {
            Projection = new Projection(3857)
        };

        // Set style
        graticuleLayer.GraticuleLineStyle.OuterPen.Color =GeoColor.FromArgb(125, GeoColors.Navy);

        // Add layer
        adornmentOverlay.Layers.Add(graticuleLayer);

        // Set the map scale and center point
        mapView.MapScale = 35_000;
        mapView.CenterPoint = new PointShape(-10777884, 3914992);
        await mapView.RefreshAsync();
    }
}
