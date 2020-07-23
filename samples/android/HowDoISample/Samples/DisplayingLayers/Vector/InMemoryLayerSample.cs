using System;
using System.Collections.ObjectModel;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using Xamarin.Essentials;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display an in memory layer.
    /// </summary>
    public class InMemoryLayerSample : SampleFragment
    {
        // Controls
        private MapView mapView;

        /// <summary>
        /// Defines the Layout to use from the `Resources/layout` directory
        /// </summary>
        public override int Layout => Resource.Layout.__SampleTemplate;

        /// <summary>
        /// Creates the sample view from the Layout resource and exposes controls from the view that needs to be 
        /// referenced for the sample to run (mapView, buttons, etc.)
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Call the base OnCreateView method to inflate the Layout with basic functionality
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            // Bind the controls needed from the Layout to the class
            mapView = view.FindViewById<MapView>(Resource.Id.mapView);

            return view;
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay inMemoryOverlay = new LayerOverlay();
            mapView.Overlays.Add(inMemoryOverlay);

            // Create a new layer that we will pull features from to populate the in memory layer.
            ShapeFileFeatureLayer shapeFileLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/Shapefile/Frisco_Mosquitos.shp"));
            shapeFileLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            shapeFileLayer.Open();

            // Get all the features from the above layer.
            Collection<Feature> features = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
            shapeFileLayer.Close();

            // Create the in memory layer and add it to the map
            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryOverlay.Layers.Add("Frisco Mosquitos", inMemoryFeatureLayer);

            // Loop through all the features in the first layer and add them to the in memeory layer.  We use a shortcut called internal 
            // features that is supported in the in memory layer instead of going through the edit tools
            foreach (Feature feature in features)
            {
                inMemoryFeatureLayer.InternalFeatures.Add(feature);
            }

            // Create a text style for the label and give it a mask for use below.
            TextStyle textStyle = new TextStyle("Trap: [TrapID]", new GeoFont("ariel", 14), GeoBrushes.Black);
            textStyle.Mask = new AreaStyle(GeoPens.Black, GeoBrushes.White);
            textStyle.MaskMargin = new DrawingMargin(2, 2, 2, 2);
            textStyle.YOffsetInPixel = -7;

            // Create an point style and add the text style from above on zoom level 1 and then apply it to all zoom levels up to 20.            
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red, GeoPens.White);
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