using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
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
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add a simple background overlay
            //mapView.BackgroundOverlay.BackgroundBrush = GeoBrushes.AliceBlue;

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }

        /// <summary>
        /// Create a Google Maps overlay and add it to the map view.
        /// </summary>
        private void DisplayGoogleMaps_Click(object sender, EventArgs e)
        {
            //GoogleMapsOverlay googleMapsOverlay = new GoogleMapsOverlay(googleApiKey.Text, googleSigningSecret.Text);
            //mapView.Overlays.Add(googleMapsOverlay);
            //mapView.Refresh();
        }
    }
}