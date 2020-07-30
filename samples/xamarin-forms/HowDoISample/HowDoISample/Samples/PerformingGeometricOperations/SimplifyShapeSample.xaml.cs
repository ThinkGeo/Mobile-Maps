﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimplifyShapeSample : ContentPage
    {
        public SimplifyShapeSample()
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

            //ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/FriscoCityLimits.shp");
            //InMemoryFeatureLayer simplifyLayer = new InMemoryFeatureLayer();
            //LayerOverlay layerOverlay = new LayerOverlay();

            //// Project cityLimits layer to Spherical Mercator to match the map projection
            //cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Style cityLimits layer
            //cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            //cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Style simplifyLayer
            //simplifyLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            //simplifyLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Add cityLimits layer to a LayerOverlay
            //layerOverlay.Layers.Add("cityLimits", cityLimits);

            //// Add simplifyLayer to the layerOverlay
            //layerOverlay.Layers.Add("simplifyLayer", simplifyLayer);

            //// Set the map extent to the cityLimits layer bounding box
            //cityLimits.Open();
            //mapView.CurrentExtent = cityLimits.GetBoundingBox();
            //cityLimits.Close();

            //// Add LayerOverlay to Map
            //mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        private void SimplifyShape_OnClick(object sender, EventArgs e)
        {
            //LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            //ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            //InMemoryFeatureLayer simplifyLayer = (InMemoryFeatureLayer)layerOverlay.Layers["simplifyLayer"];

            //// Query the cityLimits layer to get all the features
            //cityLimits.Open();
            //var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            //cityLimits.Close();

            //// Simplify the first feature using the Douglas Peucker method
            //var simplify = AreaBaseShape.Simplify(features[0].GetShape() as AreaBaseShape, Convert.ToInt32(tolerance.Text), SimplificationType.DouglasPeucker);

            //// Add the simplified shape into simplifyLayer to display the result.
            //// If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            //simplifyLayer.InternalFeatures.Clear();
            //simplifyLayer.InternalFeatures.Add(new Feature(simplify));

            //// Redraw the layerOverlay to see the simplified feature on the map
            //layerOverlay.Refresh();
        }
    }
}