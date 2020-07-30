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
    public partial class CreateAreaStyleSample : ContentPage
    {
        public CreateAreaStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the cityLimits and bufferLayer layers into a grouped LayerOverlay and display them on the map.
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //    // Set the map's unit of measurement to meters(Spherical Mercator)
            //    mapView.MapUnit = GeographyUnit.Meter;

            //    // Add Cloud Maps as a background overlay
            //    var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //    mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //    // Set the map extent
            //    mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            //    ShapeFileFeatureLayer friscoSubdivisions;

            //    // Create a layer with polygon data
            //    friscoSubdivisions = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Parks.shp");

            //    // Project the layer's data to match the projection of the map
            //    friscoSubdivisions.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //    // Add the layer to a layer overlay
            //    var layerOverlay = new LayerOverlay();
            //    layerOverlay.Layers.Add(friscoSubdivisions);

            //    // Add the overlay to the map
            //    mapView.Overlays.Add(layerOverlay);

            //    // Add the area style to the historicSites layer
            //    AddAreaStyle(friscoSubdivisions);

        }

        /// <summary>
        /// Create a areaStyle and add it to the Historic Sites layer
        /// </summary>
        private void AddAreaStyle(ShapeFileFeatureLayer layer)
        {
            //// Create a area style
            //var areaStyle = new AreaStyle(GeoPens.DimGray, new GeoSolidBrush(new GeoColor(128, GeoColors.ForestGreen)));

            //// Add the area style to the collection of custom styles for ZoomLevel 1.
            //layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(areaStyle);

            //// Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the area style on every zoom level on the map.
            //layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}