
/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace LabelingStyle
{
    public class CustomLabelingViewContoller : DetailViewController
    {
        private StyleSettingsController<CustomLabelingStyleSettings> settingsController;

        public CustomLabelingViewContoller(SliderViewController navigation)
            : base(navigation)
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer customLabelingStyleLayer = new ShapeFileFeatureLayer("AppData/POIs.shp");
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromHtml("#99cc33")), new GeoPen(GeoColor.FromHtml("#666666"), 1), 7);
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new CustomLabelStyle("Name", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 1);
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = 10;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.YOffsetInPixel = 10;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            customLabelingStyleLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            customLabelingStyleLayer.DrawingMarginInPixel = 256;

            LayerOverlay customLabelingOverlay = new LayerOverlay();
            customLabelingOverlay.TileHeight = 512;
            customLabelingOverlay.TileWidth = 512;
            customLabelingOverlay.Layers.Add("customLabeling", customLabelingStyleLayer);
            MapView.Overlays.Add("CustomLabeling", customLabelingOverlay);

            MapView.ZoomTo(new PointShape(-10776995.7509839, 3908478.51869558), MapView.ZoomLevelSet.ZoomLevel18.Scale);
        }

        protected override void OnConfigureButtonClicked()
        {
            if (settingsController == null)
            {
                settingsController = new StyleSettingsController<CustomLabelingStyleSettings>();
                settingsController.PreferredContentSize = new CGSize(540, 620);
                settingsController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                settingsController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                settingsController.StyleSettingsChanged += SettingsControllerStyleSettingsChanged;
            }

            PresentViewController(settingsController, true, null);
        }

        private void SettingsControllerStyleSettingsChanged(object sender, StyleSettingsChangedStyleSettingsControllerEventArgs e)
        {
            CustomLabelingStyleSettings settings = (CustomLabelingStyleSettings)e.StyleSettings;
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["CustomLabeling"];
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