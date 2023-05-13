using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to translate a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TranslateShapeSample : ContentPage
    {
        public TranslateShapeSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the cityLimits and translatedLayer layers
        ///     into a grouped LayerOverlay and display them on the map.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            ShapeFileFeatureLayer.BuildIndexFile(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/FriscoCityLimits.shp"));

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            var cityLimits = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/FriscoCityLimits.shp"));
            var translatedLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style translatedLayer
            translatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            translatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits layer to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add translatedLayer to the layerOverlay
            layerOverlay.Layers.Add("translatedLayer", translatedLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            mapView.Refresh();
        }

        /// <summary>
        ///     Translates the first feature in the cityLimits layer and displays the result on the map.
        /// </summary>
        private void OffsetTranslateShape_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var cityLimits = (ShapeFileFeatureLayer) layerOverlay.Layers["cityLimits"];
            var translatedLayer = (InMemoryFeatureLayer) layerOverlay.Layers["translatedLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Translate the first feature's shape by the X and Y values on the UI in meters
            var translate = BaseShape.TranslateByOffset(features[0].GetShape(), Convert.ToDouble(translateX.Text),
                Convert.ToDouble(translateY.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            // Add the translated shape into translatedLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            // underlying data using BeginTransaction and CommitTransaction instead.
            translatedLayer.InternalFeatures.Clear();
            translatedLayer.InternalFeatures.Add(new Feature(translate));

            // Redraw the layerOverlay to see the translated feature on the map
            layerOverlay.Refresh();
        }

        private void DegreeTranslateShape_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var cityLimits = (ShapeFileFeatureLayer) layerOverlay.Layers["cityLimits"];
            var translatedLayer = (InMemoryFeatureLayer) layerOverlay.Layers["translatedLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Translate the first feature's shape by the X and Y values on the UI in meters
            var translate = BaseShape.TranslateByDegree(features[0].GetShape(),
                Convert.ToDouble(translateDistance.Text), Convert.ToDouble(translateAngle.Text), GeographyUnit.Meter,
                DistanceUnit.Meter);

            // Add the translated shape into translatedLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            // underlying data using BeginTransaction and CommitTransaction instead.
            translatedLayer.InternalFeatures.Clear();
            translatedLayer.InternalFeatures.Add(new Feature(translate));

            // Redraw the layerOverlay to see the translated feature on the map
            layerOverlay.Refresh();
        }
    }
}