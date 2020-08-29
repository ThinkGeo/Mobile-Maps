using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NOAAWeatherStationLayerSample : ContentPage
    {
        public NOAAWeatherStationLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;            

            // Create background world map with vector tile requested from ThinkGeo Cloud Service.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay weatherOverlay = new LayerOverlay();
            mapView.Overlays.Add("Weather", weatherOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            NoaaWeatherStationFeatureLayer nOAAWeatherStationLayer = new NoaaWeatherStationFeatureLayer();
            nOAAWeatherStationLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // Add the new layer to the overlay we created earlier
            weatherOverlay.Layers.Add("Noaa Weather Stations", nOAAWeatherStationLayer);

            // Get the layers feature source and setup an event that will refresh the map when the data refreshes
            var featureSource = (NoaaWeatherStationFeatureSource)nOAAWeatherStationLayer.FeatureSource;
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;
            featureSource.StationsUpdated -= FeatureSource_StationsUpdated;
            featureSource.StationsUpdated += FeatureSource_StationsUpdated;

            // Create the weather stations style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
            nOAAWeatherStationLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherStationStyle());
            nOAAWeatherStationLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the extent to a view of the US
            mapView.CurrentExtent = new RectangleShape(-14927495.374917, 8262593.0543992, -6686622.84891633, 1827556.23117885);

            // Refresh the map.
            mapView.Refresh();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var weatherStations = (NoaaWeatherStationFeatureSource)mapView.FindFeatureLayer("Noaa Weather Stations").FeatureSource;
            weatherStations.StationsUpdated -= FeatureSource_StationsUpdated;
        }

        private void FeatureSource_StationsUpdated(object sender, StationsUpdatedNoaaWeatherStationFeatureSourceEventArgs e)
        {
            // This event fires when the the feature source has new data.  We need to make sure we refresh the map
            // on the UI threat so we use the Invoke method on the map using the delegate we created at the top.
            mapView.Dispatcher.BeginInvokeOnMainThread(UpdateWeatherStations);
        }

        private void UpdateWeatherStations()
        {
            // Here we fresh the map based on the delegate that fires when the feature source has new data.
            var weatherOverlay = mapView.Overlays["Weather"];
            weatherOverlay.Refresh();

            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;
        }
    }
}
