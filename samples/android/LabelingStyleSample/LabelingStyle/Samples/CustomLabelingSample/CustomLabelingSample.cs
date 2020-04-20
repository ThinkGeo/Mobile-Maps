using Android.App;
using Android.Content;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    public class CustomLabelingSample : BaseSample
    {
        private CustomLabelingStyleSettingsDialog customLabelingStyleSettingsDialog;

        public CustomLabelingSample(Context context)
            : base(context)
        {
            Title = "Custom Labeling";
        }

        protected override void ApplySettings()
        {
            if (customLabelingStyleSettingsDialog == null)
            {
                customLabelingStyleSettingsDialog = new CustomLabelingStyleSettingsDialog(Context, new CustomLabelingStyleSettings());
                customLabelingStyleSettingsDialog.ApplyingSettings += customLabelingStyleSettingsDialog_ApplyingSettings;
            }
            customLabelingStyleSettingsDialog.Show();
        }

        private void customLabelingStyleSettingsDialog_ApplyingSettings(object sender, EventArgs e)
        {
            LayerOverlay labelingStyleOverlay = MapView.Overlays["CustomLabeling"] as LayerOverlay;
            if (labelingStyleOverlay != null)
            {
                UpdateLabelStylingOverlay(labelingStyleOverlay, customLabelingStyleSettingsDialog.Settings);
                MapView.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = new RectangleShape(-10777472.620674, 3909177.1327916, -10776518.8812938, 3907779.90459956);

            /*===========================================
               Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
               a Client ID and Secret. These were sent to you via email when you signed up
               with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
            ===========================================*/
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            string baseFolder = Application.Context.ExternalCacheDir.AbsolutePath;
            string cachePathFilename = System.IO.Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);
            if (isWriteable) thinkGeoCloudMapsOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename);
            MapView.Overlays.Add("WMK", thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer customLabelingStyleLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs.shp"));
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 7, new GeoSolidBrush(GeoColor.FromHtml("#99cc33")), new GeoPen(GeoColor.FromHtml("#666666"), 1));
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new CustomLabelStyle("Name", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 1);
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = 10;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = 10;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextPlacement = TextPlacement.Center;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            customLabelingStyleLayer.DrawingMarginInPixel = 256;

            LayerOverlay customLabelingOverlay = new LayerOverlay();
            customLabelingOverlay.Layers.Add("customLabeling", customLabelingStyleLayer);
            MapView.Overlays.Add("CustomLabeling", customLabelingOverlay);
        }

        private void UpdateLabelStylingOverlay(LayerOverlay layerOverlay, CustomLabelingStyleSettings settings)
        {
            FeatureLayer featureLayer = (FeatureLayer)layerOverlay.Layers["customLabeling"];
            CustomLabelStyle customLabelStyle = featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle as CustomLabelStyle;
            if (customLabelStyle != null)
            {
                customLabelStyle.MinFontSize = settings.GetMinFontSize();
                customLabelStyle.MaxFontSize = settings.GetMaxFontSize();
            }

            layerOverlay.Refresh();
        }
    }
}