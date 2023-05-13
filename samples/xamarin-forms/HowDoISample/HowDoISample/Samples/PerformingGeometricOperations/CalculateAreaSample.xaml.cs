using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to calculate the area of a feature
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateAreaSample : ContentPage
    {
        public CalculateAreaSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the friscoParks and selectedAreaLayer layers
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

            var friscoParks = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));
            var selectedAreaLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project friscoParks layer to Spherical Mercator to match the map projection
            friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoParks layer
            friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style selectedAreaLayer
            selectedAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
            selectedAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoParks layer to a LayerOverlay
            layerOverlay.Layers.Add("friscoParks", friscoParks);

            // Add selectedAreaLayer to the layerOverlay
            layerOverlay.Layers.Add("selectedAreaLayer", selectedAreaLayer);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            mapView.Refresh();
        }

        /// <summary>
        ///     Calculates the area of a feature selected on the map and displays it in the areaResult TextBox
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var friscoParks = (ShapeFileFeatureLayer) layerOverlay.Layers["friscoParks"];
            var selectedAreaLayer = (InMemoryFeatureLayer) layerOverlay.Layers["selectedAreaLayer"];

            // Query the friscoParks layer to get the first feature closest to the map tap event
            var feature = friscoParks.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Show the selected feature on the map
            selectedAreaLayer.InternalFeatures.Clear();
            selectedAreaLayer.InternalFeatures.Add(feature);
            layerOverlay.Refresh();

            // Get the area of the first feature
            var area = ((AreaBaseShape) feature.GetShape()).GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers);

            // Display the selectedArea's area in the areaResult TextBox
            areaResult.Text = $"{area:f3} sq km";
        }
    }
}