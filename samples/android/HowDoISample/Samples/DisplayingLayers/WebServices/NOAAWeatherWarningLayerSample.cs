using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display NOAA weather warnings to your map.
    /// </summary>
    public class NOAAWeatherWarningLayerSample : SampleFragment
    {
        private ImageView loadingImage;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay noaaWeatherWarningsOverlay = new LayerOverlay();
            mapView.Overlays.Add("Noaa Weather Warning", noaaWeatherWarningsOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            NoaaWeatherWarningsFeatureLayer noaaWeatherWarningsFeatureLayer = new NoaaWeatherWarningsFeatureLayer();
            noaaWeatherWarningsFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // Add the new layer to the overlay we created earlier
            noaaWeatherWarningsOverlay.Layers.Add(noaaWeatherWarningsFeatureLayer);

            // Get the layers feature source and setup an event that will refresh the map when the data refreshes
            var featureSource = (NoaaWeatherWarningsFeatureSource)noaaWeatherWarningsFeatureLayer.FeatureSource;
            featureSource.WarningsUpdated -= FeatureSource_WarningsUpdated;
            featureSource.WarningsUpdated += FeatureSource_WarningsUpdated;

            featureSource.WarningsUpdating -= FeatureSource_WarningsUpdating;
            featureSource.WarningsUpdating += FeatureSource_WarningsUpdating;

            // Create the weather warnings style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
            noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherWarningsStyle());
            noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the extent to a view of the US
            mapView.CurrentExtent = new RectangleShape(-14927495.374917, 8262593.0543992, -6686622.84891633, 1827556.23117885);

            // Refresh the map.
            mapView.Refresh();
        }

        private void FeatureSource_WarningsUpdating(object sender, WarningsUpdatingNoaaWeatherWarningsFeatureSourceEventArgs e)
        {
            loadingImage.Visibility = ViewStates.Visible;
        }

        private void FeatureSource_WarningsUpdated(object sender, WarningsUpdatedNoaaWeatherWarningsFeatureSourceEventArgs e)
        {
            // This event fires when the the feature source has new data.  We need to make sure we refresh the map
            // on the UI threat so we use the Invoke method on the map using the delegate we created at the top.
            loadingImage.Visibility = ViewStates.Visible;
            mapView.Overlays["Noaa Weather Warning"].Refresh();

        }
    }
}