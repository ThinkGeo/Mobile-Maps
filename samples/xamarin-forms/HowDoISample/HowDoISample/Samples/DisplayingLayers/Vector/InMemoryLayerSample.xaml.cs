using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display an In-Memory Feature Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InMemoryLayerSample : ContentPage
    {
        public InMemoryLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the feature layer to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            var inMemoryOverlay = new LayerOverlay();
            mapView.Overlays.Add(inMemoryOverlay);

            // Create a new layer that we will pull features from to populate the in memory layer.
            var shapeFileLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco_Mosquitos.shp"));
            shapeFileLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            shapeFileLayer.Open();

            // Get all the features from the above layer.
            var features = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
            shapeFileLayer.Close();

            // Create the in memory layer and add it to the map
            var inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryOverlay.Layers.Add("Frisco Mosquitos", inMemoryFeatureLayer);

            // Loop through all the features in the first layer and add them to the in memeory layer.  We use a shortcut called internal
            // features that is supported in the in memory layer instead of going through the edit tools
            foreach (var feature in features) inMemoryFeatureLayer.InternalFeatures.Add(feature);

            // Create a text style for the label and give it a mask for use below.
            var textStyle = new TextStyle("Trap: [TrapID]", new GeoFont("ariel", 14), GeoBrushes.Black);
            textStyle.Mask = new AreaStyle(GeoPens.Black, GeoBrushes.White);
            textStyle.MaskMargin = new DrawingMargin(2, 2, 2, 2);
            textStyle.YOffsetInPixel = -10;

            // Create an point style and add the text style from above on zoom level 1 and then apply it to all zoom levels up to 20.
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red, GeoPens.White);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Open the layer and set the map view current extent to the bounding box of the layer.
            inMemoryFeatureLayer.Open();
            mapView.CurrentExtent = inMemoryFeatureLayer.GetBoundingBox();

            //Refresh the map.
            mapView.Refresh();
        }
    }
}