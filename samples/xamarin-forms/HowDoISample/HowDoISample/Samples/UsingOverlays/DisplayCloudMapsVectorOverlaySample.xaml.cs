using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayCloudMapsVectorOverlaySample : ContentPage
    {
        public DisplayCloudMapsVectorOverlaySample()
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
        /// Enable the PanZoomBar and remove it from the MapView
        /// </summary>
        /// <summary>
        /// Create a ThinkGeo Cloud Maps vector overlay and add it to the map view.
        /// </summary>
        private void DisplayVectorCloudMaps_Click(object sender, EventArgs e)
        {
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);
            //mapView.Refresh();
        }
    }
}