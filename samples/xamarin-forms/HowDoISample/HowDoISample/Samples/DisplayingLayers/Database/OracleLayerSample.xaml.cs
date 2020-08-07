using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.UI.Forms;


namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OracleLayerSample : ContentPage
    {
        public OracleLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Interaction logic for Placeholder.xaml
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay schoolOverlay = new LayerOverlay();
            mapView.Overlays.Add(schoolOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
            //OracleFeatureLayer schoolLayer = new OracleFeatureLayer(@"OCI:system/ThinkGeodatabasepassword!@sampledatabases.thinkgeo.com/xe", "SCHOOLS", "OGR_FID");
            OracleFeatureLayer schoolLayer = new OracleFeatureLayer(@"OCI:ThinkGeoSampleUser/ThinkGeoSamplePassword@sampledatabases.thinkgeo.com/xe", "SYSTEM.SCHOOLS", "OGR_FID");
            schoolLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Add the layer to the overlay we created earlier.
            schoolOverlay.Layers.Add("Schools", schoolLayer);

            //// Set a point style to zoom level 1 and then apply it to all zoom levels up to 20.
            schoolLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoColors.White, 2));
            schoolLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map view current extent to a bounding box that shows just a few sightings.
            mapView.CurrentExtent = new RectangleShape(-10789388.4602951, 3923878.18083465, -10768258.7082788, 3906668.46719412);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}