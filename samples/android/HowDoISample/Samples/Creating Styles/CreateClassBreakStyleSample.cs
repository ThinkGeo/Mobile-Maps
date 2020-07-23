using System;
using System.IO;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using Xamarin.Essentials;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to selectively style features based on numerical data using a ClassBreakStyle
    /// </summary>
    public class CreateClassBreakStyleSample : SampleFragment
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

            ShapeFileFeatureLayer housingUnitsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/Shapefile/Frisco 2010 Census Housing Units.shp"));
            LegendAdornmentLayer legend = new LegendAdornmentLayer();

            // Setup the legend adornment
            legend.Title = new LegendItem()
            {
                TextStyle = new TextStyle("Housing Units", new GeoFont("Verdana", 10, DrawingFontStyles.Bold), GeoBrushes.Black)
            };
            legend.Location = AdornmentLocation.UpperRight;
            mapView.AdornmentOverlay.Layers.Add(legend);

            // Project the layer's data to match the projection of the map
            housingUnitsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            AddClassBreakStyle(housingUnitsLayer, legend);

            // Add housingUnitsLayer to a LayerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(housingUnitsLayer);

            // Add layerOverlay to the mapView
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            housingUnitsLayer.Open();
            mapView.CurrentExtent = housingUnitsLayer.GetBoundingBox();
            housingUnitsLayer.Close();
        }

        /// <summary>
        /// Adds a ClassBreakStyle to the housingUnitsLayer that changes colors based on the numerical value of the H_UNITS column as they fall within the range of a ClassBreak
        /// </summary>
        private void AddClassBreakStyle(ShapeFileFeatureLayer layer, LegendAdornmentLayer legend)
        {
            // Create the ClassBreakStyle based on the H_UNITS numerical column
            var housingUnitsStyle = new ClassBreakStyle("H_UNITS");

            var classBreakIntervals = new double[] { 0, 1000, 2000, 3000, 4000, 5000 };
            var colors = GeoColor.GetColorsInHueFamily(GeoColors.Red, classBreakIntervals.Count()).Reverse().ToList();

            // Create ClassBreaks for each of the classBreakIntervals
            for (int i = 0; i < classBreakIntervals.Count(); i++)
            {
                // Create the classBreak using one of the intervals and colors defined above
                var classBreak = new ClassBreak(classBreakIntervals[i], AreaStyle.CreateSimpleAreaStyle(new GeoColor(192, colors[i]), GeoColors.White));

                // Add the classBreak to the housingUnitsStyle ClassBreaks collection
                housingUnitsStyle.ClassBreaks.Add(classBreak);

                // Add a LegendItem to the legend adornment to represent the classBreak
                var legendItem = new LegendItem()
                {
                    ImageStyle = classBreak.DefaultAreaStyle,
                    TextStyle = new TextStyle($@">{classBreak.Value} units", new GeoFont("Verdana", 10), GeoBrushes.Black)
                };
                legend.LegendItems.Add(legendItem);
            }

            // Add and apply the ClassBreakStyle to the housingUnitsLayer
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(housingUnitsStyle);
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}