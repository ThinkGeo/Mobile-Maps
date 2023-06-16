using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find features in a layer within a given distance of a point
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetFeaturesWithinDistanceSample : ContentPage
    {
        public GetFeaturesWithinDistanceSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco zoning data
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);
            
            // Create a feature layer to hold the Frisco zoning data
            var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);
            
            var friscoOverlay = new LayerOverlay();
            friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);
            mapView.Overlays.Add("FriscoOverlay", friscoOverlay);

            // Create a layer to hold features found by the spatial query
            var highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightedFeaturesLayer);
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            // Add a MarkerOverlay to the map to display the selected point for the query
            var markerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("MarkerOverlay", markerOverlay);

            // Add a sample point to the map for the initial query
            var sampleShape = new PointShape(-10779425.2690712, 3914970.73561765);
            await GetFeaturesWithinDistance(sampleShape);

            // Set the map extent to the initial area
            mapView.CurrentExtent = new RectangleShape(-10781338.5834248, 3916678.62545891, -10777511.9547176, 3913262.84577639);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesWithinDistance(PointShape point)
        {
            // Find the layers we will be modifying in the MapView
            var markerOverlay = (SimpleMarkerOverlay) mapView.Overlays["MarkerOverlay"];
            var friscoLayer = (ShapeFileFeatureLayer) mapView.FindFeatureLayer("FriscoLayer");

            // Clear the query point marker overlay and add a marker on the newly drawn point
            markerOverlay.Markers.Clear();
            markerOverlay.Markers.Add(CreateNewMarker(point));
            await markerOverlay.RefreshAsync();

            // Perform the spatial query on features in the specified layer
            friscoLayer.Open();
            var queriedFeatures = friscoLayer.QueryTools.GetFeaturesWithinDistanceOf(point, GeographyUnit.Meter, DistanceUnit.Meter,
                (int)searchRadius.Value, ReturningColumnsType.NoColumns);

            await HighlightQueriedFeatures(queriedFeatures);
            
            // Disable map drawing and clear the drawn point
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        ///     Highlight the features that were found by the spatial query
        /// </summary>
        private async Task HighlightQueriedFeatures(IEnumerable<Feature> features)
        {
            // Find the layers we will be modifying in the MapView dictionary
            var highlightOverlay = (LayerOverlay)mapView.Overlays["HighlightOverlay"];
            var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            // Clear the currently highlighted features
            highlightLayer.Open();
            highlightLayer.InternalFeatures.Clear();

            // Add new features to the layer
            foreach (var feature in features) highlightLayer.InternalFeatures.Add(feature);

            // Refresh the overlay so the layer is redrawn
            await highlightOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Perform the spatial query when a new point is drawn
        /// </summary>
        private async void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            await GetFeaturesWithinDistance(e.PointInWorldCoordinate);
        }

        /// <summary>
        ///     Create a new map marker using preloaded image assets
        /// </summary>
        private Marker CreateNewMarker(PointShape point)
        {
            return new Marker
            {
                Position = point,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/AQUA.png"),
                YOffset = -17
            };
        }
    }
}