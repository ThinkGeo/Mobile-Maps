using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ShapefileLayer
{
    private bool _initialized;
    public ShapefileLayer()
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
        var parksOverlay = new LayerOverlay();
        MapView.Overlays.Add(parksOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var parksLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"))
        {
            FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        // Add the layer to the overlay we created earlier.
        parksOverlay.Layers.Add("Frisco Parks", parksLayer);

        // Create a dashed pen that we will use below.
        var dashedPen = new GeoPen(GeoColors.Green, 5);
        dashedPen.DashPattern.Add(1);
        dashedPen.DashPattern.Add(1);

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            new AreaStyle(dashedPen, new GeoSolidBrush(new GeoColor(64, GeoColors.Green)));
        parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 35_000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }
}