using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render ThinkGeo Cloud Maps in vector format.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayCloudMapsVectorOverlaySample : ContentPage
    {
        public DisplayCloudMapsVectorOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with a background overlay and set the map's extent to Frisco, Tx.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }

        /// <summary>
        ///     Create a ThinkGeo Cloud Maps vector overlay and add it to the map view.
        /// </summary>
        private void DisplayVectorCloudMaps_Click(object sender, EventArgs e)
        {
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(cloudMapsApiKey.Text,
                cloudMapsSecretKey.Text, ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);
            mapView.Refresh();
        }

        /// <summary>
        ///     Opens a link when the element is tapped on
        /// </summary>
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
        }
    }
}