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
    public partial class CalculateAreaSample : ContentPage
    {
        public CalculateAreaSample()
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

            //ShapeFileFeatureLayer friscoParks = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Parks.shp");
            //InMemoryFeatureLayer selectedAreaLayer = new InMemoryFeatureLayer();
            //LayerOverlay layerOverlay = new LayerOverlay();

            //// Project friscoParks layer to Spherical Mercator to match the map projection
            //friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Style friscoParks layer
            //friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            //friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Style selectedAreaLayer
            //selectedAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
            //selectedAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Add friscoParks layer to a LayerOverlay
            //layerOverlay.Layers.Add("friscoParks", friscoParks);

            //// Add selectedAreaLayer to the layerOverlay
            //layerOverlay.Layers.Add("selectedAreaLayer", selectedAreaLayer);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            //// Add LayerOverlay to Map
            //mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        //private void MapView_OnMapClick(object sender, MapClickMapViewEventArgs e)
        //{
        //    LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

        //    ShapeFileFeatureLayer friscoParks = (ShapeFileFeatureLayer)layerOverlay.Layers["friscoParks"];
        //    InMemoryFeatureLayer selectedAreaLayer = (InMemoryFeatureLayer)layerOverlay.Layers["selectedAreaLayer"];

        //    // Query the friscoParks layer to get the first feature closest to the map click event
        //    var feature = friscoParks.QueryTools.GetFeaturesNearestTo(e.WorldLocation, GeographyUnit.Meter, 1,
        //        ReturningColumnsType.NoColumns).First();

        //    // Show the selected feature on the map
        //    selectedAreaLayer.InternalFeatures.Clear();
        //    selectedAreaLayer.InternalFeatures.Add(feature);
        //    layerOverlay.Refresh();

        //    // Get the area of the first feature
        //    var area = ((AreaBaseShape)feature.GetShape()).GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers);

        //    // Display the selectedArea's area in the areaResult TextBox
        //    areaResult.Text = $"{area:f3} sq km";
        //}
    }
}