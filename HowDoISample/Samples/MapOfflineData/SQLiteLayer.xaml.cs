using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SqLiteLayer
{
    private bool _initialized;
    public SqLiteLayer()
    {
        InitializeComponent();
    }

    private async void SQLiteLayer_OnSizeChanged(object sender, EventArgs e)
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
        var restaurantsOverlay = new LayerOverlay();
        MapView.Overlays.Add(restaurantsOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
        var restaurantPath = Path.Combine(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "SQLite", "frisco-restaurants.sqlite"));

        //var restaurantsLayer = new SqliteFeatureLayer($"Data Source={restaurantPath};", "restaurants", "id", "geometry");
        var restaurantsLayer = new SqliteFeatureLayer
        {
            ConnectionString = $"Data Source={restaurantPath};",
            TableName = "restaurants",
            FeatureIdColumn = "id",
            GeometryColumnName = "geometry",
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(2276, 3857)
            }
        };

        // Add the layer to the overlay we created earlier.
        restaurantsOverlay.Layers.Add("Frisco Restaurants", restaurantsLayer);

        // Create a new text style and set various settings to make it look good.
        var textStyle = new TextStyle("Name", new GeoFont("Arial", 10), GeoBrushes.Black)
        {
            MaskType = MaskType.RoundedCorners,
            OverlappingRule = LabelOverlappingRule.NoOverlapping,
            TextPlacement = TextPlacement.LowerLeft,
            Mask = new AreaStyle(GeoBrushes.WhiteSmoke),
            SuppressPartialLabels = true,
            YOffsetInPixel = 1
        };


        // Set a point style and the above text style to zoom level 1 and then apply it to all zoom levels up to 20.
        restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12,
            GeoBrushes.Green, new GeoPen(GeoColors.White, 2));
        restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
        restaurantsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map view current extent to a bounding box that shows just a few restaurants.
        MapView.MapScale = 10_000;
        MapView.CenterPoint = new PointShape(-10776468, 3915061);
        await MapView.RefreshAsync();
    }
}