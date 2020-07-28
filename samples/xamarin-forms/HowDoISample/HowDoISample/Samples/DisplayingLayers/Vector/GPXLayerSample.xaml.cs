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
    public partial class GPXLayerSample : ContentPage
    {
        public GPXLayerSample()
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
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay gpxOverlay = new LayerOverlay();
            //mapView.Overlays.Add(gpxOverlay);

            //// Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            //GpxFeatureLayer gpxLayer = new GpxFeatureLayer(@"../../../Data/Gpx/Hike_Bike.gpx");
            //gpxLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            //// Add the layer to the overlay we created earlier.
            //gpxOverlay.Layers.Add("Hike Bike Trails", gpxLayer);

            //// Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            //gpxLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.Black);
            //gpxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Open the layer and set the map view current extent to the bounding box of the layer.  
            //gpxLayer.Open();
            //mapView.CurrentExtent = gpxLayer.GetBoundingBox();

            //// Refresh the map.
            //mapView.Refresh();
        }
    }
}