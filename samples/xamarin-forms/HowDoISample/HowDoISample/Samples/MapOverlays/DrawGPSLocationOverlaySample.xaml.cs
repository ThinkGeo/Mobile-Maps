using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     This samples shows how to refresh points on the map based on some outside event
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawGpsLocationOverlaySample
    {
        private Timer gpsTimer;
        private Queue<Feature> vehicleLocationQueue;

        public DrawGpsLocationOverlaySample()
        {
            InitializeComponent();
        }

        private bool centerOnVehicle;

        // whether to center the map on the vehicle
        public bool CenterOnVehicle
        {
            get => centerOnVehicle;
            set
            {
                centerOnVehicle = value;
                OnPropertyChanged();
            }
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
            mapView.Overlays.Add("Background Maps", backgroundOverlay);

            // Create a marker overlay to show where the vehicle is
            var markerOverlay = new SimpleMarkerOverlay();

            // Create the marker of the vehicle
            var vehicleMarker = new Marker
            {
                Position = new PointShape(-10778817.49746323, 3912420.8997628987),
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/vehicle-location.png"),
                YOffset = -33
            };

            // Add the marker to the overlay
            markerOverlay.Markers.Add("vehicle", vehicleMarker);

            // Add the vehicle overlay to the maps
            mapView.Overlays.Add("Vehicle Overlay", markerOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10779430, 3912668, -10778438, 3911814);

            // center the map at the vehicle
            CenterOnVehicle = true;

            // Refresh the map
            await mapView.RefreshAsync();

            await InitGpsData();

            gpsTimer = new Timer(1000);
            gpsTimer.Elapsed += GpsTimerElapsed;
            gpsTimer.Start();
        }

        private async Task InitGpsData()
        {
            vehicleLocationQueue = new Queue<Feature>();

            var locations = await File.ReadAllLinesAsync(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Csv/vehicle-route.csv"));

            foreach (var location in locations)
            {
                var posItems = location.Split(',');
                vehicleLocationQueue.Enqueue(new Feature(double.Parse(posItems[0]), double.Parse(posItems[1])));
            }
        }

        private void GpsTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Get the latest point from the queue and re-add it to the loop
            var currentFeature = vehicleLocationQueue.Dequeue();
            vehicleLocationQueue.Enqueue(currentFeature);

            // the GpsTimerElapsed method was not invoked in the UI thread and that's why we need this BeginInvokeOnMainThread 
            Device.BeginInvokeOnMainThread(() =>
            {
                var _ = UpdateMapAsync(currentFeature);
            });
        }

        private async Task UpdateMapAsync(Feature currentFeature)
        {
            // We need to first find our vehicle overlay
            var vehicleOverlay = (SimpleMarkerOverlay)mapView.Overlays["Vehicle Overlay"];

            // Update the markers position
            vehicleOverlay.Markers["vehicle"].Position = (PointShape)currentFeature.GetShape();

            // If we have the center on vehicle check box checked then we center the map on the new location
            if (CenterOnVehicle)
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            gpsTimer.Stop();
        }
    }
}