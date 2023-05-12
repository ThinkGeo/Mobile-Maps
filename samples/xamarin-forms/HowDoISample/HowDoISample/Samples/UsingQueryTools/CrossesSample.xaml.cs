using System;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find which features in a layer a shape crosses
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrossesSample
    {
        public CrossesSample()
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

            // Add Cloud Maps as a background overlay
            var background = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            background.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(background);

            // Create a feature layer to hold the Frisco zoning data
            var friscoLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shape file from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            friscoLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            friscoLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
            friscoLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a layer to hold the feature we will perform the spatial query against, Create a layer to hold features found by the spatial query
            var highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Red, 6, false);
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var friscoOverlay = new LayerOverlay();
            mapView.Overlays.Add("FriscoOverlay", friscoOverlay);
            friscoOverlay.Layers.Add("FriscoLayer", friscoLayer);

            var highlightOverlay = new LayerOverlay
            {
                TileType = TileType.SingleTile
            };
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            // Add an event to handle new shapes that are drawn on the map
            mapView.TrackOverlay.TrackEnded += OnLineDrawn;

            // Add a sample shape to the map for the initial query
            var sampleShape = new LineShape("LINESTRING(-10774628 3914024,-10776902 3915582,-10778030 3914368,-10778708 3914445)");

            await GetFeaturesCrossingAsync(sampleShape);

            // Set the map extent to the sample shapes
            mapView.CurrentExtent = AreaBaseShape.ScaleUp(sampleShape.GetBoundingBox(), 20).GetBoundingBox();

            mapView.TrackOverlay.TrackMode = TrackMode.Line;

            await mapView.RefreshAsync();
        }


        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesCrossingAsync(BaseShape shape)
        {
            // Find the layers we will be modifying in the MapView
            var highlightOverlay = (LayerOverlay)mapView.Overlays["HighlightOverlay"];
            var highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            var friscoOverlay = (LayerOverlay)mapView.Overlays["FriscoOverlay"];
            var friscoLayer = (FeatureLayer)friscoOverlay.Layers["FriscoLayer"];

            // Clear the query shape layer and add the newly drawn shape
            highlightLayer.InternalFeatures.Clear();
            highlightLayer.InternalFeatures.Add(new Feature(shape));

            // Perform the spatial query using the drawn shape 
            friscoLayer.Open();
            var queriedFeatures = friscoLayer.QueryTools.GetFeaturesCrossing(shape, ReturningColumnsType.AllColumns);

            foreach (var feature in queriedFeatures) 
                highlightLayer.InternalFeatures.Add(feature);

            // Highlight the found features
            await highlightOverlay.RefreshAsync();

            // Disable map drawing and clear the drawn shape
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

            txtNumberOfFeaturesFound.Text = $"Number of features crossing the drawn shape: {queriedFeatures.Count}";
        }

        /// <summary>
        ///     Performs the spatial query when a new line is drawn
        /// </summary>
        private async void OnLineDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            await GetFeaturesCrossingAsync(e.TrackShape);
        }
    }
}