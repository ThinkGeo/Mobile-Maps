using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to get part of a line from an existing line
    /// </summary>
    public class GetLineOnALineSample : SampleFragment
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

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            InMemoryFeatureLayer railway = new InMemoryFeatureLayer();
            InMemoryFeatureLayer subLineLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Add the rail line feature to the railway layer
            railway.InternalFeatures.Add(new Feature("LineString (-10776730.91861553490161896 3925750.69222266925498843, -10778989.31895966082811356 3915278.00731692276895046, -10781766.12723691388964653 3909228.15506267035380006, -10782065.98029803484678268 3907458.59967381786555052, -10781867.48601813986897469 3905465.21030976390466094)"));

            // Style railway layer
            railway.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
            railway.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the subLineLayer
            subLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            subLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add railway to the layerOverlay
            layerOverlay.Layers.Add("railway", railway);

            // Add subLineLayer to the layerOverlay
            layerOverlay.Layers.Add("subLineLayer", subLineLayer);

            // Set the map extent to the railway layer bounding box
            railway.Open();
            mapView.CurrentExtent = railway.GetBoundingBox();
            railway.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Get a subLine of a line and displays it on the map
        /// </summary>
        private void Button_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            InMemoryFeatureLayer railway = (InMemoryFeatureLayer)layerOverlay.Layers["railway"];
            InMemoryFeatureLayer subLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["subLineLayer"];

            // Query the railway layer to get all the features
            railway.Open();
            var feature = railway.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            railway.Close();

            // Get the subLine from the railway line shape
            //var subLine = ((LineShape)feature.GetShape()).GetLineOnALine(StartingPoint.FirstPoint, Convert.ToDouble(startingOffset.Text), Convert.ToDouble(distance.Text), GeographyUnit.Meter,
            //    DistanceUnit.Meter);

            // Add the subLine into an InMemoryFeatureLayer to display the result.
            subLineLayer.InternalFeatures.Clear();
            //subLineLayer.InternalFeatures.Add(new Feature(subLine));

            // Redraw the layerOverlay to see the subLine on the map
            layerOverlay.Refresh();
        }
    }
}