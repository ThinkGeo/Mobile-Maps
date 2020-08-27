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
    public partial class DisplayBingMapsOverlaySample : ContentPage
    {
        public DisplayBingMapsOverlaySample()
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

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }


        /// <summary>
        /// Create a Bing Maps overlay and add it to the map view.
        /// </summary>
        private void DisplayBingMaps_Click(object sender, EventArgs e)
        {
            BingMapsOverlay bingMapsOverlay = new BingMapsOverlay() {ApplicationId = bingApplicationId.Text, MapStyle = BingMapsMapType.Road};
            mapView.Overlays.Add(bingMapsOverlay);
            mapView.Refresh();
        }

        /// <summary>
        /// Opens a link when the element is tapped on
        /// </summary>
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.bingmapsportal.com/");
        }
    }
}
