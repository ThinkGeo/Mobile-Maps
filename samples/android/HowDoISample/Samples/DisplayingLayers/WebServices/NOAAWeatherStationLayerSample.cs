using System;
using System.Collections.ObjectModel;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display NOAA weather stations current readings to your map.
    /// </summary>
    public class NOAAWeatherStationLayerSample : SampleFragment
    {
        //  We use this delegate for refresing the map from another thread
        public delegate void RefreshWeatherStations();
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
            LayerOverlay weatherOverlay = new LayerOverlay();
            mapView.Overlays.Add("Weather", weatherOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            NoaaWeatherStationFeatureLayer nOAAWeatherStationLayer = new NoaaWeatherStationFeatureLayer();
            nOAAWeatherStationLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // Add the new layer to the overlay we created earlier
            weatherOverlay.Layers.Add(nOAAWeatherStationLayer);

            // Get the layers feature source and setup an event that will refresh the map when the data refreshes
            var featureSource = (NoaaWeatherStationFeatureSource)nOAAWeatherStationLayer.FeatureSource;
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

        private void FeatureSource_StationsUpdated(object sender, StationsUpdatedNoaaWeatherStationFeatureSourceEventArgs e)
        {
            // Here we fresh the map based on the delegate that fires when the feature source has new data.
            mapView.Overlays["Weather"].Refresh();
            loadingImage.Visibility = ViewStates.Visible;
        }
    }
}