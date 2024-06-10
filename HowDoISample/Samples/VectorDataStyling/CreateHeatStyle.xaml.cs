using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateHeatStyle
{
    private bool _initialized;
    public CreateHeatStyle()
    {
        InitializeComponent();
    }

    private async void CreateHeatStyle_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        var coyoteSightings = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco_Coyote_Sightings.shp"))
        {
            FeatureSource =
                {
                    // Project the layer's data to match the projection of the map
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        // Add the layer to a layer overlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(coyoteSightings);

        // Add the overlay to the map
        MapView.Overlays.Add(layerOverlay);

        // Apply HeatStyle
        AddHeatStyle(coyoteSightings);

        // Set the map scale and center point
        MapView.MapScale = 200000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }

    private static void AddHeatStyle(FeatureLayer layer)
    {
        // Create the heat style
        var heatStyle = new HeatStyle(20, 1, DistanceUnit.Kilometer);

        // Add the point style to the collection of custom styles for ZoomLevel 1.
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(heatStyle);

        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map. 
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}