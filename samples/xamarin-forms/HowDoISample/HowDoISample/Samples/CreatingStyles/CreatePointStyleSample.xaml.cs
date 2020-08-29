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
    public partial class CreatePointStyleSample : ContentPage
    {
        public CreatePointStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay. Also, load Frisco Hotels shapefile data and add it to the map
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

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Hotels.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project the layer's data to match the projection of the map
            hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to a layer overlay
            layerOverlay.Layers.Add("hotels", hotelsLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("hotels", layerOverlay);

            pointSymbol.IsChecked = true;

            mapView.Refresh();
        }

        /// <summary>
        /// Create a pointStyle using a PointSymbol and add it to the Hotels layer
        /// </summary>
        private void PointSymbol_OnChecked(object sender, EventArgs e)
        {
            if (mapView.Overlays.Count > 0)
            {
                LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["hotels"];
                ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

                // Create a point style
                var pointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoBrushes.White, 2));

                // Add the point style to the collection of custom styles for ZoomLevel 1.
                hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
                hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);

                // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map.
                hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Refresh the layerOverlay to show the new style
                layerOverlay.Refresh();
            }
        }

        /// <summary>
        /// Create a pointStyle using an icon image and add it to the Hotels layer
        /// </summary>
        private void Icon_OnChecked(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["hotels"];
            ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

            // Create a point style
            var pointStyle = new PointStyle(new GeoImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/hotel_icon.png")))
            {
                ImageScale = .25
            };

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);

            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            layerOverlay.Refresh();
        }


        /// <summary>
        /// Create a pointStyle using a font symbol and add it to the Hotels layer
        /// </summary>
        private void Symbol_Checked(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["hotels"];
            ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

            // Create a point style
            var symbolPointStyle = new PointStyle(new GeoFont("Verdana", 16, DrawingFontStyles.Bold), "@", GeoBrushes.Black)
            {
                Mask = new AreaStyle(GeoBrushes.White),
                MaskType = MaskType.Circle
            };

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(symbolPointStyle);

            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            layerOverlay.Refresh();
        }
    }
}
