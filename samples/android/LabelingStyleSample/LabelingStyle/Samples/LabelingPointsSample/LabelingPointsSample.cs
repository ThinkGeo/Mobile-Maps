﻿/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.Content;
using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelingPointsSample : BaseSample
    {
        private LabelingPointsSettingsDialog labelingPointsSettingsDialog;

        public LabelingPointsSample(Context context)
            : base(context)
        {
            Title = "Labeling Points";
        }

        protected override void ApplySettings()
        {
            if (labelingPointsSettingsDialog == null)
            {
                labelingPointsSettingsDialog = new LabelingPointsSettingsDialog(Context, new LabelingPointsStyleSettings());
                labelingPointsSettingsDialog.ApplyingSettings += labelingPointsSettingsDialog_ApplyingSettings;
            }

            labelingPointsSettingsDialog.Show();
        }

        private void labelingPointsSettingsDialog_ApplyingSettings(object sender, EventArgs e)
        {
            LayerOverlay labelingPointsOverlay = MapView.Overlays["LabelingPoints"] as LayerOverlay;
            if (labelingPointsOverlay != null)
            {
                UpdateLabelingPointsOverlay(labelingPointsOverlay, labelingPointsSettingsDialog.Settings);
                MapView.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = new RectangleShape(-10777472.620674, 3909177.1327916, -10776518.8812938, 3907779.90459956);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            MapView.Overlays.Add("WMK", thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer poiLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("Pois.shp"));
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromHtml("#99cc33")), new GeoPen(GeoColor.FromHtml("#666666"), 1), 7);
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("Name", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = 0;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = 8;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.PointPlacement = PointPlacement.UpperCenter;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            poiLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            poiLayer.DrawingMarginInPixel = 256;

            LayerOverlay labelingPointsOverlay = new LayerOverlay();
            labelingPointsOverlay.Layers.Add("poi", poiLayer);
            MapView.Overlays.Add("LabelingPoints", labelingPointsOverlay);
        }

        private void UpdateLabelingPointsOverlay(LayerOverlay layerOverlay, LabelingPointsStyleSettings settings)
        {
            FeatureLayer poiLayer = (FeatureLayer)layerOverlay.Layers["poi"];

            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.PointPlacement = settings.Placement;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = settings.GetXOffset();
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = settings.GetYOffset();

            layerOverlay.Refresh();
        }
    }
}