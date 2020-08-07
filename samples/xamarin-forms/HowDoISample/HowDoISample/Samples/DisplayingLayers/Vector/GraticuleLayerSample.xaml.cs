using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GraticuleLayerSample : ContentPage
    {
        public GraticuleLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay layerOverlay = new LayerOverlay();
            mapView.Overlays.Add(layerOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            GraticuleFeatureLayer graticuleFeatureLayer = new GraticuleFeatureLayer();
            graticuleFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // We set the pen color to the graticule layer.
            graticuleFeatureLayer.GraticuleLineStyle.OuterPen.Color = GeoColor.FromArgb(125, GeoColors.Navy);

            // Add the layer to the overlay we created earlier.
            layerOverlay.Layers.Add("graticule", graticuleFeatureLayer);

            // Set the current extent of the map to start in Frisco TX
            mapView.CurrentExtent = new RectangleShape(-10782364.041857453, 3914916.6811720245, -10772029.75569071, 3908067.923475721);

            //Refresh the map.
            mapView.Refresh();
        }
    }
}