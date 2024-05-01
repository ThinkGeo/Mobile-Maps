using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GpxLayer
{
    private bool _initialized;
    public GpxLayer()
    {
        InitializeComponent();
    }

    private async void GPXLayer_OnSizeChanged(object sender, EventArgs e)
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
        var gpxOverlay = new LayerOverlay();
        MapView.Overlays.Add(gpxOverlay);

        // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
        var gpxLayer = new GpxFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Gpx", "Hike_Bike.gpx"))
            {
                FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(4326, 3857)
                }
            };

        // Add the layer to the overlay we created earlier.
        gpxOverlay.Layers.Add("Hike Bike Trails", gpxLayer);

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        gpxLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.Black);
        gpxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 150_000;
        MapView.CenterPoint = new PointShape(-10778823, 3915273);
        await MapView.RefreshAsync();
    }
}