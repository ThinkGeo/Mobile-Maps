using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Equals
{
    private bool _initialized;
    private InMemoryFeatureLayer _zoningLayer;
    public Equals()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Set the Map Unit to meters (used in Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a feature layer to hold and display the zoning data
        _zoningLayer = new InMemoryFeatureLayer();

        // Add a style to use to draw the Frisco zoning polygons
        _zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        _zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Import the features from the Frisco zoning data shapefile
        var zoningDataFeatureSource = new ShapeFileFeatureSource(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"));

        // Create a ProjectionConverter to convert the shapefile data from North Central Texas (2276) to Spherical Mercator (3857)
        var projectionConverter = new ProjectionConverter(3857, 2276);

        // For this sample, we have to re-project the features before adding them to the feature layer
        // This is because the topological equality query often does not work when used on a feature layer with a ProjectionConverter, due to rounding issues between projections
        zoningDataFeatureSource.Open();
        projectionConverter.Open();
        foreach (var zoningFeature in zoningDataFeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns))
        {
            var reprojectedFeature = projectionConverter.ConvertToInternalProjection(zoningFeature);
            _zoningLayer.InternalFeatures.Add(reprojectedFeature);
        }

        zoningDataFeatureSource.Close();
        projectionConverter.Close();

        // Create a layer to hold the feature we will perform the spatial query against
        var queryFeatureLayer = new InMemoryFeatureLayer();
        queryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
        queryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a layer to hold features found by the spatial query
        var highlightedFeaturesLayer = new InMemoryFeatureLayer();
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add each feature layer to its own overlay
        // We do this, so we can control and refresh/redraw each layer individually
        var layerOverlay = new LayerOverlay();

        layerOverlay.Layers.Add("Frisco Zoning", _zoningLayer);
        layerOverlay.Layers.Add("Query Feature", queryFeatureLayer);
        layerOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);

        MapView.Overlays.Add("Layer Overlay", layerOverlay);

        // Add a sample shape to the map for the initial query
        // To ensure topological equality for this sample, we create a new shape using the same geometry as an existing feature
        _zoningLayer.Open();
        var sampleShape = _zoningLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns).First().GetShape();
        _zoningLayer.Close();
        await GetFeaturesEqual(sampleShape);

        // Set the map extent to the sample shape
        MapView.MapScale = 15000;
        MapView.CenterPoint = new PointShape(-10776516, 3919246);
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the 'Equals' spatial query using the layer's QueryTools
    /// </summary>
    private static IEnumerable<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
    {
        // Perform the spatial query on features in the specified layer
        layer.Open();
        var features = layer.QueryTools.GetFeaturesTopologicalEqual(shape, ReturningColumnsType.AllColumns);
        layer.Close();

        return features;
    }

    /// <summary>
    ///     Highlight the features that were found by the spatial query
    /// </summary>
    private async Task HighlightQueriedFeatures(IEnumerable<Feature> features)
    {
        // Find the layers we will be modifying in the MapView dictionary
        var layerOverlay = (LayerOverlay)MapView.Overlays["Layer Overlay"];
        var highlightedFeaturesLayer = (InMemoryFeatureLayer)layerOverlay.Layers["Highlighted Features"];

        // Clear the currently highlighted features
        highlightedFeaturesLayer.Open();
        highlightedFeaturesLayer.InternalFeatures.Clear();

        // Add new features to the layer
        foreach (var feature in features) highlightedFeaturesLayer.InternalFeatures.Add(feature);
        highlightedFeaturesLayer.Close();

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeaturesEqual(BaseShape shape)
    {
        // Find the layers we will be modifying in the MapView
        var layerOverlay = (LayerOverlay)MapView.Overlays["Layer Overlay"];
        var queryFeatureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["Query Feature"];

        // Clear the query shape layer and add the newly drawn shape
        queryFeatureLayer.InternalFeatures.Clear();
        queryFeatureLayer.InternalFeatures.Add(new Feature(shape));
        await layerOverlay.RefreshAsync();

        // Perform the spatial query using the drawn shape and highlight features that were found
        var queriedFeatures = PerformSpatialQuery(shape, _zoningLayer);
        await HighlightQueriedFeatures(queriedFeatures);

        // Disable map drawing and clear the drawn shape
        MapView.TrackOverlay.TrackMode = TrackMode.None;
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
    }
}