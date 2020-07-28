﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateLengthSample : ContentPage
    {
        public CalculateLengthSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //ShapeFileFeatureLayer friscoTrails = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Hike_Bike.shp");
            //InMemoryFeatureLayer selectedLineLayer = new InMemoryFeatureLayer();
            //LayerOverlay layerOverlay = new LayerOverlay();

            //// Project friscoTrails layer to Spherical Mercator to match the map projection
            //friscoTrails.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Style friscoTrails layer
            //friscoTrails.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Orange, 2, false);
            //friscoTrails.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Style selectedLineLayer
            //selectedLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            //selectedLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Add friscoTrails layer to a LayerOverlay
            //layerOverlay.Layers.Add("friscoTrails", friscoTrails);

            //// Add selectedLineLayer to the layerOverlay
            //layerOverlay.Layers.Add("selectedLineLayer", selectedLineLayer);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            //// Add LayerOverlay to Map
            //mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Calculates the length of a line selected on the map and displays it in the lengthResult TextBox
        /// </summary>
        //private void MapView_OnMapClick(object sender, MapClickMapViewEventArgs e)
        //{
        //    LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

        //    ShapeFileFeatureLayer friscoTrails = (ShapeFileFeatureLayer)layerOverlay.Layers["friscoTrails"];
        //    InMemoryFeatureLayer selectedLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["selectedLineLayer"];

        //    // Query the friscoTrails layer to get the first feature closest to the map click event
        //    var feature = friscoTrails.QueryTools.GetFeaturesNearestTo(e.WorldLocation, GeographyUnit.Meter, 1,
        //        ReturningColumnsType.NoColumns).First();

        //    // Show the selected feature on the map
        //    selectedLineLayer.InternalFeatures.Clear();
        //    selectedLineLayer.InternalFeatures.Add(feature);
        //    layerOverlay.Refresh();

        //    // Get the length of the first feature
        //    var length = ((LineBaseShape)feature.GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

        //    // Display the selectedLine's length in the lengthResult TextBox
        //    lengthResult.Text = $"{length:f3} km";
        //}
    }
}