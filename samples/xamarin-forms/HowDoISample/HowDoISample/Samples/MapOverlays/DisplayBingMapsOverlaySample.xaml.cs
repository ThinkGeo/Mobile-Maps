using System;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render Bing Maps using the BingMapsOverlay.
    ///     A valid Bing Maps ApplicationID is required.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayBingMapsOverlaySample : ContentPage
    {
        public DisplayBingMapsOverlaySample()
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

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            await mapView.RefreshAsync();
        }


        /// <summary>
        ///     Create a Bing Maps overlay and add it to the map view.
        /// </summary>
        private async void DisplayBingMaps_Click(object sender, EventArgs e)
        {
            var bingMapsOverlay = new BingMapsOverlay
                {ApplicationId = bingApplicationId.Text, MapStyle = BingMapsMapType.Road};
            mapView.Overlays.Add(bingMapsOverlay);
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Opens a link when the element is tapped on
        /// </summary>
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.bingmapsportal.com/");
        }
    }
}