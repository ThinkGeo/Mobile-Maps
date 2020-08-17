using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayGoogleMapsOverlaySample : ContentPage
    {
        public DisplayGoogleMapsOverlaySample()
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

            // Add a simple background overlay
            mapView.BackgroundColor = new Color(234, 232, 226);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }

        /// <summary>
        /// Create a Google Maps overlay and add it to the map view.
        /// </summary>
        private void DisplayGoogleMaps_Click(object sender, EventArgs e)
        {
           // GoogleMapsOverlay googleMapsOverlay = new GoogleMapsOverlay(googleApiKey.Text, googleSigningSecret.Text);
           // mapView.Overlays.Add(googleMapsOverlay);
           // mapView.Refresh();
        }
    }
}