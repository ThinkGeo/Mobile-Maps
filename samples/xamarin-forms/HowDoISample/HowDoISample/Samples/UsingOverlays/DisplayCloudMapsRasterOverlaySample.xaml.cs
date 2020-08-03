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
    public partial class DisplayCloudMapsRasterOverlaySample : ContentPage
    {
        public DisplayCloudMapsRasterOverlaySample()
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
            //mapView.CurrentExtent = new RectangleShape(-10782598.9806675, 3915669.09132595, -10772234.1196896, 3906343.77392696);
        }

        /// <summary>
        /// Create a ThinkGeo Cloud Maps raster overlay and add it to the map view.
        /// </summary>
        private void DisplayRasterCloudMaps_Click(object sender, EventArgs e)
        {
            //var thinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudRasterMapsMapType.Hybrid);
            //mapView.Overlays.Add(thinkGeoCloudRasterMapsOverlay);
            //mapView.Refresh();
        }

    }
}