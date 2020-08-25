using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetFeaturesWithinDistanceSample : ContentPage
    {
        public GetFeaturesWithinDistanceSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create a feature layer to hold the Frisco zoning data
            ShapeFileFeatureLayer zoningLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            ProjectionConverter projectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);

            // Create a layer to hold features found by the spatial query
            InMemoryFeatureLayer highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            LayerOverlay zoningOverlay = new LayerOverlay();
            zoningOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            mapView.Overlays.Add("Frisco Zoning Overlay", zoningOverlay);

            LayerOverlay highlightedFeaturesOverlay = new LayerOverlay();
            highlightedFeaturesOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);
            mapView.Overlays.Add("Highlighted Features Overlay", highlightedFeaturesOverlay);

            // Add a MarkerOverlay to the map to display the selected point for the query
            MarkerOverlay queryFeatureMarkerOverlay = new MarkerOverlay();
            mapView.Overlays.Add("Query Feature Marker Overlay", queryFeatureMarkerOverlay);

            // Add a sample point to the map for the initial query
            PointShape sampleShape = new PointShape(-10779425.2690712, 3914970.73561765);
            GetFeaturesWithinDistance(sampleShape);

            // Set the map extent to the initial area
            mapView.CurrentExtent = new RectangleShape(-10781338.5834248, 3916678.62545891, -10777511.9547176, 3913262.84577639);
        }


        /// <summary>
        /// Perform the 'Get Features Within Distance' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            Collection<Feature> features = new Collection<Feature>();

            // Perform the spatial query on features in the specified layer
            layer.Open();
            features = layer.QueryTools.GetFeaturesWithinDistanceOf(shape, GeographyUnit.Meter, DistanceUnit.Meter, (int)searchRadius.Value, ReturningColumnsType.NoColumns);
            layer.Close();

            return features;
        }

        /// <summary>
        /// Highlight the features that were found by the spatial query
        /// </summary>
        private void HighlightQueriedFeatures(IEnumerable<Feature> features)
        {
            // Find the layers we will be modifying in the MapView dictionary
            LayerOverlay highlightedFeaturesOverlay = (LayerOverlay)mapView.Overlays["Highlighted Features Overlay"];
            InMemoryFeatureLayer highlightedFeaturesLayer = (InMemoryFeatureLayer)highlightedFeaturesOverlay.Layers["Highlighted Features"];

            // Clear the currently highlighted features
            highlightedFeaturesLayer.Open();
            highlightedFeaturesLayer.InternalFeatures.Clear();

            // Add new features to the layer
            foreach (var feature in features)
            {
                highlightedFeaturesLayer.InternalFeatures.Add(feature);
            }
            highlightedFeaturesLayer.Close();

            // Refresh the overlay so the layer is redrawn
            highlightedFeaturesOverlay.Refresh();

            // Update the number of matching features found in the UI
            txtNumberOfFeaturesFound.Text = $"Number of features within distance of the drawn shape: {features.Count()}";
        }

        private void GetFeaturesWithinDistance(PointShape point)
        {
            // Find the layers we will be modifying in the MapView
            MarkerOverlay queryFeatureMarkerOverlay = (MarkerOverlay)mapView.Overlays["Query Feature Marker Overlay"];
            ShapeFileFeatureLayer zoningLayer = (ShapeFileFeatureLayer)mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query point marker overlaylayer and add a marker on the newly drawn point
            queryFeatureMarkerOverlay.Markers.Clear();
            queryFeatureMarkerOverlay.Markers.Add(CreateNewMarker(point));
            queryFeatureMarkerOverlay.Refresh();

            // Perform the spatial query using the drawn point and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(point, zoningLayer);
            HighlightQueriedFeatures(queriedFeatures);

            // Disable map drawing and clear the drawn point
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        /// Perform the spatial query when a new point is drawn
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            GetFeaturesWithinDistance(e.PointInWorldCoordinate);
        }

        /// <summary>
        /// Create a new map marker using preloaded image assets
        /// </summary>
        private Marker CreateNewMarker(PointShape point)
        {
            return new Marker()
            {
                Position = point,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "/Resources/AQUA.png"),
                YOffset = -17
            };
        }
    }
}
