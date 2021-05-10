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
    public partial class CreateLineStyleSample : ContentPage
    {
        public CreateLineStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay. Also, load Frisco Streets shapefile data and add it to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudRasterMapsMapType.Aerial);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10779675.1746605, 3914631.77546835, -10779173.5566652, 3914204.80300804);

            // Create a layer with line data
            ShapeFileFeatureLayer friscoRailroad = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Railroad/Railroad.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project the layer's data to match the projection of the map
            friscoRailroad.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to a layer overlay
            layerOverlay.Layers.Add("Railroad", friscoRailroad);

            // Add the overlay to the map
            mapView.Overlays.Add("overlay", layerOverlay);

            rbLineStyle.IsChecked = true;
        }

        private void rbLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (mapView.Overlays.Count > 0)
            {
                LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["overlay"];
                ShapeFileFeatureLayer friscoRailroad = (ShapeFileFeatureLayer)layerOverlay.Layers["Railroad"];

                // Create a line style
                var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));

                // Add the line style to the collection of custom styles for ZoomLevel 1.
                friscoRailroad.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
                friscoRailroad.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(lineStyle);

                // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
                friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Refresh the layerOverlay to show the new style
                layerOverlay.Refresh();
            }
        }

        private void rbDashedLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (mapView.Overlays.Count > 0)
            {
                LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["overlay"];
                ShapeFileFeatureLayer friscoRailroad = (ShapeFileFeatureLayer)layerOverlay.Layers["Railroad"];

                var lineStyle = new LineStyle(
                    outerPen: new GeoPen(GeoColors.Black, 6),
                    innerPen: new GeoPen(GeoColors.White, 4)
                    {
                        DashStyle = LineDashStyle.Custom,
                        DashPattern = { 3f, 3f },
                        StartCap = DrawingLineCap.Flat,
                        EndCap = DrawingLineCap.Flat
                    }
                );

                // Add the line style to the collection of custom styles for ZoomLevel 1.
                friscoRailroad.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
                friscoRailroad.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(lineStyle);

                // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
                friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Refresh the layerOverlay to show the new style
                layerOverlay.Refresh();
            }
        }
    }
}
