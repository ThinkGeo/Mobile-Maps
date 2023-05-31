using System;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render Google Maps using the GoogleMapsOverlay.
    ///     A Google Maps API key and secret is required.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayGoogleMapsOverlaySample : ContentPage
    {
        public DisplayGoogleMapsOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with a background overlay and set the map's extent to Frisco, Tx.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add a simple background overlay
            mapView.BackgroundColor = new Color(234, 232, 226);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Create a Google Maps overlay and add it to the map view.
        /// </summary>
        private async void DisplayGoogleMaps_Click(object sender, EventArgs e)
        {
            var googleMapsOverlay = new GoogleMapsOverlay(googleApiKey.Text, googleSigningSecret.Text);
            mapView.Overlays.Add(googleMapsOverlay);
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Opens a link when the element is tapped on
        /// </summary>
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://developers.google.com/maps/documentation/maps-static/get-api-key");
        }
    }
}