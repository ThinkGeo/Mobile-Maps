using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find which features in a layer intersect a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntersectsSample
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

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a feature layer to hold the Frisco zoning data
            var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,2);
                     
            // Create a layer to hold the feature we will perform the spatial query against
            var queryLayer = new InMemoryFeatureLayer();
            queryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
            queryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a layer to hold features found by the spatial query
            var highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            var friscoOverlay = new LayerOverlay();
            mapView.Overlays.Add("FriscoOverlay", friscoOverlay);
            friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);

            var highlightOverlay = new LayerOverlay { TileType = TileType.SingleTile };
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            highlightOverlay.Layers.Add("QueryLayer", queryLayer);
            
            // Add an event to handle new shapes that are drawn on the map
            mapView.TrackOverlay.TrackEnded += OnPolygonDrawn;

            // Add a sample shape to the map for the initial query
            var sampleShape = new PolygonShape("POLYGON((-10778718 3914865,-10778746 3913709,-10777103 3913766,-10777179 3914942,-10778718 3914865))");

            await GetFeaturesIntersects(sampleShape);

            // Set the map extent to the sample shapes
            mapView.CurrentExtent = AreaBaseShape.ScaleUp(sampleShape.GetBoundingBox(), 20).GetBoundingBox();

            mapView.TrackOverlay.TrackMode = TrackMode.Polygon;

            await mapView.RefreshAsync();
        } 

        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesIntersects(PolygonShape polygon)
        {
            // Find the layers we will be modifying in the MapView
            var highlightOverlay = (LayerOverlay) mapView.Overlays["HighlightOverlay"];
            var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];
            var queryLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["QueryLayer"];

            var friscoOverlay = (LayerOverlay)mapView.Overlays["FriscoOverlay"];
            var friscoLayer = (FeatureLayer)friscoOverlay.Layers["FriscoLayer"];

            // Clear the query shape layer and add the newly drawn shape
            queryLayer.InternalFeatures.Clear();
            queryLayer.InternalFeatures.Add(new Feature(polygon));

            // Perform the spatial query using the drawn shape            
            friscoLayer.Open();
            var queriedFeatures = friscoLayer.QueryTools.GetFeaturesIntersecting(polygon, ReturningColumnsType.AllColumns);

            highlightLayer.InternalFeatures.Clear();

            foreach (var feature in queriedFeatures)
                highlightLayer.InternalFeatures.Add(feature);

            // Highlight the found features
            //TODO: do not refresh automatically
            await highlightOverlay.RefreshAsync();

            // Clear the drawn shape            
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
            await mapView.TrackOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Performs the spatial query when a new polygon is drawn
        /// </summary>
        private async void OnPolygonDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            await GetFeaturesIntersects((PolygonShape) e.TrackShape);
        }
          
    }
}