using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayWfs
{
    private bool _initialized;
    public DisplayWfs()
    {
        InitializeComponent();
    }

    private async void OpenStreetMapLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
        var thinkGeoVectorOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            // Set up the tile cache for the ThinkGeoCloudVectorMapsOverlay, passing in the location and an ID to distinguish the cache. 
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "thinkgeo_vector_light")
        };
        MapView.Overlays.Add(thinkGeoVectorOverlay);

        var helsinkiParcelsLayer = new WfsV2ProgressiveFeatureLayer("https://inspire-wfs.maanmittauslaitos.fi/inspire-wfs/cp/ows", "cp:CadastralParcel")
        {
            TimeoutInSeconds = 500,
        };
        helsinkiParcelsLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.OrangeRed, 4);
        helsinkiParcelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        helsinkiParcelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(3067, 3857);

        var overlay = new ProgressiveFeaturesTileOverlay()
        {
            FeatureLayer = helsinkiParcelsLayer,
            DrawingBulkCount = 200
        };

        MapView.CenterPoint = new PointShape(2778000, 8435000);
        MapView.MapScale = 20000;

        MapView.Overlays.Add("LayerOverlay", overlay);
        await MapView.RefreshAsync();
    }
}