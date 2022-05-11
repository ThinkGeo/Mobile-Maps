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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            var friscoTrails = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Hike_Bike.shp"));
            var selectedLineLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project friscoTrails layer to Spherical Mercator to match the map projection
            friscoTrails.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoTrails layer
            friscoTrails.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Orange, 2, false);
            friscoTrails.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style selectedLineLayer
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoTrails layer to a LayerOverlay
            layerOverlay.Layers.Add("friscoTrails", friscoTrails);

            // Add selectedLineLayer to the layerOverlay
            layerOverlay.Layers.Add("selectedLineLayer", selectedLineLayer);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            mapView.Refresh();
        }

        /// <summary>
        ///     Calculates the length of a line selected on the map and displays it in the lengthResult TextBox
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var friscoTrails = (ShapeFileFeatureLayer) layerOverlay.Layers["friscoTrails"];
            var selectedLineLayer = (InMemoryFeatureLayer) layerOverlay.Layers["selectedLineLayer"];

            // Query the friscoTrails layer to get the first feature closest to the map tap event
            var feature = friscoTrails.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Show the selected feature on the map
            selectedLineLayer.InternalFeatures.Clear();
            selectedLineLayer.InternalFeatures.Add(feature);
            layerOverlay.Refresh();

            // Get the length of the first feature
            var length = ((LineBaseShape) feature.GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

            // Display the selectedLine's length in the lengthResult TextBox
            lengthResult.Text = $"{length:f3} km";
        }
    }
}