using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Crosses
{
    private bool _initialized;
    public Crosses()
    {
        InitializeComponent();
    }

    private async void Crosses_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the Map Unit to meters (used in Spherical Mercator)
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

        // Create a feature layer to hold the Frisco zoning data
        var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"));

        // Convert the Frisco shape file from its native projection to Spherical Mercator, to match the map
        var projectionConverter = new ProjectionConverter(2276, 3857);
        friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the Frisco zoning polygons
        friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a layer to hold the feature we will perform the spatial query against, Create a layer to hold features found by the spatial query
        var highlightLayer = new InMemoryFeatureLayer();
        highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
        highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Red, 6, false);
        highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var friscoOverlay = new LayerOverlay();
        MapView.Overlays.Add("FriscoOverlay", friscoOverlay);
        friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);

        var highlightOverlay = new LayerOverlay();
        MapView.Overlays.Add("HighlightOverlay", highlightOverlay);
        highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

        // Add an event to handle new shapes that are drawn on the map
        MapView.TrackOverlay.TrackEnded += OnLineDrawn;

        // Add a sample shape to the map for the initial query
        var sampleShape = new LineShape("LINESTRING(-10774628 3914024,-10776902 3915582,-10778030 3914368,-10778708 3914445)");

        await GetFeaturesCrossingAsync(sampleShape);

        // Set the map extent to the sample shapes
        MapView.MapScale = 60_000;
        MapView.CenterPoint = new PointShape(-10776668, 3914803);
        MapView.TrackOverlay.TrackMode = TrackMode.Line;
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Perform the spatial query and draw the shapes on the map
    /// </summary>
    private async Task GetFeaturesCrossingAsync(BaseShape shape)
    {
        // Find the layers we will be modifying in the MapView
        var highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
        var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

        var friscoOverlay = (LayerOverlay)MapView.Overlays["FriscoOverlay"];
        var friscoLayer = (FeatureLayer)friscoOverlay.Layers["FriscoLayer"];

        // Clear the query shape layer and add the newly drawn shape
        highlightLayer.InternalFeatures.Clear();
        highlightLayer.InternalFeatures.Add(new Feature(shape));

        // Perform the spatial query using the drawn shape 
        friscoLayer.Open();
        var crossingFeatures = friscoLayer.QueryTools.GetFeaturesCrossing(shape, ReturningColumnsType.AllColumns);

        foreach (var feature in crossingFeatures)
            highlightLayer.InternalFeatures.Add(feature);

        // Highlight the found features
        await highlightOverlay.RefreshAsync();

        // Disable map drawing and clear the drawn shape
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        await MapView.TrackOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Performs the spatial query when a new line is drawn
    /// </summary>
    private async void OnLineDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
    {
        await GetFeaturesCrossingAsync(e.TrackShape);
    }
}