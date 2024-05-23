using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Within
{
    private bool _initialized;
    public Within()
    {
        InitializeComponent();
    }

    private async void Within_OnSizeChanged(object sender, EventArgs e)
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

        // Create a layer to hold the feature we will perform the spatial query against
        var queryLayer = new InMemoryFeatureLayer();
        queryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
        queryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a layer to hold features found by the spatial query
        var highlightLayer = new InMemoryFeatureLayer();
        highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
        highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add each feature layer to its own overlay
        // We do this, so we can control and refresh/redraw each layer individually
        var friscoOverlay = new LayerOverlay();
        MapView.Overlays.Add("FriscoOverlay", friscoOverlay);
        friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);

        var highlightOverlay = new LayerOverlay();
        MapView.Overlays.Add("HighlightOverlay", highlightOverlay);
        highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
        highlightOverlay.Layers.Add("QueryLayer", queryLayer);

        // Add an event to handle new shapes that are drawn on the map
        MapView.TrackOverlay.TrackEnded += OnPolygonDrawn;

        // Add a sample shape to the map for the initial query
        var sampleShape = new PolygonShape("POLYGON((-10779148 3916088,-10779960 3913862,-10777189 3911913,-10777179 3915754,-10779148 3916088))");

        await GetFeaturesWithin(sampleShape);

        // Set the map extent to the sample shapes
        MapView.MapScale = 50_000;
        MapView.CenterPoint = new PointShape(-10778569, 3914000);
        MapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeaturesWithin(BaseShape polygon)
    {
        // Find the layers we will be modifying in the MapView
        var highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
        var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];
        var queryLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["QueryLayer"];

        var friscoOverlay = (LayerOverlay)MapView.Overlays["FriscoOverlay"];
        var friscoLayer = (FeatureLayer)friscoOverlay.Layers["FriscoLayer"];

        // Clear the query shape layer and add the newly drawn shape
        queryLayer.InternalFeatures.Clear();
        queryLayer.InternalFeatures.Add(new Feature(polygon));

        // Perform the spatial query using the drawn shape 
        friscoLayer.Open();
        var queriedFeatures = friscoLayer.QueryTools.GetFeaturesWithin(polygon, ReturningColumnsType.AllColumns);

        highlightLayer.InternalFeatures.Clear();

        foreach (var feature in queriedFeatures)
            highlightLayer.InternalFeatures.Add(feature);

        // Highlight the found features
        //TODO: do not refresh automatically
        await highlightOverlay.RefreshAsync();

        // Clear the drawn shape            
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        await MapView.TrackOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Performs the spatial query when a new polygon is drawn
    /// </summary>
    private async void OnPolygonDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
    {
        await GetFeaturesWithin((PolygonShape)e.TrackShape);
    }
}