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
    /// Learn how to translate a shape
    /// </summary>
    public class TranslateShapeSample : SampleFragment
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

            ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/Shapefile/FriscoCityLimits.shp"));
            InMemoryFeatureLayer translatedLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style translatedLayer
            translatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            translatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits layer to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add translatedLayer to the layerOverlay
            layerOverlay.Layers.Add("translatedLayer", translatedLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Translates the first feature in the cityLimits layer and displays the result on the map.
        /// </summary>
        private void OffsetTranslateShape_OnClick(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Translate the first feature's shape by the X and Y values on the UI in meters
            //var translate = AreaBaseShape.TranslateByOffset(features[0].GetShape(), Convert.ToDouble(translateX.Text), Convert.ToDouble(translateY.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            // Add the translated shape into translatedLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            // underlying data using BeginTransaction and CommitTransaction instead.
            translatedLayer.InternalFeatures.Clear();
            //translatedLayer.InternalFeatures.Add(new Feature(translate));

            // Redraw the layerOverlay to see the translated feature on the map
            layerOverlay.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DegreeTranslateShape_OnClick(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Translate the first feature's shape by the X and Y values on the UI in meters
            //var translate = AreaBaseShape.TranslateByDegree(features[0].GetShape(), Convert.ToDouble(translateDistance.Text), Convert.ToDouble(translateAngle.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            // Add the translated shape into translatedLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            // underlying data using BeginTransaction and CommitTransaction instead.
            translatedLayer.InternalFeatures.Clear();
            //translatedLayer.InternalFeatures.Add(new Feature(translate));

            // Redraw the layerOverlay to see the translated feature on the map
            layerOverlay.Refresh();
        }
    }
}