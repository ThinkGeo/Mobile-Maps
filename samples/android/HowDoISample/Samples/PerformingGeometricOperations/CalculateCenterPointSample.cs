﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to calculate the center point of a feature
    /// </summary>
    public class CalculateCenterPointSample : SampleFragment
    {
        private TextView instructions;
        private RadioButton centroidCenter;
        private RadioButton bboxCenter;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

            instructions = new TextView(this.Context)
            {
                Text = "Tap a feature on the map to display it's center point."
            };

            centroidCenter = new RadioButton(this.Context)
            {
                Text = "Show Centroid Center"
            };
            centroidCenter.Click += RadioButton_Checked;

            bboxCenter = new RadioButton(this.Context)
            {
                Text = "Show Bounding Box Center"
            };
            bboxCenter.Click += RadioButton_Checked;

            var radioGroup = new RadioGroup(this.Context);
            radioGroup.AddView(centroidCenter);
            radioGroup.AddView(bboxCenter);

            var gridLayout = new GridLayout(this.Context)
            {
                RowCount = 2,
                ColumnCount = 1
            };
            gridLayout.AddView(instructions, new GridLayout.LayoutParams(GridLayout.InvokeSpec(0), GridLayout.InvokeSpec(0, 1f)));
            gridLayout.AddView(radioGroup, new GridLayout.LayoutParams(GridLayout.InvokeSpec(1), GridLayout.InvokeSpec(0, 1f)));

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { gridLayout });
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            ShapeFileFeatureLayer censusHousing = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Frisco 2010 Census Housing Units.shp");
            InMemoryFeatureLayer centerPointLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project censusHousing layer to Spherical Mercator to match the map projection
            censusHousing.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style censusHousing layer
            censusHousing.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            censusHousing.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style centerPointLayer
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 12, GeoColors.White, 4);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.Black, 2);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add censusHousing layer to a LayerOverlay
            layerOverlay.Layers.Add("censusHousing", censusHousing);

            // Add centerPointLayer to the layerOverlay
            layerOverlay.Layers.Add("centerPointLayer", centerPointLayer);

            // Set the map extent to the censusHousing layer bounding box
            censusHousing.Open();
            mapView.CurrentExtent = censusHousing.GetBoundingBox();
            censusHousing.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            centroidCenter.Checked = true;
        }

        /// <summary>
        /// Calculates the center point of a feature
        /// </summary>
        /// <param name="feature"> The target feature to calculate it's center point</param>
        private void CalculateCenterPoint(Feature feature)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer centerPointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["centerPointLayer"];

            PointShape centerPoint;

            // Get the CenterPoint of the selected feature
            if (centroidCenter.Checked)
            {
                // Centroid, or geometric center, method. Accurate, but can be relatively slower on extremely complex shapes
                centerPoint = feature.GetShape().GetCenterPoint();
            }
            else
            {
                // BoundingBox method. Less accurate, but much faster
                centerPoint = feature.GetBoundingBox().GetCenterPoint();
            }

            // Show the centerPoint on the map
            centerPointLayer.InternalFeatures.Clear();
            centerPointLayer.InternalFeatures.Add("selectedFeature", feature);
            centerPointLayer.InternalFeatures.Add("centerPoint", new Feature(centerPoint));

            // Refresh the overlay to show the results
            layerOverlay.Refresh();

        }

        /// <summary>
        /// Map event that fires whenever the user clicks on the map. Gets the closest feature from the click event and calculates the center point
        /// </summary>
        private void MapView_OnMapClick(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            ShapeFileFeatureLayer censusHousing = (ShapeFileFeatureLayer)layerOverlay.Layers["censusHousing"];

            // Query the censusHousing layer to get the first feature closest to the map click event
            var feature = censusHousing.QueryTools.GetFeaturesNearestTo(e.WorldPoint, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            CalculateCenterPoint(feature);
        }

        /// <summary>
        /// RadioButton checked event that will recalculate the center point so long as a feature was already selected
        /// </summary>
        private void RadioButton_Checked(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer centerPointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["centerPointLayer"];

            // Recalculate the center point if a feature has already been selected
            if (centerPointLayer.InternalFeatures.Contains("selectedFeature"))
            {
                CalculateCenterPoint(centerPointLayer.InternalFeatures["selectedFeature"]);
            }
        }
    }
}