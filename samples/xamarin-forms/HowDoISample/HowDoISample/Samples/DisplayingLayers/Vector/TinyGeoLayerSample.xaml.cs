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
    public partial class TinyGeoLayerSample : ContentPage
    {
        public TinyGeoLayerSample()
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
            //ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay tinyGeoOverlay = new LayerOverlay();
            //mapView.Overlays.Add(tinyGeoOverlay);

            //// Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            //TinyGeoFeatureLayer tinyGeoLayer = new TinyGeoFeatureLayer(@"../../../Data/TinyGeo/Zoning.tgeo");
            //tinyGeoLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Add the layer to the overlay we created earlier.
            //tinyGeoOverlay.Layers.Add("Zoning", tinyGeoLayer);

            //// Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            //tinyGeoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.Black, new GeoSolidBrush(new GeoColor(50, GeoColors.Blue)));
            //tinyGeoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Open the layer and set the map view current extent to the bounding box of the layer.
            //tinyGeoLayer.Open();
            //mapView.CurrentExtent = tinyGeoLayer.GetBoundingBox();

            //// Refresh the map.
            //mapView.Refresh();
        }
    }
}