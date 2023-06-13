using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to get the convex hull of a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetConvexHullSample
    {
        public GetConvexHullSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the cityLimits and convexHullLayer layers
        ///     into a grouped LayerOverlay and display them on the map.
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
            var convexHullLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the convexHullLayer
            convexHullLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            convexHullLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add convexHullLayer to the layerOverlay
            layerOverlay.Layers.Add("convexHullLayer", convexHullLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Gets Convex Hull of the first feature in the cityLimits layer and adds them to the convexHullLayer to display on
        ///     the map
        /// </summary>
        private async void ShapeConvexHull_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var cityLimits = (ShapeFileFeatureLayer) layerOverlay.Layers["cityLimits"];
            var convexHullLayer = (InMemoryFeatureLayer) layerOverlay.Layers["convexHullLayer"];

            // Query the cityLimits layer to get the first feature
            cityLimits.Open();
            var feature = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            cityLimits.Close();

            // Get the convex hull of the feature
            var convexHull = feature.GetConvexHull();

            // Add the convexHull into an InMemoryFeatureLayer to display the result.
            convexHullLayer.InternalFeatures.Clear();
            convexHullLayer.InternalFeatures.Add(convexHull);

            // Redraw the layerOverlay to see the convexHull feature on the map
            await layerOverlay.RefreshAsync();
        }

        //TODO: apply it to other samples. 
        protected override void OnDisappearing()
        {
            // You can use `mapView.Dispose()` to release all the resources. However, we don't call `mapView.Dispose()` here because
            // the `mapView` will not be initialized again if revisiting this page, which would result in accessing disposed object issues.
            // This limitation is specific to the structure of this HowDoI sample. You may consider using `mapView.Dispose()` as needed in your own project.
            mapView.Overlays.Clear();
            base.OnDisappearing();
        }
    }
}