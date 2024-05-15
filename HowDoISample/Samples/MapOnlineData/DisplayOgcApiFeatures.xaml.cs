using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayOgcApiFeatures
{
    private bool _initialized;
    public DisplayOgcApiFeatures()
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
        var thinkGeoVectorOverlay = new ThinkGeoVectorOverlay()
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            // Set up the tile cache for the ThinkGeoCloudVectorMapsOverlay, passing in the location and an ID to distinguish the cache. 
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "thinkgeo_vector_light")
        };
        MapView.Overlays.Add(thinkGeoVectorOverlay);

        var ignLayer = new OgcApiProgressiveFeatureLayer("https://api-features.ign.es", "namedplace", 200)
        {
            FeatureSource = { ProjectionConverter = new ProjectionConverter(4326, 3857) }
        };
        ignLayer.ZoomLevelSet.ZoomLevel13.DefaultPointStyle = PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Black, 10);
        ignLayer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var overlay = new ProgressiveFeaturesTileOverlay()
        {
            FeatureLayer = ignLayer,
            DrawingBulkCount = 100
        };

        MapView.CenterPoint = new PointShape(242000, 5065000);
        MapView.MapScale = 100000;
        MapView.Overlays.Add("LayerOverlay", overlay);

        await MapView.RefreshAsync();
    }
}