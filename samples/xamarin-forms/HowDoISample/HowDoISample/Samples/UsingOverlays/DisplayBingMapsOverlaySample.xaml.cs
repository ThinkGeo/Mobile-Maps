using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayBingMapsOverlaySample : ContentPage
    {
        public DisplayBingMapsOverlaySample()
        {
            InitializeComponent();
        }
        // Launcher.OpenAsync is provided by Xamarin.Essentials.
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }


        /// <summary>
        /// Create a Bing Maps overlay and add it to the map view.
        /// </summary>
        private void DisplayBingMaps_Click(object sender, EventArgs e)
        {
            //BingMapsOverlay bingMapsOverlay = new BingMapsOverlay(bingApplicationId.Text, BingMapsMapType.Road);
            //mapView.Overlays.Add(bingMapsOverlay);
            //mapView.Refresh();
        }
    }
}