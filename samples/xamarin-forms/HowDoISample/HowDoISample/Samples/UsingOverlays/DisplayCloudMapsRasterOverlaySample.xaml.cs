using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayCloudMapsRasterOverlaySample : ContentPage
    {
        public DisplayCloudMapsRasterOverlaySample()
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
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10782598.9806675, 3915669.09132595, -10772234.1196896, 3906343.77392696);
        }

        /// <summary>
        /// Create a ThinkGeo Cloud Maps raster overlay and add it to the map view.
        /// </summary>
        private void DisplayRasterCloudMaps_Click(object sender, EventArgs e)
        {
            var thinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(cloudMapsApiKey.Text, cloudMapsSecretKey.Text, ThinkGeoCloudRasterMapsMapType.Hybrid);
            mapView.Overlays.Add(thinkGeoCloudRasterMapsOverlay);
            mapView.Refresh();
        }

        /// <summary>
        /// Opens a link when the element is tapped on
        /// </summary>
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
        }
    }
}
