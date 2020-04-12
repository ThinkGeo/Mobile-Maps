/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace LabelingStyle
{
    public class LabelingPointsViewContoller : DetailViewController
    {
        private StyleSettingsController<LabelingPointsStyleSettings> settingsController;

        public LabelingPointsViewContoller(SliderViewController navigation)
            : base(navigation)
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer poiLayer = new ShapeFileFeatureLayer("AppData/POIs.shp");
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
            labelingPointsOverlay.TileHeight = 512;
            labelingPointsOverlay.TileWidth = 512;
            labelingPointsOverlay.Layers.Add("poi", poiLayer);
            MapView.Overlays.Add("LabelingPoints", labelingPointsOverlay);

            MapView.ZoomTo(new PointShape(-10776995.7509839, 3908478.51869558), MapView.ZoomLevelSet.ZoomLevel18.Scale);
        }

        protected override void OnConfigureButtonClicked()
        {
            if (settingsController == null)
            {
                settingsController = new StyleSettingsController<LabelingPointsStyleSettings>();
                settingsController.PreferredContentSize = new CGSize(540, 620);
                settingsController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                settingsController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                settingsController.StyleSettingsChanged += SettingsControllerStyleSettingsChanged;
            }

            PresentViewController(settingsController, true, null);
        }

        private void SettingsControllerStyleSettingsChanged(object sender, StyleSettingsChangedStyleSettingsControllerEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LabelingPoints"];
            FeatureLayer poiLayer = (FeatureLayer)layerOverlay.Layers["poi"];

            LabelingPointsStyleSettings settings = (LabelingPointsStyleSettings)e.StyleSettings;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextPlacement = settings.Placement;
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = settings.GetXOffset();
            poiLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = settings.GetYOffset();

            layerOverlay.Refresh();
        }
    }
}