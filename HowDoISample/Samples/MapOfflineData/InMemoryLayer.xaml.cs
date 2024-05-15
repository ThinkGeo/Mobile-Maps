using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class InMemoryLayer
{
    private bool _initialized;
    public InMemoryLayer()
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

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var inMemoryOverlay = new LayerOverlay();
        MapView.Overlays.Add(inMemoryOverlay);

        // Create a new layer that we will pull features from to populate the in memory layer.
        var shapeFileLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco_Mosquitoes.shp"))
        {
            FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        shapeFileLayer.Open();

        // Get all the features from the above layer.
        var features = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
        shapeFileLayer.Close();

        // Create the in memory layer and add it to the map
        var inMemoryFeatureLayer = new InMemoryFeatureLayer();
        inMemoryOverlay.Layers.Add("Frisco Mosquitoes", inMemoryFeatureLayer);

        // Loop through all the features in the first layer and add them to the in memory layer.  We use a shortcut called internal
        // features that is supported in the in memory layer instead of going through the edit tools
        foreach (var feature in features) inMemoryFeatureLayer.InternalFeatures.Add(feature);

        // Create a text style for the label and give it a mask for use below.
        var textStyle = new TextStyle("Trap: [TrapID]", new GeoFont("ariel", 14), GeoBrushes.Black)
        {
            Mask = new AreaStyle(GeoPens.Black, GeoBrushes.White),
            MaskMargin = new DrawingMargin(2, 2, 2, 2),
            YOffsetInPixel = -10
        };

        // Create a point style and add the text style from above on zoom level 1 and then apply it to all zoom levels up to 20.
        inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red, GeoPens.White);
        inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
        inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 150_000;
        MapView.CenterPoint = new PointShape(-10778469, 3914651);
        await MapView.RefreshAsync();
    }
}