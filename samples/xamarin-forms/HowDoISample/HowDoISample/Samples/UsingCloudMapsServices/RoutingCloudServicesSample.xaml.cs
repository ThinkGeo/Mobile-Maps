using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the RoutingCloudClient to route through a set of waypoints with the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutingCloudServicesSample : ContentPage
    {
        private RoutingCloudClient routingCloudClient;

        public RoutingCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay, as well as a feature layer to display the route
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

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
            mapView.Overlays.Add("Routing Overlay", routingOverlay);

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent =
                new RectangleShape(-10782288.5963252, 3917613.16813763, -10777138.4036748, 3913574.08186237);

            // Initialize the RoutingCloudClient with our ThinkGeo Cloud Client credentials
            routingCloudClient = new RoutingCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            // Run the routing request
            RouteWaypoints();

            mapView.Refresh();
        }

        /// <summary>
        ///     Set options and perform routing using the RoutingCloudClient through a preset set of waypoints
        /// </summary>
        private async Task<CloudRoutingGetRouteResult> GetRoute(Collection<PointShape> waypoints)
        {
            // Set up options for the routing request
            // Enable turn-by-turn so we get turn by turn instructions
            var options = new CloudRoutingGetRouteOptions();
            options.TurnByTurn = true;

            var route = await routingCloudClient.GetRouteAsync(waypoints, 3857, options);

            return route;
        }

        /// <summary>
        ///     Draw the result of a Cloud Routing request on the map
        /// </summary>
        private void DrawRoute(CloudRoutingGetRouteResult routingResult)
        {
            // Get the routing feature layer from the MapView
            var routingLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Routing Layer");

            // Clear the previous features from the routing layer
            routingLayer.InternalFeatures.Clear();

            // Create a collection to hold the route segments. These include information like distance, duration, warnings, and instructions for turn-by-turn routing
            var routeSegments = new List<CloudRoutingSegment>();

            var index = 0;
            // Add the route waypoints and route segments to the map
            foreach (var waypoint in routingResult.RouteResult.Waypoints)
            {
                var columnValues = new Dictionary<string, string>();

                // Get the order of the stops and label the point
                // '0' represents the start/end point of the route for a round trip route, so we change the label to indicate that for readability
                columnValues.Add("SequenceNumber", index == 0 ? "Start Point" : "Stop " + index);
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
            lsbRouteSegments.ItemsSource = routeSegments;

            // Set the map extent to the newly displayed route
            routingLayer.Open();
            mapView.CurrentExtent = AreaBaseShape.ScaleUp(routingLayer.GetBoundingBox(), 20).GetBoundingBox();

            routingLayer.Close();
            mapView.Refresh();
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

            // Show a loading graphic to let users know the request is running
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            // Send the routing request
            var routingResult = await GetRoute(new Collection<PointShape> {startPoint, waypoint1, waypoint2, endPoint});

            // Hide the loading graphic
            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;

            // Handle an exception returned from the service
            if (routingResult.Exception != null)
            {
                await DisplayAlert("Error", routingResult.Exception.Message, "OK");
                return;
            }

            //Draw the result on the map
            DrawRoute(routingResult);
        }

        /// <summary>
        ///     When a route segment is selected in the UI, highlight it
        /// </summary>
        private void lsbRouteSegments_SelectionChanged(object sender,
            SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            var routeSegments = (ListView) sender;
            if (routeSegments.SelectedItem != null)
            {
                var highlightLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Highlight Layer");
                highlightLayer.InternalFeatures.Clear();

                // Highlight the selected route segment
                highlightLayer.InternalFeatures.Add(
                    new Feature(((CloudRoutingSegment) routeSegments.SelectedItem).Shape));

                // Zoom to the selected feature and zoom out to an appropriate level
                mapView.CurrentExtent = ((CloudRoutingSegment) routeSegments.SelectedItem).Shape.GetBoundingBox();
                var standardZoomLevelSet = new ZoomLevelSet();
                mapView.ZoomToScale(standardZoomLevelSet.ZoomLevel15.Scale);
                mapView.Refresh();
            }
        }
    }
}