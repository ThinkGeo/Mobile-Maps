using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Contains
{
    private bool _initialized;
    private ShapeFileFeatureLayer _friscoLayer;
    public Contains()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // Set the Map Unit to meters (used in Spherical Mercator)
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

        // Create a feature layer to hold the Frisco zoning data
        var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp");
        _friscoLayer = new ShapeFileFeatureLayer(filePath);

        // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
        var projectionConverter = new ProjectionConverter(2276, 3857);
        _friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the Frisco zoning polygons
        _friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        _friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var friscoOverlay = new LayerOverlay();
        friscoOverlay.Layers.Add("FriscoLayer", _friscoLayer);
        MapView.Overlays.Add("FriscoOverlay", friscoOverlay);

        // Create a layer to hold features found by the spatial query
        var highlightedFeaturesLayer = new InMemoryFeatureLayer();
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var highlightOverlay = new LayerOverlay();
        highlightOverlay.Layers.Add("HighlightLayer", highlightedFeaturesLayer);
        MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

        // Add a MarkerOverlay to the map to display the selected point for the query
        var markerOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("MarkerOverlay", markerOverlay);

        // Add a sample point to the map for the initial query
        var sampleShape = new PointShape(-10779425, 3914970);
        await GetFeaturesContaining(sampleShape);

        // Set the map extent to Frisco, TX
        MapView.MapScale = 25_000;
        MapView.CenterPoint = new PointShape(-10779558, 3914201);
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeaturesContaining(PointShape point)
    {
        var queryFeatureMarkerOverlay = (SimpleMarkerOverlay)MapView.Overlays["MarkerOverlay"];
        queryFeatureMarkerOverlay.Children.Clear();

        var marker = new ImageMarker
        {
            Position = point,
            ImagePath = "marker.png",
            TranslationY = -17,
            WidthRequest = 20,
            HeightRequest = 34
        };

        // Add the marker to the simpleMarkerOverlay and refresh the map
        queryFeatureMarkerOverlay.Children.Clear();
        queryFeatureMarkerOverlay.Children.Add(marker);
        await queryFeatureMarkerOverlay.RefreshAsync();

        // Perform the spatial query on features in the specified layer
        _friscoLayer.Open();
        var queriedFeatures = _friscoLayer.QueryTools.GetFeaturesContaining(point, ReturningColumnsType.AllColumns);

        await HighlightQueriedFeatures(queriedFeatures);
    }

    /// <summary>
    ///     Highlight the features that were found by the spatial query
    /// </summary>
    private async Task HighlightQueriedFeatures(IEnumerable<Feature> features)
    {
        // Find the layers we will be modifying in the MapView dictionary
        var highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
        var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

        // Clear the currently highlighted features
        highlightLayer.Open();
        highlightLayer.InternalFeatures.Clear();

        // Add new features to the layer
        foreach (var feature in features) highlightLayer.InternalFeatures.Add(feature);

        // Refresh the overlay so the layer is redrawn
        await highlightOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query when a new point is drawn
    /// </summary>
    private async void mapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);
        await GetFeaturesContaining(pointInWorldCoordinate);
    }
}