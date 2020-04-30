using CoreGraphics;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace LabelingStyle
{
    public class LabelingPolygonsViewContoller : DetailViewController
    {
        private StyleSettingsController<LabelingPolygonsStyleSettings> settingsController;

        public LabelingPolygonsViewContoller(SliderViewController navigation)
            : base(navigation)
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();

            MapView.MapUnit = GeographyUnit.Meter;

            WkbFileFeatureLayer subdivisionsLayer = new WkbFileFeatureLayer("AppData/WkbFiles/Subdivisions.wkb");
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.White, GeoColor.FromHtml("#9C9C9C"), 1);
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("NAME_COMMO", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 1);
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 3000;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay labelingPolygonsOverlay = new LayerOverlay();
            labelingPolygonsOverlay.TransitionEffect = TransitionEffect.None;
            labelingPolygonsOverlay.Layers.Add("subdivision", subdivisionsLayer);
            MapView.Overlays.Add("LabelingPolygons", labelingPolygonsOverlay);

            MapView.ZoomTo(new PointShape(-10776995.7509839, 3908478.51869558), MapView.ZoomLevelSet.ZoomLevel17.Scale);
        }

        protected override void OnConfigureButtonClicked()
        {
            if (settingsController == null)
            {
                settingsController = new StyleSettingsController<LabelingPolygonsStyleSettings>();
                settingsController.PreferredContentSize = new CGSize(540, 620);
                settingsController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                settingsController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                settingsController.StyleSettingsChanged += SettingsControllerStyleSettingsChanged;
            }

            PresentViewController(settingsController, true, null);
        }

        private void SettingsControllerStyleSettingsChanged(object sender, StyleSettingsChangedStyleSettingsControllerEventArgs e)
        {
            LabelingPolygonsStyleSettings settings = (LabelingPolygonsStyleSettings)e.StyleSettings;

            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LabelingPolygons"];
            FeatureLayer featureLayer = (FeatureLayer)layerOverlay.Layers["subdivision"];
            featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = settings.FittingFactorsOnlyWithin;
            featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.LabelAllPolygonParts = settings.LabelAllPolygonParts;
            layerOverlay.Refresh();
        }
    }
}