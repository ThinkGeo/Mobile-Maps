using System;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a CloudRasterMaps Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CloudMapsRasterLayerSample : ContentPage
    {
        public CloudMapsRasterLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the layer overlay with some additional settings and add to the map.
            var cloudOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            mapView.Overlays.Add("Cloud Overlay", cloudOverlay);

            // Set the current extent to a neighborhood in Frisco Texas.
            mapView.CurrentExtent =
                new RectangleShape(-10781708.9749424, 3913502.90429046, -10777685.1114043, 3910360.79646662);

            // Refresh the map.
            mapView.Refresh();
        }

        /// <summary>
        ///     Switch the Map Type based on the radio buttons
        /// </summary>
        private void rbMapType_Checked(object sender, EventArgs e)
        {
            var button = (RadioButton) sender;
            if (mapView.Overlays.Contains("Cloud Overlay"))
            {
                var cloudOverlay = (ThinkGeoCloudRasterMapsOverlay) mapView.Overlays["Cloud Overlay"];

                switch (button.Content.ToString())
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
                }

                mapView.Refresh();
            }
        }
    }
}