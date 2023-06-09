using System;
using System.Collections.ObjectModel;
using System.Timers;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     This samples shows how to refresh points on the map based on some outside event
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackDynamicData : ContentPage
    {
        //private bool timerRunning;
        private Timer timer;

        public TrackDynamicData()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Creating a rectangle area we will use to generate the polygons and also start the map there.
            //var currentExtent = MaxExtents.SphericalMercator;mapScale5
            var currentExtent = new RectangleShape(-10810995, 3939081, -10747552, 3884429);

            //Do all the things we need to setup the polygon layer and overlay such as creating all the polygons etc.
            AddPolygonOverlay(AreaBaseShape.ScaleDown(currentExtent.GetBoundingBox(), 80).GetBoundingBox());

            //Set the maps current extent so we start there
            mapView.CurrentExtent = currentExtent;

            mapView.IsRotationEnabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                //I go to find the layer and then loop through all of the features and assign them new
                // random colors and refresh just the overlay that we are using to draw the polygons
                var polygonLayer = (InMemoryFeatureLayer)mapView.FindFeatureLayer("PolygonLayer");
                if (polygonLayer == null)
                    return;

                var random = new Random();

                foreach (var feature in polygonLayer.InternalFeatures)
                    feature.ColumnValues["DataValue"] = random.Next(1, 5).ToString();

                // We are only going to refresh the one overlay that draws the polygons.  This saves us having toe refresh the background data.            
                mapView.RefreshAsync(mapView.Overlays["PolygonOverlay"]).ContinueWith(t =>
                //mapView.Overlays["PolygonOverlay"].RefreshAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                    }
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            timer.Stop();
        }

        private void AddPolygonOverlay(RectangleShape boundingRectangle)
        {
            //We are going to store all of the polygons in an in memory layer
            var polygonLayer = new InMemoryFeatureLayer();
            //polygonLayer.ThreadSafe = ThreadSafetyLevel.Safe;

            //Here we generate all of our make believe polygons
            var features = GetGeneratedPolygons(boundingRectangle.GetBoundingBox());

            //Add all of the polygons to the layer
            foreach (var feature in features) polygonLayer.InternalFeatures.Add(feature);

            //We are going to style them based on their values we randomly added using the column DataValue
            var valueStyle = new ValueStyle();
            valueStyle.ColumnName = "DataValue";

            //Here we add all of the different sub styles so for example "1" is going to be a red semitransparent fill with a black border etc.
            valueStyle.ValueItems.Add(new ValueItem("1",
                new AreaStyle(new GeoPen(GeoColors.Black, 1f), new GeoSolidBrush(new GeoColor(50, GeoColors.Red)))));
            valueStyle.ValueItems.Add(new ValueItem("2",
                new AreaStyle(new GeoPen(GeoColors.Black, 1f), new GeoSolidBrush(new GeoColor(50, GeoColors.Blue)))));
            valueStyle.ValueItems.Add(new ValueItem("3",
                new AreaStyle(new GeoPen(GeoColors.Black, 1f), new GeoSolidBrush(new GeoColor(50, GeoColors.Green)))));
            valueStyle.ValueItems.Add(new ValueItem("4",
                new AreaStyle(new GeoPen(GeoColors.Black, 1f), new GeoSolidBrush(new GeoColor(50, GeoColors.White)))));

            //We add the style we just created to the custom styles of the first zoom level.  Zoom level on is the highest
            // on as such you can see the whole globe.  Zoom level 20 is the lowest street level.
            polygonLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);

            // He we say that whatever is one zoom level 1 is applied all the way to zoom level 20.  If you only wanted to see this style at lower levels
            // you would make the above line start at ZoomLevel15 say and them apply it to 20.  That way once you zoomed out further than 15 the style would no longer apply.
            polygonLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create the overlay to house the layer and add it to the map.
            var polygonOverlay = new LayerOverlay();

            // Here we set the overlay to draw as a single tile.  Alternatively we could draw it as multiples tiles all threaded but single tile is a bit faster
            //We like to use multi tile for slow data sources or very complex ones that may take sime to render as you can start to see data as it comes in.
            polygonOverlay.TileType = TileType.SingleTile;

            polygonOverlay.Layers.Add("PolygonLayer", polygonLayer);
            mapView.Overlays.Add("PolygonOverlay", polygonOverlay);
        }

        private Collection<Feature> GetGeneratedPolygons(RectangleShape boundingRectangle)
        {
            //Here i just created about 20,000 rectangles around the bounding box area and generated random number from 1-4 for their data

            var random = new Random();

            boundingRectangle.ScaleTo(10);

            var features = new Collection<Feature>();

            for (var x = 1; x < 150; x++)
                for (var y = 1; y < 150; y++)
                {
                    var upperLeftX = boundingRectangle.UpperLeftPoint.X + x * boundingRectangle.Width / 150;
                    var upperLeftY = boundingRectangle.UpperLeftPoint.Y - y * boundingRectangle.Height / 150;

                    var lowerRightX = upperLeftX + boundingRectangle.Width / 150;
                    var lowerRightY = upperLeftY - boundingRectangle.Height / 150;

                    var feature = new Feature(new RectangleShape(new PointShape(upperLeftX, upperLeftY),
                        new PointShape(lowerRightX, lowerRightY)));
                    feature.ColumnValues.Add("DataValue", random.Next(1, 5).ToString());

                    features.Add(feature);
                }

            return features;
        }
    }
}