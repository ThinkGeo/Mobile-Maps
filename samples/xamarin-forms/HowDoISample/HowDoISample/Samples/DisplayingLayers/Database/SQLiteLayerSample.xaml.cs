using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class SQLiteLayerSample : ContentPage
    {
        public SQLiteLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U - efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay restuarantsOverlay = new LayerOverlay();
            mapView.Overlays.Add(restuarantsOverlay);
            string resturantPath = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/SQLite/frisco-restaurants.sqlite"));
            // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
            SqliteFeatureLayer restaurantsLayer = new SqliteFeatureLayer($"Data Source={resturantPath};", "restaurants", "id", "geometry");
            restaurantsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            restuarantsOverlay.Layers.Add("Frisco Restaurants", restaurantsLayer);

            // Create a new text style and set various settings to make it look good.
            var textStyle = new TextStyle("Name", new GeoFont("Arial", 12), GeoBrushes.Black);
            textStyle.MaskType = MaskType.RoundedCorners;
            textStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            textStyle.Mask = new AreaStyle(GeoBrushes.WhiteSmoke);
            textStyle.SuppressPartialLabels = true;
            textStyle.YOffsetInPixel = -5;

            // Set a point style and the above text style to zoom level 1 and then apply it to all zoom levels up to 20.
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Green, new GeoPen(GeoColors.White, 2));
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map view current extent to a bounding box that shows just a few restaurants.
            mapView.CurrentExtent = new RectangleShape(-10776971.1234695, 3915454.06613793, -10775965.157585, 3914668.53918197);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}