using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateAreaStyle
{
    private bool _initialized;
    public CreateAreaStyle()
    {
        InitializeComponent();
    }

    private async void CreateAreaStyle_OnSizeChanged(object sender, EventArgs e)
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

        var friscoSubdivisions = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"))
            {
                FeatureSource =
                {
                    // Project the layer's data to match the projection of the map
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
            };

        // Add the layer to a layer overlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(friscoSubdivisions);

        // Add the overlay to the map
        MapView.Overlays.Add(layerOverlay);

        // Add the area style to the historicSites layer
        AddAreaStyle(friscoSubdivisions);

        // Set the map scale and center point
        MapView.MapScale = 40000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }

    private static void AddAreaStyle(FeatureLayer layer)
    {
        // Create an area style
        var areaStyle = new AreaStyle(GeoPens.DimGray, new GeoSolidBrush(new GeoColor(128, GeoColors.ForestGreen)));

        // Add the area style to the collection of custom styles for ZoomLevel 1.
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(areaStyle);

        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the area style on every zoom level on the map.
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}