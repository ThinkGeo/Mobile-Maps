using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to calculate the center point of a feature
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateCenterPointSample : ContentPage
    {
        public CalculateCenterPointSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the censusHousing and centerPointLayer layers
        ///     into a grouped LayerOverlay and display it on the map.
        /// </summary>
        protected override void OnAppearing()
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

            var censusHousing = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco 2010 Census Housing Units.shp"));
            var centerPointLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project censusHousing layer to Spherical Mercator to match the map projection
            censusHousing.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style censusHousing layer
            censusHousing.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            censusHousing.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style centerPointLayer
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 12, GeoColors.White, 4);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.Black, 2);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add censusHousing layer to a LayerOverlay
            layerOverlay.Layers.Add("censusHousing", censusHousing);

            // Add centerPointLayer to the layerOverlay
            layerOverlay.Layers.Add("centerPointLayer", centerPointLayer);

            // Set the map extent to the censusHousing layer bounding box
            censusHousing.Open();
            mapView.CurrentExtent = censusHousing.GetBoundingBox();
            censusHousing.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            centroidCenter.IsChecked = true;

            mapView.Refresh();
        }

        /// <summary>
        ///     Calculates the center point of a feature
        /// </summary>
        /// <param name="feature"> The target feature to calculate it's center point</param>
        private void CalculateCenterPoint(Feature feature)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];
            var centerPointLayer = (InMemoryFeatureLayer) layerOverlay.Layers["centerPointLayer"];

            PointShape centerPoint;

            // Get the CenterPoint of the selected feature
            if (centroidCenter.IsChecked)
                // Centroid, or geometric center, method. Accurate, but can be relatively slower on extremely complex shapes
                centerPoint = feature.GetShape().GetCenterPoint();
            else
                // BoundingBox method. Less accurate, but much faster
                centerPoint = feature.GetBoundingBox().GetCenterPoint();

            // Show the centerPoint on the map
            centerPointLayer.InternalFeatures.Clear();
            centerPointLayer.InternalFeatures.Add("selectedFeature", feature);
            centerPointLayer.InternalFeatures.Add("centerPoint", new Feature(centerPoint));

            // Refresh the overlay to show the results
            layerOverlay.Refresh();
        }

        /// <summary>
        ///     Map event that fires whenever the user taps on the map. Gets the closest feature from the tap event and calculates
        ///     the center point
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];
            var censusHousing = (ShapeFileFeatureLayer) layerOverlay.Layers["censusHousing"];

            // Query the censusHousing layer to get the first feature closest to the map tap event
            var feature = censusHousing.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter,
                1,
                ReturningColumnsType.NoColumns).First();

            CalculateCenterPoint(feature);
        }

        /// <summary>
        ///     RadioButton checked event that will recalculate the center point so long as a feature was already selected
        /// </summary>
        private void RadioButton_Checked(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];
            var centerPointLayer = (InMemoryFeatureLayer) layerOverlay.Layers["centerPointLayer"];

            // Recalculate the center point if a feature has already been selected
            if (centerPointLayer.InternalFeatures.Contains("selectedFeature"))
                CalculateCenterPoint(centerPointLayer.InternalFeatures["selectedFeature"]);
        }
    }
}