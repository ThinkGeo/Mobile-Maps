using System;
using System.Collections.ObjectModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to label data using a TextStyle
    /// </summary>
    public class CreateTextStyleSample : SampleFragment
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

            // Set the map background color
            mapView.Background = new ColorDrawable(Color.Argb(255, 234, 232, 226));

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Hotels.shp");
            ShapeFileFeatureLayer streetsLayer = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Streets.shp");
            ShapeFileFeatureLayer parksLayer = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Parks.shp");

            // Project the layer's data to match the projection of the map
            hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            streetsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add Styles to the layers
            StyleHotelsLayer(hotelsLayer);
            StyleStreetsLayer(streetsLayer);
            StyleParksLayer(parksLayer);

            // Add layers to a layerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(parksLayer);
            layerOverlay.Layers.Add(streetsLayer);
            layerOverlay.Layers.Add(hotelsLayer);

            // Add overlay to map
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            mapView.Refresh();
        }

        /// <summary>
        /// Adds a PointStyle and TextStyle to the Hotels Layer
        /// </summary>
        private void StyleHotelsLayer(ShapeFileFeatureLayer hotelsLayer)
        {
            var pointStyle = new PointStyle(PointSymbolType.Circle, 4, GeoBrushes.Brown, new GeoPen(GeoBrushes.DarkRed, 2));
            var textStyle = new TextStyle("NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold), GeoBrushes.DarkRed)
            {
                TextPlacement = TextPlacement.Lower,
                YOffsetInPixel = 2,
                HaloPen = new GeoPen(GeoBrushes.White, 2),
                DrawingLevel = DrawingLevel.LabelLevel,
                AllowLineCarriage = true
            };
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);

            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        /// <summary>
        /// Adds a LineStyle and TextStyle to the Streets Layer
        /// </summary>
        private void StyleStreetsLayer(ShapeFileFeatureLayer streetsLayer)
        {
            var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));
            var textStyle = new TextStyle("FULL_NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold), GeoBrushes.MidnightBlue)
            {
                SplineType = SplineType.StandardSplining,
                HaloPen = new GeoPen(GeoBrushes.White, 2),
                DrawingLevel = DrawingLevel.LabelLevel
            };
            streetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(lineStyle);
            streetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);

            streetsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        /// <summary>
        /// Adds an AreaStyle and TextStyle to the Parks Layer
        /// </summary>
        private void StyleParksLayer(ShapeFileFeatureLayer parksLayer)
        {
            var areaStyle = new AreaStyle(GeoPens.DimGray, GeoBrushes.PastelGreen);
            var textStyle = new TextStyle("NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold), GeoBrushes.DarkGreen)
            {
                FittingPolygon = false,
                HaloPen = new GeoPen(GeoBrushes.White, 2),
                DrawingLevel = DrawingLevel.LabelLevel,
                AllowLineCarriage = true
            };
            parksLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(areaStyle);
            parksLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);

            parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}