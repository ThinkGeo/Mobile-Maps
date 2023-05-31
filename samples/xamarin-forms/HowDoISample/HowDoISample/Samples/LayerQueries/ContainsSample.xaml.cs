using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find which features in a layer contain a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainsSample : ContentPage
    {
        public ContainsSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco zoning data
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

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a feature layer to hold the Frisco zoning data
            var zoningLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent = new RectangleShape(-10781137.28, 3917162.59, -10774579.34, 3911241.35);

            // Create a layer to hold features found by the spatial query
            var highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            layerOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);

            mapView.Overlays.Add("Layer Overlay", layerOverlay);

            // Add a MarkerOverlay to the map to display the selected point for the query
            var queryFeatureMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("Query Feature Marker Overlay", queryFeatureMarkerOverlay);

            // Add a sample point to the map for the initial query
            var sampleShape = new PointShape(-10779425.2690712, 3914970.73561765);
            await GetFeaturesContaining(sampleShape);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Perform the 'Contains' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            // Perform the spatial query on features in the specified layer
            layer.Open();
            var features = layer.QueryTools.GetFeaturesContaining(shape, ReturningColumnsType.AllColumns);
            layer.Close();

            return features;
        }

        /// <summary>
        ///     Highlight the features that were found by the spatial query
        /// </summary>
        private async Task HighlightQueriedFeatures(IEnumerable<Feature> features)
        {
            // Find the layers we will be modifying in the MapView dictionary
            var layerOverlay = (LayerOverlay) mapView.Overlays["Layer Overlay"];
            var highlightedFeaturesLayer = (InMemoryFeatureLayer) layerOverlay.Layers["Highlighted Features"];

            // Clear the currently highlighted features
            highlightedFeaturesLayer.Open();
            highlightedFeaturesLayer.InternalFeatures.Clear();

            // Add new features to the layer
            foreach (var feature in features) highlightedFeaturesLayer.InternalFeatures.Add(feature);
            highlightedFeaturesLayer.Close();

            // Refresh the overlay so the layer is redrawn
            await layerOverlay.RefreshAsync();

            // Update the number of matching features found in the UI
            txtNumberOfFeaturesFound.Text = $"Number of features containing the drawn shape: {features.Count()}";
        }

        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesContaining(PointShape point)
        {
            // Find the layers we will be modifying in the MapView
            var queryFeatureMarkerOverlay = (SimpleMarkerOverlay) mapView.Overlays["Query Feature Marker Overlay"];
            var zoningLayer = (ShapeFileFeatureLayer) mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query point marker overlay and add a marker on the newly drawn point
            queryFeatureMarkerOverlay.Markers.Clear();

            // Create a marker with a static marker image and add it to the map
            var marker = new Marker(point)
            {
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/AQUA.png"),
                YOffset = -17
            };
            ;
            queryFeatureMarkerOverlay.Markers.Add(marker);
            await queryFeatureMarkerOverlay.RefreshAsync();

            // Perform the spatial query using the drawn point and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(point, zoningLayer);
            await HighlightQueriedFeatures(queriedFeatures);

            // Clear the drawn point
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        ///     Perform the spatial query when a new point is drawn
        /// </summary>
        private async void MapView_OnMapTap(object sender, TouchMapViewEventArgs e)
        {
            await GetFeaturesContaining(e.PointInWorldCoordinate);
        }
    }
}