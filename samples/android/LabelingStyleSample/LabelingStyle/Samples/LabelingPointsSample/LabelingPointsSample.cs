/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.Content;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

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
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            string baseFolder = Application.Context.ExternalCacheDir.AbsolutePath;
            string cachePathFilename = System.IO.Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);
            if (isWriteable) thinkGeoCloudMapsOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "ThinkGeoCloudMaps");
            MapView.Overlays.Add("WMK", thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer poiLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("Pois.shp"));
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 7, new GeoSolidBrush(GeoColor.FromHtml("#99cc33")), new GeoPen(GeoColor.FromHtml("#666666"), 1));
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("Name", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 2);
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = 0;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = 8;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextPlacement = TextPlacement.Upper;
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

            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextPlacement = settings.Placement;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = settings.GetXOffset();
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = settings.GetYOffset();

            layerOverlay.Refresh();
        }
    }
}
