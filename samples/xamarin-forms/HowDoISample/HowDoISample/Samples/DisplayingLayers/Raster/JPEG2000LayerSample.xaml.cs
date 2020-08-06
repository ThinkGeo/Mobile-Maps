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
    public partial class JPEG2000LayerSample : ContentPage
    {
        public JPEG2000LayerSample()
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

            //// Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            //ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay layerOverlay = new LayerOverlay();
            //mapView.Overlays.Add(layerOverlay);

            //// Create the new layer and dd the layer to the overlay we created earlier.
            //MrSidRasterLayer jp2000RasterLayer = new MrSidRasterLayer("../../../Data/Jp2/m_3309650_sw_14_1_20160911_20161121.jp2");
            //layerOverlay.Layers.Add(jp2000RasterLayer);

            //// Set the map view current extent to a slightly zoomed in area of the image.
            //mapView.CurrentExtent = new RectangleShape(-10783910.2966461, 3917274.29233111, -10777309.4670677, 3912119.9131963);

            //// Refresh the map.
            //mapView.Refresh();
        }
    }
}