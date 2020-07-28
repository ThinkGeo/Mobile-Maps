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
    public partial class InMemoryLayerSample : ContentPage
    {
        public InMemoryLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// It is important to set the map unit first to either feet, meters or decimal degrees.
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            //ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay inMemoryOverlay = new LayerOverlay();
            //mapView.Overlays.Add(inMemoryOverlay);

            //// Create a new layer that we will pull features from to populate the in memory layer.
            //ShapeFileFeatureLayer shapeFileLayer = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Frisco_Mosquitos.shp");
            //shapeFileLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            //shapeFileLayer.Open();

            //// Get all the features from the above layer.
            //Collection<Feature> features = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
            //shapeFileLayer.Close();

            //// Create the in memory layer and add it to the map
            //InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            //inMemoryOverlay.Layers.Add("Frisco Mosquitos", inMemoryFeatureLayer);

            //// Loop through all the features in the first layer and add them to the in memeory layer.  We use a shortcut called internal 
            //// features that is supported in the in memory layer instead of going through the edit tools
            //foreach (Feature feature in features)
            //{
            //    inMemoryFeatureLayer.InternalFeatures.Add(feature);
            //}

            //// Create a text style for the label and give it a mask for use below.
            //TextStyle textStyle = new TextStyle("Trap: [TrapID]", new GeoFont("ariel", 14), GeoBrushes.Black);
            //textStyle.Mask = new AreaStyle(GeoPens.Black, GeoBrushes.White);
            //textStyle.MaskMargin = new DrawingMargin(2, 2, 2, 2);
            //textStyle.YOffsetInPixel = -10;

            //// Create an point style and add the text style from above on zoom level 1 and then apply it to all zoom levels up to 20.            
            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red, GeoPens.White);
            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Open the layer and set the map view current extent to the bounding box of the layer.  
            //inMemoryFeatureLayer.Open();
            //mapView.CurrentExtent = inMemoryFeatureLayer.GetBoundingBox();

            ////Refresh the map.
            //mapView.Refresh();
        }
    }
}