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
    ///     Learn how to use layer query tools to find which features in a layer intersect a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntersectsSample : ContentPage
    {
        public IntersectsSample()
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
            //mapView.CurrentExtent = new RectangleShape(-10781137.28, 3917162.59, -10774579.34, 3911241.35);

            // Create a layer to hold the feature we will perform the spatial query against
            var queryFeatureLayer = new InMemoryFeatureLayer();
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a layer to hold features found by the spatial query
            var highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            var layerOverlay = new LayerOverlay();

            layerOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            layerOverlay.Layers.Add("Query Feature", queryFeatureLayer);
            layerOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);


            mapView.Overlays.Add("Layer Overlay", layerOverlay);

            // Add an event to handle new shapes that are drawn on the map
            mapView.TrackOverlay.TrackEnded += OnPolygonDrawn;

            // Add a sample shape to the map for the initial query
            var sampleShape =
                new PolygonShape(
                    "POLYGON((-10778718.2265634 3914865.63441276,-10778746.8904489 3913709.52436636,-10777103.4943499 3913766.85213726,-10777179.9313777 3914942.07158641,-10778718.2265634 3914865.63441276))");
            await GetFeaturesIntersects(sampleShape);

            // Set the map extent to the sample shapes
            mapView.CurrentExtent = AreaBaseShape.ScaleUp(sampleShape.GetBoundingBox(), 20).GetBoundingBox();

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Perform the 'Intersects' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            // Perform the spatial query on features in the specified layer
            layer.Open();
            var features = layer.QueryTools.GetFeaturesIntersecting(shape, ReturningColumnsType.AllColumns);
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
            txtNumberOfFeaturesFound.Text =
                string.Format("Number of features intersecting the drawn shape: {0}", features.Count());
        }

        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesIntersects(PolygonShape polygon)
        {
            // Find the layers we will be modifying in the MapView
            var layerOverlay = (LayerOverlay) mapView.Overlays["Layer Overlay"];
            var queryFeatureLayer = (InMemoryFeatureLayer) layerOverlay.Layers["Query Feature"];
            var zoningLayer = (ShapeFileFeatureLayer) mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query shape layer and add the newly drawn shape
            queryFeatureLayer.InternalFeatures.Clear();
            queryFeatureLayer.InternalFeatures.Add(new Feature(polygon));
            await layerOverlay.RefreshAsync();

            // Perform the spatial query using the drawn shape and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(polygon, zoningLayer);
            await HighlightQueriedFeatures(queriedFeatures);

            // Disable map drawing and clear the drawn shape
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        ///     Performs the spatial query when a new polygon is drawn
        /// </summary>
        private async void OnPolygonDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            await GetFeaturesIntersects((PolygonShape) e.TrackShape);
        }

        /// <summary>
        ///     Set the map to 'Polygon Drawing Mode' when the user taps on the map without panning
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            if (!(mapView.TrackOverlay.TrackMode == TrackMode.Polygon))
                // Set the drawing mode to 'Polygon'
                mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        }
    }
}