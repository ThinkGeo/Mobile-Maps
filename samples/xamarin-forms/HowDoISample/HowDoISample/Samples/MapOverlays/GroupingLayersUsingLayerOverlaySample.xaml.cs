using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to group layers into logical groups using LayerOverlays.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupingLayersUsingLayerOverlaySample : ContentPage
    {
        public GroupingLayersUsingLayerOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, load landuse and POI layers into a grouped LayerOverlay
        ///     and display them on the map.
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

            //**********************
            // * Landuse LayerOverlay
            // **********************/

            // Create cityLimits layer
            var cityLimits = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/FriscoCityLimits.shp"));

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.DimGray, 2);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            var poiOverlay = new LayerOverlay();
            var landuseOverlay = new LayerOverlay();

            // Add cityLimits layer to the landuseGroup overlay
            landuseOverlay.Layers.Add(cityLimits);

            // Create Parks landuse layer
            var parks = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));

            // Style Parks landuse layer
            parks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(128, GeoColors.Green), GeoColors.Transparent);
            parks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Project Parks landuse layer to Spherical Mercator to match the map projection
            parks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add Parks landuse layer to the landuseGroup overlay
            landuseOverlay.Layers.Add(parks);

            // Add Landuse overlay to the map
            mapView.Overlays.Add("landuseOverlay", landuseOverlay);

            //******************
            // * POI LayerOverlay
            // ******************/

            // Create Hotel POI layer
            var hotels = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Hotels.shp"));

            // Style Hotel POI layer
            hotels.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.White, 2);
            hotels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Project Hotels POI layer to Spherical Mercator to match the map projection
            hotels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add Hotel POI layer to the poiGroup overlay
            poiOverlay.Layers.Add(hotels);

            // Create School POI layer
            var schools = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Schools.shp"));

            // Style School POI layer
            schools.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Red, 8, GeoColors.White, 2);
            schools.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Project Schools POI layer to Spherical Mercator to match the map projection
            schools.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add School POI layer to the poiGroup overlay
            poiOverlay.Layers.Add(schools);

            // Add POI overlay to the map
            mapView.Overlays.Add("poiOverlay", poiOverlay);

            // Set the map extent
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            ShowPoi.IsChecked = true;
            ShowLandUse.IsChecked = true;

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Show the Land Use overlay
        /// </summary>
        private async void ShowLanduseGroup_Checked(object sender, EventArgs e)
        {
            var landUseOverlay = (LayerOverlay) mapView.Overlays["landuseOverlay"];
            landUseOverlay.IsVisible = ShowLandUse.IsChecked;
            await landUseOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Show the POI overlay
        /// </summary>
        private async void ShowPoiGroup_Checked(object sender, EventArgs e)
        {
            var poiOverlay = (LayerOverlay) mapView.Overlays["poiOverlay"];
            poiOverlay.IsVisible = ShowPoi.IsChecked;
            await poiOverlay.RefreshAsync();
        }
    }
}