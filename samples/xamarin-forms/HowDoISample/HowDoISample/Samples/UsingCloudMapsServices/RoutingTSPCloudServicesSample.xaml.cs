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
    ///     Learn how to use the RoutingCloudClient to find an optimized route through a set of waypoints with the ThinkGeo
    ///     Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutingTSPCloudServicesSample : ContentPage
    {
        private RoutingCloudClient routingCloudClient;
        private Collection<PointShape> routingWaypoints;

        public RoutingTSPCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay, as well as a feature layer to display the route
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a new feature layer to display the route
            var routingLayer = new InMemoryFeatureLayer();

            // Add point, line, and text styles to display the waypoints, route, and labels for the route
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Star, 24,
                GeoBrushes.MediumPurple, GeoPens.Purple);
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 3, false);
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateMaskTextStyle("SequenceNumber",
                new GeoFont("Verdana", 20), GeoBrushes.White,
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.Black), GeoColors.Black, 2), 0, 0);
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.TextPlacement = TextPlacement.Upper;
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.YOffsetInPixel = -1;
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.OverlappingRule =
                LabelOverlappingRule.AllowOverlapping;
            routingLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a feature layer to highlight selected features
            var highlightLayer = new InMemoryFeatureLayer();

            // Add styles to display the highlighted route features
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.BrightYellow, 6, GeoColors.Black, 2, false);
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to an overlay, and add the overlay to the mapview
            var routingOverlay = new LayerOverlay();
            routingOverlay.Layers.Add("Routing Layer", routingLayer);
            routingOverlay.Layers.Add("Highlight Layer", highlightLayer);
            mapView.Overlays.Add("Routing Overlay", routingOverlay);

            // Create a set of preset waypoints to route through
            routingWaypoints = new Collection<PointShape>
            {
                new PointShape(-10776986.85, 3908680.24),
                new PointShape(-10776836.12, 3912348.04),
                new PointShape(-10778917.01, 3909965.17),
                new PointShape(-10779631.80, 3915721.82),
                new PointShape(-10774900.61, 3912552.36)
            };

            // Add the waypoints to the map to be displayed
            foreach (var routingWaypoint in routingWaypoints)
                routingLayer.InternalFeatures.Add(new Feature(routingWaypoint));

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent =
                new RectangleShape(-10798419.605087, 3934270.12359632, -10759021.6785336, 3896039.57306867);

            // Initialize the RoutingCloudClient with our ThinkGeo Cloud Client credentials
            routingCloudClient = new RoutingCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            // Run the routing request
            RouteWaypoints();

            await mapView.RefreshAsync();
        }


        /// <summary>
        ///     Make a request to the ThinkGeo Cloud for an optimized route
        /// </summary>
        private async Task<CloudRoutingOptimizationResult> GetOptimizedRoute()
        {
            // Set up options for the TSP routing request
            var options = new CloudRoutingOptimizationOptions();

            // Enable turn-by-turn so we get turn by turn instructions
            options.TurnByTurn = true;

            // A specific starting and ending location can be specified, if desired. 
            // For this example, any point can be used as the start and end
            options.Destination = CloudRoutingTspFixDestinationCoordinate.Any;
            options.Source = CloudRoutingTspFixSourceCoordinate.Any;

            // The 'roundtrip' option specifies whether the route returns to the starting point or not
            options.Roundtrip = true;

            // Send the TSP routing request
            var optimizedRoutingResult =
                await routingCloudClient.GetOptimizedRouteAsync(routingWaypoints, 3857, options);

            return optimizedRoutingResult;
        }

        /// <summary>
        ///     Draw the result of an optimized routing request on the map
        /// </summary>
        private async Task DrawOptimizedRoute(CloudRoutingOptimizationResult optimizedRoutingResult)
        {
            // Get the routing feature layer from the MapView
            var routingLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Routing Layer");

            // Clear the previous features from the routing layer
            routingLayer.InternalFeatures.Clear();

            // Create a collection to hold the route segments. These include information like distance, duration, warnings, and instructions for turn-by-turn routing
            var routeSegments = new List<CloudRoutingSegment>();

            //// CloudRoutingOptimizationResult.TspResult.VisitSequences is an ordered array of integers
            //// Each integer corresponds to the index of the corresponding waypoint in the original set of waypoints passed into the query
            //// For example, if the second element in 'VisitSequences' is '3', the second stop on the route is originalWaypointArray[3]

            var index = 0;
            // Add the route visit points and route segments to the map
            foreach (var waypointIndex in optimizedRoutingResult.TspResult.VisitSequences)
            {
                var columnValues = new Dictionary<string, string>();

                // Get the order of the stops and label the point
                // '0' represents the start/end point of the route for a round trip route, so we change the label to indicate that for readability
                columnValues.Add("SequenceNumber",
                    index == 0 || index == optimizedRoutingResult.TspResult.VisitSequences.Count - 1
                        ? "Start/End Point"
                        : "Stop " + index);
                var waypoint = routingWaypoints[waypointIndex];

                // Add the point to the map
                routingLayer.InternalFeatures.Add(new Feature(waypoint, columnValues));

                // Increment the index for labeling purposes
                index++;
            }

            foreach (var route in optimizedRoutingResult.TspResult.Routes)
            {
                routingLayer.InternalFeatures.Add(new Feature(route.Shape));
                routeSegments.AddRange(route.Segments);
            }

            //// Set the data source for the list box to the route segments
            lsbRouteSegments.ItemsSource = routeSegments;

            //// Set the map extent to the newly displayed route
            routingLayer.Open();
            mapView.CurrentExtent = AreaBaseShape.ScaleUp(routingLayer.GetBoundingBox(), 20).GetBoundingBox();
            routingLayer.Close();
            await mapView.RefreshAsync();
        }


        /// <summary>
        ///     Perform routing using the RoutingCloudClient through a preset set of waypoints
        /// </summary>
        private async void RouteWaypoints()
        {
            // Show a loading graphic to let users know the request is running
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            // Run the optimized routing query
            var optimizedRoutingResult = await GetOptimizedRoute();

            // Hide the loading graphic
            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;

            // Handle an exception returned from the service
            if (optimizedRoutingResult.Exception != null)
            {
                await DisplayAlert("Alert", optimizedRoutingResult.Exception.Message, "OK");
                return;
            }

            // Draw the result on the map
            await DrawOptimizedRoute(optimizedRoutingResult);
        }

        /// <summary>
        ///     When a route segment is selected in the UI, center the map on it
        /// </summary>
        private async void lsbRouteSegments_SelectionChanged(object sender,
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
                //if (mapView.CurrentScale < standardZoomLevelSet.ZoomLevel15.Scale)
                //{
                //    mapView.ZoomToScale(standardZoomLevelSet.ZoomLevel15.Scale);
                //}
                await mapView.RefreshAsync();
            }
        }
    }
}