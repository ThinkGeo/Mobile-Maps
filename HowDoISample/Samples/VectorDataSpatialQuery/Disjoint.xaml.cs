using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Disjoint
{
    private bool _initialized;
    public Disjoint()
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
        var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"));

        // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
        var projectionConverter = new ProjectionConverter(2276, 3857);
        friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the Frisco zoning polygons
        friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a layer to hold the feature we will perform the spatial query against, 
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
        var sampleShape = new PolygonShape("POLYGON((-10780418 3915973,-10780428 3913422,-10775737 3913413,-10775612 3915954,-10780418 3915973))");

        await GetFeaturesDisjointAsync(sampleShape);

        // Set the map extent to the sample shapes
        MapView.MapScale = 60_000;
        MapView.CenterPoint = new PointShape(-10778020, 3914693);
        MapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeaturesDisjointAsync(BaseShape polygon)
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
        var queriedFeatures = friscoLayer.QueryTools.GetFeaturesDisjointed(polygon, ReturningColumnsType.AllColumns);

        highlightLayer.InternalFeatures.Clear();

        foreach (var feature in queriedFeatures)
            highlightLayer.InternalFeatures.Add(feature);

        // Highlight the found features
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
        await GetFeaturesDisjointAsync((PolygonShape)e.TrackShape);
    }
}