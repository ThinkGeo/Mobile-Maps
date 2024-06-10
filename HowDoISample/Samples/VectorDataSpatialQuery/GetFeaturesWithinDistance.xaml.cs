using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GetFeaturesWithinDistance
{
    private bool _initialized;
    public GetFeaturesWithinDistance()
    {
        InitializeComponent();
    }

    private async void GetFeaturesWithinDistance_OnSizeChanged(object sender, EventArgs e)
    {
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
        var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"));

        // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
        var projectionConverter = new ProjectionConverter(2276, 3857);
        friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the Frisco zoning polygons
        friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var friscoOverlay = new LayerOverlay();
        friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);
        MapView.Overlays.Add("FriscoOverlay", friscoOverlay);

        // Create a layer to hold features found by the spatial query
        var highlightedFeaturesLayer = new InMemoryFeatureLayer();
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
        highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var highlightOverlay = new LayerOverlay();
        highlightOverlay.Layers.Add("HighlightLayer", highlightedFeaturesLayer);
        MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

        // Add a MarkerOverlay to the map to display the selected point for the query
        var markerOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("MarkerOverlay", markerOverlay);

        // Add a sample point to the map for the initial query
        var sampleShape = new PointShape(-10779425, 3914970);
        await GetFeatures(sampleShape);

        // Set the map extent to the initial area
        MapView.MapScale = 60_000;
        MapView.CenterPoint = new PointShape(-10777932, 3912260);
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeatures(PointShape point)
    {
        // Find the layers we will be modifying in the MapView
        var markerOverlay = (SimpleMarkerOverlay)MapView.Overlays["MarkerOverlay"];
        var friscoOverlay = (LayerOverlay)MapView.Overlays["FriscoOverlay"];
        var friscoLayer = (ShapeFileFeatureLayer)friscoOverlay.Layers["FriscoLayer"];

        // Clear the query point marker overlay and add a marker on the newly drawn point
        markerOverlay.Children.Clear();
        markerOverlay.Children.Add(CreateNewMarker(point));
        await markerOverlay.RefreshAsync();

        // Perform the spatial query on features in the specified layer
        friscoLayer.Open();
        var queriedFeatures = friscoLayer.QueryTools.GetFeaturesWithinDistanceOf(point, GeographyUnit.Meter, DistanceUnit.Meter, (int)SearchRadius.Value, ReturningColumnsType.NoColumns);

        await HighlightQueriedFeatures(queriedFeatures);

        // Disable map drawing and clear the drawn point
        MapView.TrackOverlay.TrackMode = TrackMode.None;
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
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

        await highlightOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query when a new point is drawn
    /// </summary>
    private async void MapView_OnMapClick(object _, TouchMapViewEventArgs e)
    {
        await GetFeatures(e.PointInWorldCoordinate);
    }

    /// <summary>
    ///     Create a new map marker using preloaded image assets
    /// </summary>
    private static Marker CreateNewMarker(PointShape point)
    {
        return new ImageMarker
        {
            Position = point,
            ImagePath = "marker.png",
            TranslationY = -17,
            WidthRequest = 20,
            HeightRequest = 34
        };
    }

    private async void mapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        // Get the selected feature based on the map tap location
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);
        await GetFeatures(pointInWorldCoordinate);
    }
}