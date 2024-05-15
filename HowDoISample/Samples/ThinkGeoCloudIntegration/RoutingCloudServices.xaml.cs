using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class RoutingCloudServices
{
    private bool _initialized;
    private RoutingCloudClient _routingCloudClient;
    public RoutingCloudServices()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters (Spherical Mercator)
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

        // Create a new feature layer to display the route
        var routingLayer = new InMemoryFeatureLayer();

        // Add styles to display the route and waypoints
        // Add a point, line, and text style to the layer. These styles control how the route will be drawn and labeled
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Star, 24,
            GeoBrushes.MediumPurple, GeoPens.Purple);
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 3, false);
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateMaskTextStyle("SequenceNumber",
            new GeoFont("Verdana", 20), GeoBrushes.White,
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.Black), GeoColors.Black, 2), 0, 0);
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.TextPlacement = TextPlacement.Upper;
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.YOffsetInPixel = -8;
        routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.OverlappingRule =
            LabelOverlappingRule.AllowOverlapping;
        routingLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a feature layer to highlight selected features
        var highlightLayer = new InMemoryFeatureLayer();

        // Add styles to display the highlighted route features
        highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.BrightYellow, 6, GeoColors.Black, 2, false);
        highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layers to an overlay, and add the overlay to the mapview
        var routingOverlay = new LayerOverlay();
        routingOverlay.Layers.Add("Routing Layer", routingLayer);
        routingOverlay.Layers.Add("Highlight Layer", highlightLayer);
        MapView.Overlays.Add("Routing Overlay", routingOverlay);

        // Set the map extent to Frisco, TX
        MapView.CenterPoint = new PointShape(-10777600, 3915260);
        MapView.MapScale = 240000;

        // Initialize the RoutingCloudClient with our ThinkGeo Cloud Client credentials
        _routingCloudClient = new RoutingCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        // Run the routing request
        RouteWaypoints();

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Set options and perform routing using the RoutingCloudClient through a preset set of waypoints
    /// </summary>
    private async Task<CloudRoutingGetRouteResult> GetRoute(IEnumerable<PointShape> waypoints)
    {
        // Set up options for the routing request
        // Enable turn-by-turn instructions to receive step-by-step guidance
        var options = new CloudRoutingGetRouteOptions
        {
            TurnByTurn = true
        };

        var route = await _routingCloudClient.GetRouteAsync(waypoints, 3857, options);

        return route;
    }

    /// <summary>
    ///     Draw the result of a Cloud Routing request on the map
    /// </summary>
    private async Task DrawRoute(CloudRoutingGetRouteResult routingResult)
    {
        // Get the routing feature layer from the MapView
        var routingOverlay = (LayerOverlay)MapView.Overlays["Routing Overlay"];
        var routingLayer = (InMemoryFeatureLayer)routingOverlay.Layers["Routing Layer"];

        // Clear the previous features from the routing layer
        routingLayer.InternalFeatures.Clear();

        // Create a collection to hold the route segments. These include information like distance, duration, warnings, and instructions for turn-by-turn routing
        var routeSegments = new List<CloudRoutingSegment>();

        var index = 0;
        // Add the route waypoints and route segments to the map
        foreach (var waypoint in routingResult.RouteResult.Waypoints)
        {
            var columnValues = new Dictionary<string, string>
            {
                // Get the order of the stops and label the point
                // '0' represents the start/end point of the route for a round trip route, so we change the label to indicate that for readability
                { "SequenceNumber", index == 0 ? "Start Point" : "Stop " + index }
            };
            var routeWaypoint = new PointShape(waypoint.Coordinate);

            // Add the point to the map
            routingLayer.InternalFeatures.Add(new Feature(routeWaypoint, columnValues));

            // Increment the index for labeling purposes
            index++;
        }

        foreach (var route in routingResult.RouteResult.Routes)
        {
            routingLayer.InternalFeatures.Add(new Feature(route.Shape));
            routeSegments.AddRange(route.Segments);
        }

        // Set the data source for the list box to the route segments
        LsbRouteSegments.ItemsSource = routeSegments;

        // Set the map extent to the newly displayed route
        routingLayer.Open();
        await MapView.ZoomToExtentAsync(AreaBaseShape.ScaleUp(routingLayer.GetBoundingBox(), 20).GetCenterPoint(),
            50000, 0, new AnimationSettings());
        routingLayer.Close();

        await routingOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Perform routing using the RoutingCloudClient through a preset set of waypoints
    /// </summary>
    private async void RouteWaypoints()
    {
        // Create a set of preset waypoints to route through
        var startPoint = new PointShape(-10776986.85, 3908680.24);
        var waypoint1 = new PointShape(-10776836.12, 3912348.04);
        var waypoint2 = new PointShape(-10778917.01, 3909965.17);
        var endPoint = new PointShape(-10779631.80, 3915721.82);

        // Send the routing request
        var routingResult = await GetRoute([startPoint, waypoint1, waypoint2, endPoint]);

        // Handle an exception returned from the service
        if (routingResult.Exception != null)
        {
            await DisplayAlert("Error", routingResult.Exception.Message, "OK");
            return;
        }

        //Draw the result on the map
        await DrawRoute(routingResult);
    }

    /// <summary>
    ///     When a route segment is selected in the UI, highlight it
    /// </summary>
    private async void lsbRouteSegments_SelectionChanged(object sender,
        SelectedItemChangedEventArgs selectedItemChangedEventArgs)
    {
        var routeSegments = (ListView)sender;
        if (routeSegments.SelectedItem == null) return;
        var routingOverlay = (LayerOverlay)MapView.Overlays["Routing Overlay"];
        var highlightLayer = (InMemoryFeatureLayer)routingOverlay.Layers["Highlight Layer"];
        highlightLayer.InternalFeatures.Clear();

        // Highlight the selected route segment
        highlightLayer.InternalFeatures.Add(
            new Feature(((CloudRoutingSegment)routeSegments.SelectedItem).Shape));

        // Zoom to the selected feature and zoom out to an appropriate level 
        await MapView.ZoomToExtentAsync(((CloudRoutingSegment)routeSegments.SelectedItem).Shape.GetBoundingBox().GetCenterPoint(),
            10000, 0, new AnimationSettings());

        await routingOverlay.RefreshAsync();
    }
}