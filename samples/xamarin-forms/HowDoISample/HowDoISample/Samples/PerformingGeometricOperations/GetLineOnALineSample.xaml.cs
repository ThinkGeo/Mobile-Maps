﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetLineOnALineSample : ContentPage
    {
        public GetLineOnALineSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
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
        private void GetSubLine_OnClick(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            InMemoryFeatureLayer railway = (InMemoryFeatureLayer)layerOverlay.Layers["railway"];
            InMemoryFeatureLayer subLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["subLineLayer"];

            // Query the railway layer to get all the features
            railway.Open();
            var feature = railway.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            railway.Close();

            // Get the subLine from the railway line shape
            var subLine = ((LineShape)feature.GetShape()).GetLineOnALine(StartingPoint.FirstPoint, Convert.ToDouble(startingOffset.Text), Convert.ToDouble(distance.Text), GeographyUnit.Meter,
                DistanceUnit.Meter);

            // Add the subLine into an InMemoryFeatureLayer to display the result.
            subLineLayer.InternalFeatures.Clear();
            subLineLayer.InternalFeatures.Add(new Feature(subLine));

            // Redraw the layerOverlay to see the subLine on the map
            layerOverlay.Refresh();
        }
    }
}