using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TinyGeoLayer
{
    private bool _initialized;
    public TinyGeoLayer()
    {
        InitializeComponent();
    }

    private async void TinyGeoLayer_OnSizeChanged(object sender, EventArgs e)
    {
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
        var tinyGeoOverlay = new LayerOverlay();
        MapView.Overlays.Add(tinyGeoOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var tinyGeoLayer = new TinyGeoFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "TinyGeo", "Zoning.tgeo"))
            {
                FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
            };

        // Add the layer to the overlay we created earlier.
        tinyGeoOverlay.Layers.Add("Zoning", tinyGeoLayer);

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        tinyGeoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.Black,
            new GeoSolidBrush(new GeoColor(50, GeoColors.Blue)));
        tinyGeoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 250_000;
        MapView.CenterPoint = new PointShape(-10778772, 3915250);
        await MapView.RefreshAsync();
    }
}