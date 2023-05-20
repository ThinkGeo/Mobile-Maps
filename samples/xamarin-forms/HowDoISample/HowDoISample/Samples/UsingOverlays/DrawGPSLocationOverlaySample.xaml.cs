using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     This samples shows how to refresh points on the map based on some outside event
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawGPSLocationOverlaySample : ContentPage
    {
        private bool isFeedCanceled;
        private bool isFeedPaused;

        private Task updateDataFeed;

        public DrawGPSLocationOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a marker overlay to show where the vehicle is
            var markerOverlay = new SimpleMarkerOverlay();

            // Create the marker of the vehicle
            var marker = new Marker
            {
                Position = new PointShape(-10778817.49746323, 3912420.8997628987),
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/vehicle-location.png"),
                YOffset = -33
            };

            // Add the marker to the overlay
            markerOverlay.Markers.Add("vehicle", marker);

            // Add the vehicle overlay to the maps
            mapView.Overlays.Add("Vehicle Overlay", markerOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10779430.188014803, 3912668.1732483786, -10778438.895309737,
                3911814.2283277493);

            //  Here we call the method below to start the background data feed
            await StartDataFeedAsync();

            // Refresh the map
            await mapView.RefreshAsync();
        }

        protected override void OnDisappearing()
        {
            // Set the data feed token to cancel and then wait for it to process the cancel. 
            // This is important otherwise the feed will continue to run even if we navigate away
            isFeedCanceled = true;

            updateDataFeed.Wait();

            base.OnDisappearing();
        }

        private async Task StartDataFeedAsync()
        {
            // Create a task that simulated the external data feed and run it until we cancel it

            //updateDataFeed = Task.Run(() =>
            //{
                // Create a queue and load it up with coordinated from the CSV file
                var vehicleLocationQueue = new Queue<Feature>();

                var locations = File.ReadAllLines(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Data/Csv/vehicle-route.csv"));

                foreach (var location in locations)
                    vehicleLocationQueue.Enqueue(new Feature(double.Parse(location.Split(',')[0]),
                        double.Parse(location.Split(',')[1])));

                // Keep looping as long as it's not canceled
                while (isFeedCanceled == false)
                {
                    // If the feed is not paused then update the vehicle location
                    if (!isFeedPaused)
                    {
                        Debug.WriteLine(
                            $"Processing Vehicle Location Data Feed: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        // Get the latest point from the queue and then re-add it so the points
                        // will loop forever
                        var currentFeature = vehicleLocationQueue.Dequeue();
                        vehicleLocationQueue.Enqueue(currentFeature);

                    // Call the invoke on the mapview so we pop over to the main UI thread
                    // to update the map control                        
                    //mapView.Dispatcher.BeginInvokeOnMainThread(() => { UpdateMap(currentFeature); });
                    await UpdateMapAsync(currentFeature);
                    }
                    else
                    {
                        Debug.WriteLine(
                            $"Paused Vehicle Location Data Feed: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                    }

                    // Delay the task for a few seconds before we update the feed
                    Debug.WriteLine(
                        $"Vehicle Location Data Feed: Paused {isFeedPaused.ToString()} {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                    await Task.Delay(2000);
                }
            //});
        }

        private async Task UpdateMapAsync(Feature currentFeature)
        {
            // We need to first find our vehicle overlay
            var vehicleOverlay = (SimpleMarkerOverlay) mapView.Overlays["Vehicle Overlay"];

            // Update the markers position
            vehicleOverlay.Markers["vehicle"].Position = (PointShape) currentFeature.GetShape();

            // If we have the center on vehicle check box checked then we center the map on the new location
            if (centerOnVehicle.IsChecked)
            {
                await mapView.CenterAtAsync(currentFeature);
                await mapView.Overlays["Background Maps"].RefreshAsync();
            }
            else
            {
                // Refresh the vehicle overlay
                await mapView.Overlays["Vehicle Overlay"].RefreshAsync();
            }
        }

        /// <summary>
        ///     Pause the data feed
        /// </summary>
        private void PauseDataFeed_Checked(object sender, EventArgs e)
        {
            isFeedPaused = pauseDataFeed.IsChecked;
        }
    }
}