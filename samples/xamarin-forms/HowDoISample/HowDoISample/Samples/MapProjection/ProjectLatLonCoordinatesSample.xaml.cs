using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to reproject features using the ProjectionConverter class
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectLatLonCoordinatesSample : ContentPage
    {
        public ProjectLatLonCoordinatesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a new feature layer to display the shapes we will be reprojecting
            var reprojectedFeaturesLayer = new InMemoryFeatureLayer();

            // Add a point, line, and polygon style to the layer. These styles control how the shapes will be drawn
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple);
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 6, false);
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Apply these styles on all zoom levels. This ensures our shapes will be visible on all zoom levels
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to an overlay
            var reprojectedFeaturesOverlay = new LayerOverlay();
            reprojectedFeaturesOverlay.Layers.Add("Reprojected Features Layer", reprojectedFeaturesLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("Reprojected Features Overlay", reprojectedFeaturesOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10779751.80, 3915369.33, -10779407.60, 3915141.57);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Draw reprojected features on the map
        /// </summary>
        private async Task ClearMapAndAddFeaturesAsync(Collection<Feature> reprojectedFeatures)
        {
            // Get the layer we prepared from the MapView
            var reprojectedFeatureLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Reprojected Features Layer");

            // Clear old features from the feature layer and add the newly reprojected features
            reprojectedFeatureLayer.InternalFeatures.Clear();
            foreach (var sphericalMercatorFeature in reprojectedFeatures)
                reprojectedFeatureLayer.InternalFeatures.Add(sphericalMercatorFeature);

            // Set the map extent to zoom into the feature and refresh the map
            reprojectedFeatureLayer.Open();
            mapView.CurrentExtent = reprojectedFeatureLayer.GetBoundingBox();

            var standardZoomLevelSet = new ZoomLevelSet();
            await mapView.ZoomToScaleAsync(standardZoomLevelSet.ZoomLevel18.Scale);

            reprojectedFeatureLayer.Close();
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Use the ProjectionConverter class to reproject multiple different features
        /// </summary>
        private async void ReprojectMultipleFeatures_Click(object sender, EventArgs e)
        {
            // Create features based on the WKT in the textbox in the UI
            var decimalDegreeFeatures = new Collection<Feature>();
            var wktStrings = new Collection<string>();
            wktStrings.Add("POINT(-96.834516 33.150083)");
            wktStrings.Add("LINESTRING(-96.83559 33.149,-96.835866046134 33.1508413556856,-96.835793626491 33.1508974965687,-96.8336008970734 33.1511063402186,-96.83356 33.15109,-96.83328 33.14922)");
            wktStrings.Add("POLYGON((-96.83582 33.1508,-96.83578 33.15046,-96.83353 33.15068,-96.83358 33.15102,-96.83582 33.1508))");

            foreach (var wktString in wktStrings)
                    decimalDegreeFeatures.Add(new Feature(wktString));

            //Create a new ProjectionConverter to convert between Decimal Degrees(4326) and Spherical Mercator(3857)
            var projectionConverter = new ProjectionConverter(4326, 3857);

            //Convert the feature to Spherical Mercator
            projectionConverter.Open();
            var sphericalMercatorFeatures = projectionConverter.ConvertToExternalProjection(decimalDegreeFeatures);

            // Add the reprojected features to the map
            await ClearMapAndAddFeaturesAsync(sphericalMercatorFeatures);
        }
    }
}