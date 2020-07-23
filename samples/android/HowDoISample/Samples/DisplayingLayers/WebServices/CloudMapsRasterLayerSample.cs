using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display ThinkGeo cloud raster background images.
    /// </summary>
    public class CloudMapsRasterLayerSample : SampleFragment
    {
        // Controls
        private MapView mapView;

        /// <summary>
        /// Defines the Layout to use from the `Resources/layout` directory
        /// </summary>
        public override int Layout => Resource.Layout.__SampleTemplate;

        /// <summary>
        /// Creates the sample view from the Layout resource and exposes controls from the view that needs to be 
        /// referenced for the sample to run (mapView, buttons, etc.)
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Call the base OnCreateView method to inflate the Layout with basic functionality
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            // Bind the controls needed from the Layout to the class
            mapView = view.FindViewById<MapView>(Resource.Id.mapView);

            return view;
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudRasterMapsMapType.Hybrid);
            mapView.Overlays.Add(thinkGeoCloudRasterMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Set the current extent to a neighborhood in Frisco Texas.
            mapView.CurrentExtent = new RectangleShape(-10781708.9749424, 3913502.90429046, -10777685.1114043, 3910360.79646662);

            // Refresh the map.
            mapView.Refresh();
        }

        private void rbMapType_Checked(object sender, EventArgs e)
        {
            RadioButton button = (RadioButton)sender;
            if (mapView.Overlays.Contains("Cloud Overlay"))
            {
                ThinkGeoCloudRasterMapsOverlay cloudOverlay = (ThinkGeoCloudRasterMapsOverlay)mapView.Overlays["Cloud Overlay"];

                switch (button.Text)
                {
                    case "Light":
                        cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                        break;
                    case "Dark":
                        cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
                        break;
                    case "Aerial":
                        cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                        break;
                    case "Hybrid":
                        cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                        break;
                    default:
                        break;
                }
                mapView.Refresh();
            }
        }
    }
}