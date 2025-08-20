using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

/// <summary>
/// Demonstrates how to cluster points using a ClusterPointStyle.
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateClusterPointStyle
{
    private bool _initialized;
    public CreateClusterPointStyle()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes the map and overlays when the MapView control is first sized.
    /// This ensures the map is fully ready before layers and styles are applied.
    /// </summary>
    private async void CreateClusterPointStyle_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add ThinkGeo Vector Maps as the background overlay.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Load coyote sightings shapefile (projected to match the map projection).
        var coyoteSightings = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco_Coyote_Sightings.shp"))
        {
            FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        var layerOverlay = new LayerOverlay { TileType = TileType.SingleTile };
        layerOverlay.Layers.Add(coyoteSightings);
        MapView.Overlays.Add(layerOverlay);

        // Apply Cluster Point Style
        await AddClusterPointStyle(coyoteSightings);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        MapView.MapScale = 800000;

        await MapView.RefreshAsync();
    }

    private static async Task AddClusterPointStyle(FeatureLayer layer)
    {
        // Setup the un-clustered point style 
        var unclusteredPointStyle = PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Orange, 10);

        // Setup the clustered point style (paw icon).
        var stream = await FileSystem.OpenAppPackageFileAsync("coyote_paw.png");
        var clusteredPointStyle = new PointStyle(new GeoImage(stream))
        {
            ImageScale = .4,
            Mask = new AreaStyle(GeoPens.Black, GeoBrushes.White),
            MaskType = MaskType.RoundedCorners
        };

        // Create a text style that will display the number of features within a clustered point
        var textStyle = new TextStyle("FeatureCount", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold),
            GeoBrushes.DimGray)
        {
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            YOffsetInPixel = 16
        };

        // Cluster style definition.
        var clusterPointStyle = new ClusterPointStyle(unclusteredPointStyle, textStyle)
        {
            MinimumFeaturesPerCellToCluster = 2,
            ClusteredPointStyle = clusteredPointStyle
        };

        // Apply cluster style across all zoom levels.
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(clusterPointStyle);
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}