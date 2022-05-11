using System;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a Google Maps Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoogleMapLayerSample : ContentPage
    {
        public GoogleMapLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the MapView
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Sets the map zoom level set to the Google maps zoom level set.
            mapView.ZoomLevelSet = new GoogleMapsZoomLevelSet();

            // Set the current extent to the whole world.
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015,
                3910065.3144544438);
        }

        /// <summary>
        ///     Add the Google Maps Layer to the map
        /// </summary>
        private void btnActivate_OnClicked(object sender, EventArgs e)
        {
            // Clear the current overlay
            mapView.Overlays.Clear();

            // Create a new overlay that will hold our new layer and add it to the map.
            var worldOverlay = new LayerOverlay();
            mapView.Overlays.Add("WorldOverlay", worldOverlay);

            // Create the new layer.
            var googleMapsLayer = new GoogleMapsLayer();

            // Add the layer to the overlay we created earlier.
            worldOverlay.Layers.Add("GoogleLayer", googleMapsLayer);

            // Set the API Key and URI Signing Secret from the text box on the sample.
            googleMapsLayer.ApiKey = txtApiKey.Text;
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