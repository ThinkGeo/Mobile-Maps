﻿using System;
using System.Collections.ObjectModel;
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
    /// Learn how to clip a shape from another shape
    /// </summary>
    public class ClipShapeSample : SampleFragment
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
            InMemoryFeatureLayer westRegionLayer = new InMemoryFeatureLayer();
            InMemoryFeatureLayer clipLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style cityLimits layer
            westRegionLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Blue), GeoColors.DimGray);
            westRegionLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the clipLayer
            clipLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
            clipLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add cityLimits to a LayerOverlay
            layerOverlay.Layers.Add("westRegionLayer", westRegionLayer);

            // Add clipLayer to the layerOverlay
            layerOverlay.Layers.Add("clipLayer", clipLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            // Add west region area to westRegionLayer
            westRegionLayer.InternalFeatures.Add(new Feature("Polygon ((-10780139.10763415694236755 3918539.43726690439507365, -10780206.68015497177839279 3915600.03261143481358886, -10780088.42824354581534863 3914687.80358042707666755, -10780037.7488529346883297 3913978.2921118657104671, -10779767.4587696734815836 3913555.96385676926001906, -10779176.19921253807842731 3913336.35316411964595318, -10778635.61904601566493511 3913049.16995065379887819, -10778280.86331173405051231 3911934.22335720015689731, -10778263.970181530341506 3910684.13172211544588208, -10778382.22209295630455017 3910447.6278992616571486, -10778382.22209295630455017 3909569.1851286618039012, -10778263.970181530341506 3909113.07061315793544054, -10778280.86331173405051231 3907356.18507195776328444, -10785595.58868999965488911 3904045.13155200378969312, -10786034.81007529981434345 3904822.21554138045758009, -10786001.02381489239633083 3908150.16219153860583901, -10786051.70320550352334976 3908690.7423580614849925, -10785933.45129407569766045 3909315.78817560384050012, -10786001.02381489239633083 3911275.39127924991771579, -10785882.77190346457064152 3912204.5134404618293047, -10785832.09251285344362259 3914485.08601798117160797, -10785832.09251285344362259 3917728.56701711937785149, -10785426.65738796070218086 3918117.10901180794462562, -10785460.44364836812019348 3919012.44491261197254062, -10782233.85577943362295628 3918995.55178240826353431, -10781642.59622230008244514 3918623.90291792340576649, -10780139.10763415694236755 3918539.43726690439507365))"));
        }

        /// <summary>
        /// Clips the cityLimits and westRegion areas and displays the results on the map
        /// </summary>
        private void Button_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer westRegionLayer = (InMemoryFeatureLayer)layerOverlay.Layers["westRegionLayer"];
            InMemoryFeatureLayer clipLayer = (InMemoryFeatureLayer)layerOverlay.Layers["clipLayer"];

            // Query the cityLimits layer to get the first feature
            cityLimits.Open();
            var feature = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            cityLimits.Close();

            // Get the westRegion area
            var westRegion = westRegionLayer.InternalFeatures[0];

            // Clips the cityLimits feature down to the common area of the westRegion
            var clip = feature.GetIntersection(westRegion);

            // Add the clip into an InMemoryFeatureLayer to display the result.
            clipLayer.InternalFeatures.Clear();
            clipLayer.InternalFeatures.Add(clip);

            // Redraw the layerOverlay to see the clip feature on the map
            layerOverlay.Refresh();
        }
    }
}