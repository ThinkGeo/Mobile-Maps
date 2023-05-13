using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the ElevationCloudClient class to get elevation data from the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ElevationCloudServicesSample : ContentPage
    {
        private ElevationCloudClient elevationCloudClient;

        public ElevationCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layers for the shape to be queried and the
        ///     returned elevation points
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

            // Create a new InMemoryFeatureLayer to hold the shape drawn for the elevation query
            var drawnShapeLayer = new InMemoryFeatureLayer();

            // Create Point, Line, and Polygon styles to display the drawn shape, and apply them across all zoom levels
            drawnShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 20, GeoBrushes.Blue);
            drawnShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.Blue);
            drawnShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.Blue,
                new GeoSolidBrush(new GeoColor(10, GeoColors.Blue)));
            drawnShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a new InMemoryFeatureLayer to display the elevation points returned from the query
            var elevationPointsLayer = new InMemoryFeatureLayer();

            // Create a point style for the elevation points
            elevationPointsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 20, GeoBrushes.Blue);
            elevationPointsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the feature layers to an overlay, and add the overlay to the map
            var elevationFeaturesOverlay = new LayerOverlay();
            elevationFeaturesOverlay.Layers.Add("Elevation Points Layer", elevationPointsLayer);
            elevationFeaturesOverlay.Layers.Add("Drawn Shape Layer", drawnShapeLayer);
            mapView.Overlays.Add("Elevation Features Overlay", elevationFeaturesOverlay);

            // Add an event to trigger the elevation query when a new shape is drawn
            mapView.TrackOverlay.TrackEnded += OnShapeDrawn;

            // Initialize the ElevationCloudClient with our ThinkGeo Cloud credentials
            elevationCloudClient = new ElevationCloudClient("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");

            // Create a sample line and get elevation along that line
            var sampleShape = new LineShape(
                "LINESTRING(-10776298.0601626 3912306.29684573,-10776496.3187036 3912399.45447343,-10776675.4679876 3912478.28015841,-10776890.4471285 3912516.49867234,-10777189.0292686 3912509.33270098,-10777329.9600387 3912442.4503016,-10777664.3720356 3912174.92070409)");

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent = sampleShape.GetBoundingBox();

            await PerformElevationQuery(sampleShape);

            mapView.Refresh();
        }

        /// <summary>
        ///     Get elevation data using the ElevationCloudClient and update the UI
        /// </summary>
        private async Task PerformElevationQuery(BaseShape queryShape)
        {
            // Get feature layers from the MapView
            var elevationPointsOverlay = (LayerOverlay) mapView.Overlays["Elevation Features Overlay"];
            var drawnShapesLayer = (InMemoryFeatureLayer) elevationPointsOverlay.Layers["Drawn Shape Layer"];
            var elevationPointsLayer = (InMemoryFeatureLayer) elevationPointsOverlay.Layers["Elevation Points Layer"];

            // Clear the existing shapes from the map
            elevationPointsLayer.Open();
            elevationPointsLayer.Clear();
            elevationPointsLayer.Close();
            drawnShapesLayer.Open();
            drawnShapesLayer.Clear();
            drawnShapesLayer.Close();

            // Add the drawn shape to the map
            drawnShapesLayer.InternalFeatures.Add(new Feature(queryShape));

            // Set options from the UI and run the query using the ElevationCloudClient
            var elevationPoints = new Collection<CloudElevationPointResult>();
            var projectionInSrid = 3857;

            // Show a loading graphic to let users know the request is running
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            // The point interval distance determines how many elevation points are retrieved for line and area queries
            var pointIntervalDistance = (int) intervalDistance.Value;
            switch (queryShape.GetWellKnownType())
            {
                case WellKnownType.Point:
                    var drawnPoint = (PointShape) queryShape;
                    var elevation =
                        await elevationCloudClient.GetElevationOfPointAsync(drawnPoint.X, drawnPoint.Y,
                            projectionInSrid);

                    // The API for getting the elevation of a single point returns a double, so we manually create a CloudElevationPointResult to use as a data source for the Elevations list
                    elevationPoints.Add(new CloudElevationPointResult(elevation, drawnPoint));

                    // Update the UI with the average, highest, and lowest elevations
                    txtAverageElevation.Text = $"Average Elevation: {elevation:0.00} feet";
                    txtHighestElevation.Text = $"Highest Elevation: {elevation:0.00} feet";
                    txtLowestElevation.Text = $"Lowest Elevation: {elevation:0.00} feet";
                    break;
                case WellKnownType.Line:
                    var drawnLine = (LineShape) queryShape;
                    var result = await elevationCloudClient.GetElevationOfLineAsync(drawnLine, projectionInSrid,
                        pointIntervalDistance, DistanceUnit.Meter, DistanceUnit.Feet);
                    elevationPoints = result.ElevationPoints;

                    // Update the UI with the average, highest, and lowest elevations
                    txtAverageElevation.Text = $"Average Elevation: {result.AverageElevation:0.00} feet";
                    txtHighestElevation.Text = $"Highest Elevation: {result.HighestElevationPoint.Elevation:0.00} feet";
                    txtLowestElevation.Text = $"Lowest Elevation: {result.LowestElevationPoint.Elevation:0.00} feet";
                    break;
                case WellKnownType.Polygon:
                    var drawnPolygon = (PolygonShape) queryShape;
                    result = await elevationCloudClient.GetElevationOfAreaAsync(drawnPolygon, projectionInSrid,
                        pointIntervalDistance, DistanceUnit.Meter);
                    elevationPoints = result.ElevationPoints;

                    // Update the UI with the average, highest, and lowest elevations
                    txtAverageElevation.Text = $"Average Elevation: {result.AverageElevation:0.00} feet";
                    txtHighestElevation.Text = $"Highest Elevation: {result.HighestElevationPoint.Elevation:0.00} feet";
                    txtLowestElevation.Text = $"Lowest Elevation: {result.LowestElevationPoint.Elevation:0.00} feet";
                    break;
            }

            // Add the elevation result points to the map and list box
            foreach (var elevationPoint in elevationPoints)
                elevationPointsLayer.InternalFeatures.Add(new Feature(elevationPoint.Point));
            lsbElevations.ItemsSource = elevationPoints;

            // Hide the loading graphic
            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;

            // Set the map extent to the elevation query feature
            drawnShapesLayer.Open();
            mapView.CenterAt(drawnShapesLayer.GetBoundingBox().GetCenterPoint());
            drawnShapesLayer.Close();
            mapView.Refresh();
        }

        /// <summary>
        ///     Disable map drawing after a shape is drawn
        /// </summary>
        private async void OnShapeDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            // Disable drawing mode and clear the drawing layer
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

            // Validate shape size to avoid queries that are too large
            // Maximum length of a line is 10km
            // Maximum area of a polygon is 10km^2
            if (e.TrackShape.GetWellKnownType() == WellKnownType.Polygon)
            {
                if (((PolygonShape) e.TrackShape).GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers) > 5)
                {
                    await DisplayAlert("Error", "Please draw a smaller polygon (limit: 5km^2)", "OK");
                    return;
                }
            }
            else if (e.TrackShape.GetWellKnownType() == WellKnownType.Line)
            {
                if (((LineShape) e.TrackShape).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer) > 5)
                {
                    await DisplayAlert("Alert", "Please draw a shorter line (limit: 5km)", "OK");
                    return;
                }
            }

            // Get elevation data for the drawn shape and update the UI
            await PerformElevationQuery(e.TrackShape);
        }

        /// <summary>
        ///     Center the map on a point when it's selected in the UI
        /// </summary>
        private async void lsbElevations_SelectionChanged(object sender,
            SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            await CollapseExpander();

            if (lsbElevations.SelectedItem != null)
            {
                // Set the map extent to the selected point
                var elevationPoint = (CloudElevationPointResult) lsbElevations.SelectedItem;
                mapView.CenterAt(elevationPoint.Point);
                mapView.Refresh();
            }
        }

        /// <summary>
        ///     Set the map to 'Point Drawing Mode' when the user taps the 'Draw a New Point' button
        /// </summary>
        private async void DrawPoint_Click(object sender, EventArgs e)
        {
            await CollapseExpander();

            // Set the drawing mode to 'Point'
            mapView.TrackOverlay.TrackMode = TrackMode.Point;
        }

        /// <summary>
        ///     Set the map to 'Line Drawing Mode' when the user taps the 'Draw a New Line' button
        /// </summary>
        private async void DrawLine_Click(object sender, EventArgs e)
        {
            await CollapseExpander();

            // Set the drawing mode to 'Line'
            mapView.TrackOverlay.TrackMode = TrackMode.Line;
        }

        /// <summary>
        ///     Set the map to 'Polygon Drawing Mode' when the user taps the 'Draw a New Polygon' button
        /// </summary>
        private async void DrawPolygon_Click(object sender, EventArgs e)
        {
            await CollapseExpander();

            // Set the drawing mode to 'Polygon'
            mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        }

        private async Task CollapseExpander()
        {
            controlsExpander.IsExpanded = false;
            await Task.Delay((int) controlsExpander.CollapseAnimationLength);
        }
    }
}