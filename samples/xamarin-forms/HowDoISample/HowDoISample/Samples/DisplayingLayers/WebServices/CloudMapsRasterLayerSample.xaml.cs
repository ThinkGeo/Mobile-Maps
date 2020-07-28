using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CloudMapsRasterLayerSample : ContentPage
    {
        public CloudMapsRasterLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// It is important to set the map unit first to either feet, meters or decimal degrees.
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Set the map zoom level set to the Cloud Maps zoom level set.
            //mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            //// Create the layer overlay with some additional settings and add to the map.
            //ThinkGeoCloudRasterMapsOverlay cloudOverlay = new ThinkGeoCloudRasterMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~");
            //cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            //mapView.Overlays.Add("Cloud Overlay", cloudOverlay);

            //// Set the current extent to a neighborhood in Frisco Texas.
            //mapView.CurrentExtent = new RectangleShape(-10781708.9749424, 3913502.90429046, -10777685.1114043, 3910360.79646662);

            //// Refresh the map.
            //mapView.Refresh();
        }

        private void rbMapType_Checked(object sender, EventArgs e)
        {
            //RadioButton button = (RadioButton)sender;
            //if (mapView.Overlays.Contains("Cloud Overlay"))
            //{
            //    ThinkGeoCloudRasterMapsOverlay cloudOverlay = (ThinkGeoCloudRasterMapsOverlay)mapView.Overlays["Cloud Overlay"];

            //    switch (button.Content.ToString())
            //    {
            //        case "Light":
            //            cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            //            break;
            //        case "Dark":
            //            cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            //            break;
            //        case "Aerial":
            //            cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            //            break;
            //        case "Hybrid":
            //            cloudOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            //            break;
            //        default:
            //            break;
            //    }
            //    mapView.Refresh();
            //}
        }
    }
}