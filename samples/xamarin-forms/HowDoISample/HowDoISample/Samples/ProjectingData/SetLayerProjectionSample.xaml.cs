using System;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to automatically reproject a layer using the ProjectionConverter class
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetLayerProjectionSample : ContentPage
    {
        public SetLayerProjectionSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                 "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                 "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the Map Unit to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;


            // Create an overlay that we can add feature layers to, and add it to the MapView
            var subdivisionsOverlay = new LayerOverlay();
            mapView.Overlays.Add("Frisco Subdivisions Overlay", subdivisionsOverlay);

            // Reproject a shapefile and set the extent
            await ReprojectFeaturesFromShapefile();

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Use the ProjectionConverter class to reproject features in a ShapeFileFeatureLayer
        /// </summary>
        private async Task ReprojectFeaturesFromShapefile()
        {
            // Create a feature layer to hold the Frisco subdivisions data
            var subdivisionsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Subdivisions.shp"));

            // Create a new ProjectionConverter to convert between Texas North Central (2276) and Spherical Mercator (3857)
            var projectionConverter = new ProjectionConverter(2276, 3857);
            subdivisionsLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco subdivions polygons
            subdivisionsLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Apply the styles across all zoom levels
            subdivisionsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Get the overlay we prepared from the MapView, and add the subdivisions ShapeFileFeatureLayer to it
            var subdivisionsOverlay = (LayerOverlay) mapView.Overlays["Frisco Subdivisions Overlay"];
            subdivisionsOverlay.Layers.Clear();
            subdivisionsOverlay.Layers.Add("Frisco Subdivisions", subdivisionsLayer);

            // Set the map to the extent of the subdivisions features and refresh the map
            subdivisionsLayer.Open();
            mapView.CurrentExtent = subdivisionsLayer.GetBoundingBox();
            subdivisionsLayer.Close();
            await mapView.RefreshAsync();
        }
    }
}