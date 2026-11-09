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
        mapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorTileOverlay(SampleKeys.ApiKey)
        {
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorTileLight_RasterCache")
        };
        mapView.Overlays.Add(backgroundOverlay);

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
        mapView.Overlays.Add(layerOverlay);

        // Apply the heatmap style
        AddHeatStyle(coyoteSightings);

        // Set the map scale and center point
        // The heatmap's radius is in screen pixels, so the scale decides how much the sightings overlap.
        mapView.MapScale = 68000;
        mapView.CenterPoint = new PointShape(-10778000, 3912500);
        await mapView.RefreshAsync();
    }

    private static void AddHeatStyle(FeatureLayer layer)
    {
        // Create the heatmap style: each sighting spreads a Gaussian of heat 22 pixels wide, and
        // the heat adds up where sightings crowd together.
        var heatStyle = new HeatmapStyle
        {
            Radius = 22,
            Intensity = 0.55f,
            Opacity = 0.85f
        };

        // Add the point style to the collection of custom styles for ZoomLevel 1.
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(heatStyle);

        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map. 
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}
