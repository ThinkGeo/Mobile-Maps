using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to simplify a shape's geometry
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimplifyShapeSample : ContentPage
    {
        public SimplifyShapeSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the cityLimits and simplifyLayer layers into a
        ///     grouped LayerOverlay and display them on the map.
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

            var cityLimits = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/FriscoCityLimits.shp"));
            var simplifyLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style simplifyLayer
            simplifyLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            simplifyLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits layer to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add simplifyLayer to the layerOverlay
            layerOverlay.Layers.Add("simplifyLayer", simplifyLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Simplifies the first feature in the cityLimits layer and displays the results on the map.
        /// </summary>
        private async void SimplifyShape_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var cityLimits = (ShapeFileFeatureLayer) layerOverlay.Layers["cityLimits"];
            var simplifyLayer = (InMemoryFeatureLayer) layerOverlay.Layers["simplifyLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Simplify the first feature using the Douglas Peucker method
            var simplify = AreaBaseShape.Simplify(features[0].GetShape() as AreaBaseShape,
                Convert.ToInt32(tolerance.Text), SimplificationType.DouglasPeucker);

            // Add the simplified shape into simplifyLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            simplifyLayer.InternalFeatures.Clear();
            simplifyLayer.InternalFeatures.Add(new Feature(simplify));

            // Redraw the layerOverlay to see the simplified feature on the map
            await layerOverlay.RefreshAsync();
        }
    }
}