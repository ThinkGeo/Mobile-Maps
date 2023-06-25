using System;
using System.Collections.ObjectModel;
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
    public partial class UpdateVehicleLocationSample
    {
        private Timer gpsTimer;
        private Collection<Vertex> gpsPoints;
        private int currentPointIndex = 0;
        private InMemoryFeatureLayer routesLayer;
        private InMemoryFeatureLayer visitedRoutesLayer;
        private int previousVertex;
        private Marker vehicleMarker;

        public UpdateVehicleLocationSample()
        {
            InitializeComponent();
        }

        public double MapRotation
        {
            get => mapView.MapRotation;
            set
            {
                mapView.MapRotation = value;
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

            await InitGpsData();

            // init the timer feeding GPS points
            gpsTimer = new Timer(1000);
            gpsTimer.Elapsed += GpsTimerElapsed;
            gpsTimer.Start();
        }

        private async Task InitGpsData()
        {
            gpsPoints = new Collection<Vertex>();

            var locations = await File.ReadAllLinesAsync(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Csv/vehicle-route.csv"));

            var lineShape = new LineShape();
            foreach (var location in locations)
            {
                var posItems = location.Split(',');
                var vertex = new Vertex(double.Parse(posItems[0]), double.Parse(posItems[1]));
                gpsPoints.Add(vertex);
                lineShape.Vertices.Add(vertex);
            }
            previousVertex = 0;

            // Create the marker of the vehicle
            vehicleMarker = new Marker
            {
                Position = new PointShape(lineShape.Vertices[0]),
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/vehicle-location.png"),
                Width = 24,
                Height = 24
            };

            // Create a marker overlay to show where the vehicle is
            var markerOverlay = new SimpleMarkerOverlay();
            // Add the marker to the overlay than add to the map. 
            markerOverlay.Markers.Add(vehicleMarker);
            mapView.Overlays.Add(markerOverlay);

            // create the layers for the routes.
            routesLayer = new InMemoryFeatureLayer();
            routesLayer.InternalFeatures.Add(new Feature(lineShape));
            routesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.SkyBlue, 6, true);
            routesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            visitedRoutesLayer = new InMemoryFeatureLayer();
            visitedRoutesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkSeaGreen, 6, true);
            visitedRoutesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add(routesLayer);
            layerOverlay.Layers.Add(visitedRoutesLayer);
            mapView.Overlays.Add(layerOverlay);

            mapView.CenterPoint = new PointShape(gpsPoints[0]);
            mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel19.Scale;
            await mapView.RefreshAsync();
        }

        private void GpsTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (currentPointIndex >= gpsPoints.Count)
            {
                gpsTimer.Stop();
                return;
            }

            // the GpsTimerElapsed method was not invoked in the UI thread and that's why we need this BeginInvokeOnMainThread 
            Device.BeginInvokeOnMainThread(() =>
            {
                var _ = UpdateMapAsync();
                currentPointIndex++;
            });
        }

        private async Task UpdateMapAsync()
        {
            var currentLocation = gpsPoints[currentPointIndex];
            var angle = GetRotationAngle(currentPointIndex, gpsPoints);

            UpdateVisitedRoutes();

            // Update the markers position
            MapRotation = angle;
            vehicleMarker.Position = new PointShape(currentLocation);
            await mapView.ZoomToAsync(vehicleMarker.Position, mapView.MapScale);
        }

        private void UpdateVisitedRoutes()
        {
            if (currentPointIndex == 0 || previousVertex >= currentPointIndex) return;
            var lineShape = new LineShape();
            for (var i = previousVertex; i <= currentPointIndex; i++)
            {
                lineShape.Vertices.Add(gpsPoints[i]);
            }

            var lineFeature = new Feature(lineShape);
            visitedRoutesLayer.InternalFeatures.Add(lineFeature);
            previousVertex = currentPointIndex;
        }

        private static double GetRotationAngle(int currentIndex, Collection<Vertex> gpsPoints)
        {
            Vertex currentLocation;
            Vertex nextLocation;

            if (currentIndex < gpsPoints.Count - 1)
            {
                currentLocation = gpsPoints[currentIndex];
                nextLocation = gpsPoints[currentIndex + 1];
            }
            else
            {
                currentLocation = gpsPoints[currentIndex - 1];
                nextLocation = gpsPoints[currentIndex];
            }

            double angle;
            if (nextLocation.X - currentLocation.X != 0)
            {
                var dx = (nextLocation.X - currentLocation.X);
                var dy = (nextLocation.Y - currentLocation.Y);

                angle = Math.Atan2(dx, dy) / Math.PI * 180; // get the angle in degrees from 
                angle = -angle;
            }
            else
            {
                angle = nextLocation.Y - currentLocation.Y >= 0 ? 0 : 180;
            }
            return angle;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            gpsTimer.Stop();
        }
    }
}