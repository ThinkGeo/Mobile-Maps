using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoogleMapLayerSample : ContentPage
    {
        public GoogleMapLayerSample()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Sets the map zoom level set to the Google maps zoom level set.
            mapView.ZoomLevelSet = new GoogleMapsZoomLevelSet();

            // Set the current extent to the whole world.
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015, 3910065.3144544438);
        }

        /// <summary>
        /// ...
        /// </summary>
        private void btnActivate_OnClicked(object sender, EventArgs e)
        {
            // Clear the current overlay
            mapView.Overlays.Clear();

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay worldOverlay = new LayerOverlay();
            mapView.Overlays.Add("WorldOverlay", worldOverlay);

            // Create the new layer.
            GoogleMapsLayer googleMapsLayer = new GoogleMapsLayer();

            // Add the layer to the overlay we created earlier.
            worldOverlay.Layers.Add("GoogleLayer", googleMapsLayer);

            // Set the client ID and Private key from the text box on the sample.
            googleMapsLayer.ApiKey = txtClientId.Text;
            googleMapsLayer.UriSigningSecret = txtUriSigningSecret.Text;

            // Refresh the map.
            mapView.Refresh();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs eventArgs)
        {
            await Launcher.OpenAsync("https://developers.google.com/maps/documentation/maps-static/get-api-key");
        }
    }
}
