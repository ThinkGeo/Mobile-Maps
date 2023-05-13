using System;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the MapsQueryClient to query the WorldMaps dataset available from the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorldMapsQueryCloudServicesSample : ContentPage
    {
        private MapsQueryCloudClient mapsQueryCloudClient;

        public WorldMapsQueryCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and feature layers for the queried shapes
        /// </summary>
        protected override void OnAppearing()
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

            // Create a new feature layer to display the query shape used to perform the query
            var queryShapeFeatureLayer = new InMemoryFeatureLayer();

            // Add a point, line, and polygon style to the layer. These styles control how the the query shape will be drawn
            queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 20, GeoBrushes.Blue);
            queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.Blue);
            queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                new AreaStyle(GeoPens.Blue, new GeoSolidBrush(new GeoColor(10, GeoColors.Blue)));

            // Apply these styles on all zoom levels. This ensures our shapes will be visible on all zoom levels
            queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a new feature layer to display the shapes returned by the query.
            var queriedFeaturesLayer = new InMemoryFeatureLayer();

            // Add a point, line, and polygon style to the layer. These styles control how the returned shapes will be drawn
            queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 20, GeoBrushes.OrangeRed);
            queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.OrangeRed);
            queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.OrangeRed,
                new GeoSolidBrush(new GeoColor(10, GeoColors.OrangeRed)));
            queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the feature layers to an overlay, then add that overlay to the map
            var queriedFeaturesOverlay = new LayerOverlay();
            queriedFeaturesOverlay.Layers.Add("Queried Features Layer", queriedFeaturesLayer);
            queriedFeaturesOverlay.Layers.Add("Query Shape Layer", queryShapeFeatureLayer);
            mapView.Overlays.Add("Queried Features Overlay", queriedFeaturesOverlay);

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent =
                new RectangleShape(-10780136.4424059, 3915901.47975593, -10779033.5515421, 3914500.39988761);

            // Add an event to handle new shapes that are drawn on the map
            mapView.TrackOverlay.TrackEnded += OnShapeDrawn;

            // Initialize the MapsQueryCloudClient with our ThinkGeo Cloud credentials
            mapsQueryCloudClient = new MapsQueryCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            // Create a sample shape and add it to the query shape layer
            var sampleShape = new RectangleShape(-10779877.70, 3915441.00, -10779248.97, 3915119.63);
            queryShapeFeatureLayer.InternalFeatures.Add(new Feature(sampleShape));
            // Run the world maps query
            cboQueryLayer.SelectedItem = "Buildings";
            cboQueryType.SelectedItem = "Intersecting";

            PerformWorldMapsQuery();

            mapView.Refresh();
        }

        /// <summary>
        ///     Get features from the WorldMapsQuery service based on the UI parameters
        /// </summary>
        private async void PerformWorldMapsQuery()
        {
            // Get the feature layers from the MapView
            var queriedFeaturesOverlay = (LayerOverlay) mapView.Overlays["Queried Features Overlay"];
            var queryShapeFeatureLayer = (InMemoryFeatureLayer) queriedFeaturesOverlay.Layers["Query Shape Layer"];
            var queriedFeaturesLayer = (InMemoryFeatureLayer) queriedFeaturesOverlay.Layers["Queried Features Layer"];

            // Show an error if trying to query with no query shape
            if (queryShapeFeatureLayer.InternalFeatures.Count == 0)
            {
                await DisplayAlert("Alert", "Please draw a shape to use for the query", "OK");
                return;
            }

            // Set the MapsQuery parameters based on the drawn query shape and the UI
            var queryShape = queryShapeFeatureLayer.InternalFeatures[0].GetShape();
            var projectionInSrid = 3857;
            var queryLayer = ((string) cboQueryLayer.SelectedItem).ToLower();

            var result = new CloudMapsQueryResult();

            // Show a loading graphic to let users know the request is running
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            // Perform the world maps query
            try
            {
                switch ((string) cboQueryType.SelectedItem)
                {
                    case "Containing":
                        result = await mapsQueryCloudClient.GetFeaturesContainingAsync(queryLayer, queryShape,
                            projectionInSrid,
                            new CloudMapsQuerySpatialQueryOptions {MaxResults = Convert.ToInt32(maxResults.Text)});
                        break;
                    case "Nearest":
                        result = await mapsQueryCloudClient.GetFeaturesNearestAsync(queryLayer, queryShape,
                            projectionInSrid, Convert.ToInt32(maxResults.Text));
                        break;
                    case "Intersecting":
                        result = await mapsQueryCloudClient.GetFeaturesIntersectingAsync(queryLayer, queryShape,
                            projectionInSrid,
                            new CloudMapsQuerySpatialQueryOptions {MaxResults = Convert.ToInt32(maxResults.Text)});
                        break;
                    case "Overlapping":
                        result = await mapsQueryCloudClient.GetFeaturesOverlappingAsync(queryLayer, queryShape,
                            projectionInSrid,
                            new CloudMapsQuerySpatialQueryOptions {MaxResults = Convert.ToInt32(maxResults.Text)});
                        break;
                    case "Within":
                        result = await mapsQueryCloudClient.GetFeaturesWithinAsync(queryLayer, queryShape,
                            projectionInSrid,
                            new CloudMapsQuerySpatialQueryOptions {MaxResults = Convert.ToInt32(maxResults.Text)});
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors returned from the maps query service
                if (ex is ArgumentException)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                    mapView.Refresh();
                    return;
                }
                else
                {
                    await DisplayAlert("Alert", ex.Message, "OK");
                    mapView.Refresh();
                    return;
                }
            }
            finally
            {
                // Hide the loading graphic
                loadingIndicator.IsRunning = false;
                loadingLayout.IsVisible = false;
            }

            if (result.Features.Count > 0)
            {
                // Add any features found by the query to the map
                foreach (var feature in result.Features) queriedFeaturesLayer.InternalFeatures.Add(feature);

                // Set the map extent to the extent of the query results
                queriedFeaturesLayer.Open();
                mapView.CurrentExtent = queriedFeaturesLayer.GetBoundingBox();
                queriedFeaturesLayer.Close();
            }
            else
            {
                await DisplayAlert("Alert", "No features found in the selected area", "OK");
            }

            // Refresh and redraw the map
            mapView.Refresh();
        }

        /// <summary>
        ///     Disable drawing mode and draw the new query shape on the map when finished drawing a shape
        /// </summary>
        private void OnShapeDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            // Disable drawing mode and clear the drawing layer
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

            // Get the query shape layer from the MapView
            var queriedFeaturesOverlay = (LayerOverlay) mapView.Overlays["Queried Features Overlay"];
            var queryShapeFeatureLayer = (InMemoryFeatureLayer) queriedFeaturesOverlay.Layers["Query Shape Layer"];

            // Add the newly drawn shape, then redraw the overlay
            queryShapeFeatureLayer.InternalFeatures.Add(new Feature(e.TrackShape));
            queriedFeaturesOverlay.Refresh();

            PerformWorldMapsQuery();
        }

        /// <summary>
        ///     Set the map to 'Point Drawing Mode' when the user taps the 'Draw a New Query Point' button
        /// </summary>
        private async void DrawPoint_Click(object sender, EventArgs e)
        {
            await CollapseExpander();
            // Set the drawing mode to 'Point'
            mapView.TrackOverlay.TrackMode = TrackMode.Point;

            // Clear the old shapes from the map
            ClearQueryShapes();
        }

        /// <summary>
        ///     Set the map to 'Line Drawing Mode' when the user taps the 'Draw a New Query Line' button
        /// </summary>
        private async void DrawLine_Click(object sender, EventArgs e)
        {
            await CollapseExpander();
            // Set the drawing mode to 'Line'
            mapView.TrackOverlay.TrackMode = TrackMode.Line;

            // Clear the old shapes from the map
            ClearQueryShapes();
        }

        /// <summary>
        ///     Set the map to 'Polygon Drawing Mode' when the user taps the 'Draw a New Query Polygon' button
        /// </summary>
        private async void DrawPolygon_Click(object sender, EventArgs e)
        {
            await CollapseExpander();
            // Set the drawing mode to 'Polygon'
            mapView.TrackOverlay.TrackMode = TrackMode.Polygon;

            // Clear the old shapes from the map
            ClearQueryShapes();
        }

        /// <summary>
        ///     Clear the query shapes from the map
        /// </summary>
        private void ClearQueryShapes()
        {
            // Get the query shape layer from the MapView
            var queriedFeaturesOverlay = (LayerOverlay) mapView.Overlays["Queried Features Overlay"];
            var queryShapeFeatureLayer = (InMemoryFeatureLayer) queriedFeaturesOverlay.Layers["Query Shape Layer"];
            var queriedFeaturesLayer = (InMemoryFeatureLayer) queriedFeaturesOverlay.Layers["Queried Features Layer"];

            // Clear the old query result and query shape from the map
            queriedFeaturesLayer.InternalFeatures.Clear();
            queryShapeFeatureLayer.InternalFeatures.Clear();
            queriedFeaturesOverlay.Refresh();
        }

        private async Task CollapseExpander()
        {
            controlsExpander.IsExpanded = false;
            await Task.Delay((int) controlsExpander.CollapseAnimationLength);
        }
    }
}