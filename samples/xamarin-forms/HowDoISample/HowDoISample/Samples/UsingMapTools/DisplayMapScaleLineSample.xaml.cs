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
    public partial class DisplayMapScaleLineSample : ContentPage
    {
        public DisplayMapScaleLineSample()
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

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }


        /// <summary>
        /// Enable the ScaleLine and add it to the MapView (default: bottom left)
        /// </summary>
        private void DisplayScaleLine_Checked(object sender, EventArgs e)
        {
           // mapView.MapTools.ScaleLine.IsEnabled = true;
        }

        /// <summary>
        /// Disable the ScaleLine and remove it from the MapView
        /// </summary>
        private void DisplayScaleLine_Unchecked(object sender, EventArgs e)
        {
            //mapView.MapTools.ScaleLine.IsEnabled = false;
        }
    }
}
