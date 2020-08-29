using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapPanZoomBarSample : ContentPage
    {
        public DisplayMapPanZoomBarSample()
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

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            mapView.Refresh();
        }


        /// <summary>
        /// Enable the PanZoomBar and remove it from the MapView
        /// </summary>
        private void DisplayPanZoomBar_Checked(object sender, EventArgs e)
        {
            mapView.MapTools.ZoomMapTool.IsEnabled = true;
        }

        /// <summary>
        /// Disable the PanZoomBar and remove it from the MapView
        /// </summary>
        private void DisplayPanZoomBar_Unchecked(object sender, EventArgs e)
        {
            mapView.MapTools.ZoomMapTool.IsEnabled = false;
        }
    }
}
