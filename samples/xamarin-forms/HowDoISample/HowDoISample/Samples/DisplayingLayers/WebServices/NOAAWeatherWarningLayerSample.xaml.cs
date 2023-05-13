using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a NOAA Weather Warning Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NOAAWeatherWarningLayerSample : ContentPage
    {
        public NOAAWeatherWarningLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the NOAA Weather Warning layer to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create background world map with vector tile requested from ThinkGeo Cloud Service.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
               "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
               "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            var noaaWeatherWarningsOverlay = new LayerOverlay();
            mapView.Overlays.Add("Noaa Weather Warning", noaaWeatherWarningsOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            var noaaWeatherWarningsFeatureLayer = new NoaaWeatherWarningsFeatureLayer();
            noaaWeatherWarningsFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // Add the new layer to the overlay we created earlier
            noaaWeatherWarningsOverlay.Layers.Add("Noaa Weather Warning", noaaWeatherWarningsFeatureLayer);

            // Get the layers feature source and setup an event that will refresh the map when the data refreshes
            var featureSource = (NoaaWeatherWarningsFeatureSource) noaaWeatherWarningsFeatureLayer.FeatureSource;
            featureSource.WarningsUpdated -= FeatureSource_WarningsUpdated;
            featureSource.WarningsUpdated += FeatureSource_WarningsUpdated;

            featureSource.WarningsUpdating -= FeatureSource_WarningsUpdating;
            featureSource.WarningsUpdating += FeatureSource_WarningsUpdating;

            // Create the weather warnings style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
            noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherWarningsStyle());
            noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the extent to a view of the US
            mapView.CurrentExtent =
                new RectangleShape(-14927495.374917, 8262593.0543992, -6686622.84891633, 1827556.23117885);

            // Add a PopupOverlay to the map, to display feature information
            var popupOverlay = new PopupOverlay();
            mapView.Overlays.Add("Info Popup Overlay", popupOverlay);

            // Refresh the map.
            mapView.Refresh();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var weatherWarnings =
                (NoaaWeatherWarningsFeatureSource) mapView.FindFeatureLayer("Noaa Weather Warning").FeatureSource;
            weatherWarnings.WarningsUpdated -= FeatureSource_WarningsUpdated;
            weatherWarnings.WarningsUpdating -= FeatureSource_WarningsUpdating;
        }

        private void FeatureSource_WarningsUpdating(object sender,
            WarningsUpdatingNoaaWeatherWarningsFeatureSourceEventArgs e)
        {
            mapView.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                loadingIndicator.IsRunning = true;
                loadingLayout.IsVisible = true;
            });
        }

        private void FeatureSource_WarningsUpdated(object sender,
            WarningsUpdatedNoaaWeatherWarningsFeatureSourceEventArgs e)
        {
            // This event fires when the the feature source has new data.  We need to make sure we refresh the map
            // on the UI threat so we use the Invoke method on the map using the delegate we created at the top.
            mapView.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                mapView.Refresh(new[] {mapView.Overlays["Noaa Weather Warning"]});
                loadingIndicator.IsRunning = false;
                loadingLayout.IsVisible = false;
            });
        }

        private void mapView_MapClick(object sender, TouchMapViewEventArgs e)
        {
            // Get the selected feature based on the tapped location
            var selectedFeatures = GetFeaturesFromLocation(e.PointInWorldCoordinate);

            // If a feature was selected, get the data from it and display it
            if (selectedFeatures != null) DisplayFeatureInfo(selectedFeatures);
        }

        private Collection<Feature> GetFeaturesFromLocation(PointShape location)
        {
            // Get the parks layer from the MapView
            var weatherWarnings = mapView.FindFeatureLayer("Noaa Weather Warning");

            // Find the feature that was tapped on by querying the layer for features containing the tapped coordinates
            var selectedFeatures =
                weatherWarnings.QueryTools.GetFeaturesContaining(location, ReturningColumnsType.AllColumns);

            return selectedFeatures;
        }

        private void DisplayFeatureInfo(Collection<Feature> features)
        {
            if (features.Count > 0)
            {
                var weatherWarningString = new StringBuilder();

                // Each column in a feature is a data attribute
                // Add all attribute pairs to the info string
                foreach (var feature in features) weatherWarningString.AppendLine($"{feature.ColumnValues["TITLE"]}");

                // Create a new popup with the park info string
                var popupOverlay = (PopupOverlay) mapView.Overlays["Info Popup Overlay"];
                var popup = new Popup();
                popup.Position = features[0].GetShape().GetCenterPoint();
                popup.Text = weatherWarningString.ToString();

                // Clear the popup overlay and add the new popup to it
                popupOverlay.Popups.Clear();
                popupOverlay.Popups.Add(popup);

                // Refresh the overlay to redraw the popups
                popupOverlay.Refresh();
            }
        }
    }
}