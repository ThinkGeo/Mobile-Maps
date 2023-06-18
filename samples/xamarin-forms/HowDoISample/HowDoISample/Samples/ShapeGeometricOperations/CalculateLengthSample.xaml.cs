using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to calculate the length of a line
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateLengthSample : ContentPage
    {
        public CalculateLengthSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the friscoTrails and selectedLineLayer layers
        ///     into a grouped LayerOverlay and display it on the map.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                 "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                 "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            var friscoTrailsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Hike_Bike.shp"));

            // Project friscoTrails layer to Spherical Mercator to match the map projection
            var projectionConverter = new ProjectionConverter(2276, 3857);
            friscoTrailsLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the friscoTrails layer
            friscoTrailsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            friscoTrailsLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Orange, 2, false);

            var friscoTrailsOverlay = new LayerOverlay();
            friscoTrailsOverlay.Layers.Add("FriscoTrailsLayer", friscoTrailsLayer);
            mapView.Overlays.Add("FriscoTrailsOverlay", friscoTrailsOverlay);

            // Create a layer to hold the selectedLineLayer found by the spatial query
            var selectedLineLayer = new InMemoryFeatureLayer();
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var selectedLineOverlay = new LayerOverlay();
            selectedLineOverlay.Layers.Add("SelectedLineLayer", selectedLineLayer);
            mapView.Overlays.Add("SelectedLineOverlay", selectedLineOverlay);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Calculates the length of a line selected on the map and displays it in the lengthResult TextBox
        /// </summary>
        private async void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            var friscoTrailsOverlay = (LayerOverlay) mapView.Overlays["FriscoTrailsOverlay"];
            var friscoTrailsLayer = (ShapeFileFeatureLayer)friscoTrailsOverlay.Layers["FriscoTrailsLayer"];

            var selectedLineOverlay = (LayerOverlay)mapView.Overlays["SelectedLineOverlay"];
            var selectedLineLayer = (InMemoryFeatureLayer)selectedLineOverlay.Layers["SelectedLineLayer"];

            // Query the friscoTrails layer to get the first feature closest to the map tap event
            var feature = friscoTrailsLayer.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Show the selected feature on the map
            selectedLineLayer.InternalFeatures.Clear();
            selectedLineLayer.InternalFeatures.Add(feature);
            await selectedLineOverlay.RefreshAsync();

            // Get the length of the first feature
            var length = ((LineBaseShape) feature.GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

            // Display the selectedLine's length in the lengthResult TextBox
            lengthResult.Text = $"{length:f3} km";
        }
    }
}