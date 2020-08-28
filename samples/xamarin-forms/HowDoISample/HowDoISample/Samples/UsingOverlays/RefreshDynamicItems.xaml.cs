using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefreshDynamicItems : ContentPage
    {
        bool cancelFeed;
        bool pauseFeed;
        Task dataFeed;

        public RefreshDynamicItems()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Setup the overlay that we will refresh often
            LayerOverlay vehicleOverlay = new LayerOverlay();

            // This in memory layer will hold the active point, we will be adding and removing from it frequently
            InMemoryFeatureLayer vehicleLayer = new InMemoryFeatureLayer();

            // Set the points image to an car icon and then apply it to all zoomlevels            
            PointStyle vehiclePointStyle = new PointStyle(new GeoImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/vehicle-location.png")));
            vehiclePointStyle.YOffsetInPixel = -24;

            vehicleLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = vehiclePointStyle;
            vehicleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the in memory layer to the overlay
            vehicleOverlay.Layers.Add("Vehicle Layer", vehicleLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("Vehicle Overlay", vehicleOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10779430.188014803, 3912668.1732483786, -10778438.895309737, 3911814.2283277493);

            //  Here we call the method below to start the background data feed
            StartDataFeed();

            // Refresh the map
            mapView.Refresh();            
        }

        protected override void OnDisappearing()
        {
            // Set the data feed token to cancel and then wait for it to process the cancel. 
            cancelFeed = true;
            dataFeed.Wait();
            base.OnDisappearing();
        }

        private async void StartDataFeed()
        {
            // Create a task that runs until we set the cancelFeed variable

            dataFeed = Task.Run(() =>
           {
               // Create a queue and load it up with coordinated from the CSV file
               Queue<Feature> vehicleLocationQueue = new Queue<Feature>();

               string[] locations = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Csv/vehicle-route.csv"));

               foreach (var location in locations)
               {
                   vehicleLocationQueue.Enqueue(new Feature(double.Parse(location.Split(',')[0]), double.Parse(location.Split(',')[1])));
               }

               // Keep looping as long as it's not canceled
               while (cancelFeed == false)
               {
                   // If the feed is not paused then update the vehicle location
                   if (!pauseFeed)
                   {
                       Debug.WriteLine($"Processing Vehicle Location Data Feed: {DateTime.Now.ToString()}");
                       // Get the latest point from the queue and then re-add it so the points
                       // will loop forever
                       Feature currentFeature = vehicleLocationQueue.Dequeue();
                       vehicleLocationQueue.Enqueue(currentFeature);

                       // Call the invoke on the mapview so we pop over to the main UI thread
                       // to update the map control                        
                       mapView.Dispatcher.BeginInvokeOnMainThread(() =>
                      {
                          UpdateMap(currentFeature);
                      });
                   }
                   else
                   {
                       Debug.WriteLine($"Paused Vehicle Location Data Feed: {DateTime.Now.ToString()}");
                   }

                   // Sleep for one second
                   Debug.WriteLine($"Sleeping Vehicle Location Data Feed: {DateTime.Now.ToString()}");
                   Thread.Sleep(1000);
               }
           });
        }

        private void UpdateMap(Feature currentFeature)
        {            
            // We need to first find our vehicle overlay and in memory layer in the map
            LayerOverlay vehicleOverlay = (LayerOverlay)mapView.Overlays["Vehicle Overlay"];
            InMemoryFeatureLayer vehicleLayer = (InMemoryFeatureLayer)vehicleOverlay.Layers["Vehicle Layer"];

            // Let's clear the old location and add the new one
            vehicleLayer.InternalFeatures.Clear();
            vehicleLayer.InternalFeatures.Add(currentFeature);

            // If we have the center on vehicle check box checked then we center the map on the new location
            if (centerOnVehicle.IsChecked == true)
            {
                mapView.CenterAt(currentFeature);
            }

            // Refresh the vehicle overlay
            mapView.Overlays["Vehicle Overlay"].Refresh();
        }


        /// <summary>
        /// Pause the data feed
        /// </summary>
        private void PauseDataFeed_Checked(object sender, EventArgs e)
        {
            pauseFeed = pauseDataFeed.IsChecked;
        }

    }
}
