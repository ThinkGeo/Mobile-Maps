using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a Shapefile Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShapefileLayerSample : ContentPage
    {
        public ShapefileLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the shapefile layer to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            var parksOverlay = new LayerOverlay();
            mapView.Overlays.Add(parksOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            var parksLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));
            parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            parksOverlay.Layers.Add("Frisco Parks", parksLayer);

            // Create a dashed pen that we will use below.
            var dashedPen = new GeoPen(GeoColors.Green, 5);
            dashedPen.DashPattern.Add(1);
            dashedPen.DashPattern.Add(1);

            // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                new AreaStyle(dashedPen, new GeoSolidBrush(new GeoColor(64, GeoColors.Green)));
            parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the current extent of the map to a few parks in the area
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015,
                3910065.3144544438);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}