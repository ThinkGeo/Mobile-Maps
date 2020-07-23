using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to selectively style features using a ValueStyle
    /// </summary>
    public class CreateValueStyleSample : SampleFragment
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

            ShapeFileFeatureLayer friscoCrime = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Frisco_Crime.shp");
            LegendAdornmentLayer legend = new LegendAdornmentLayer();

            // Project the layer's data to match the projection of the map
            friscoCrime.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add friscoCrimeLayer to a LayerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(friscoCrime);

            // Setup the legend adornment
            legend.Title = new LegendItem()
            {
                TextStyle = new TextStyle("Crime Categories", new GeoFont("Verdana", 10, DrawingFontStyles.Bold), GeoBrushes.Black)
            };
            legend.Height = 600;
            legend.Location = AdornmentLocation.LowerRight;
            mapView.AdornmentOverlay.Layers.Add(legend);

            AddValueStyle(friscoCrime, legend);

            // Add layerOverlay to the mapView
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10780196.9469504, 3916119.49665258, -10776231.7761301, 3912703.71697007);
        }

        /// <summary>
        /// Adds a ValueStyle to the friscoCrime layer that represents each OffenseGroup as a different color
        /// </summary>
        private void AddValueStyle(ShapeFileFeatureLayer friscoCrime, LegendAdornmentLayer legend)
        {
            // Get all the distinct OffenseGroups in the friscoCrime data
            friscoCrime.Open();
            var offenseGroups = friscoCrime.FeatureSource.GetDistinctColumnValues("OffenseGro");
            friscoCrime.Close();

            // Create a set of colors to represent each OffenseGroup using a spectrum starting from red
            var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Red, offenseGroups.Count);

            // Create a ValueItem styled with a PointStyle to represent each instance of an OffenseGroup
            var valueItems = new Collection<ValueItem>();
            foreach (var offenseGroup in offenseGroups)
            {
                // Create a PointStyle to represent the OffenseGroup by selecting a color using the index of the OffenseGroup
                var style = PointStyle.CreateSimpleCircleStyle(colors[offenseGroups.IndexOf(offenseGroup)], 10,
                    GeoColors.Black, 2);

                // Create a ValueItem that will house the pointStyle for the OffenseGroup
                valueItems.Add(new ValueItem(offenseGroup.ColumnValue, style));

                // Add a LegendItem to the legend adornment
                var legendItem = new LegendItem()
                {
                    ImageStyle = style,
                    TextStyle = new TextStyle(offenseGroup.ColumnValue, new GeoFont("Verdana", 10), GeoBrushes.Black)
                };
                legend.LegendItems.Add(legendItem);
            }

            // Create the ValueStyle that will use the previously created valueItems to style the data using the OffenseGroup column values
            var valueStyle = new ValueStyle("OffenseGro", valueItems);

            // Add the valueStyle to the friscoCrime layer's CustomStyles and apply the style to all ZoomLevels
            friscoCrime.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            friscoCrime.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}