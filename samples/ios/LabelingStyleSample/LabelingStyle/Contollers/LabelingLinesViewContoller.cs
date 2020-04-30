using CoreGraphics;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace LabelingStyle
{
    public class LabelingLinesViewContoller : DetailViewController
    {
        private StyleSettingsController<LabelingLinesStyleSettings> settingsController;

        public LabelingLinesViewContoller(SliderViewController navigation)
            : base(navigation)
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();

            MapView.MapUnit = GeographyUnit.Meter;

            ShapeFileFeatureLayer streetLayer = new ShapeFileFeatureLayer("AppData/Street.shp");
            streetLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(GetRoadStyle());
            streetLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            streetLayer.DrawingMarginInPixel = 256;

            LayerOverlay labelingLinesOverlay = new LayerOverlay();
            labelingLinesOverlay.TransitionEffect = TransitionEffect.None;
            labelingLinesOverlay.Layers.Add("street", streetLayer);
            MapView.Overlays.Add("LabelingLine", labelingLinesOverlay);

            MapView.ZoomTo(new PointShape(-10776995.7509839, 3908478.51869558), MapView.ZoomLevelSet.ZoomLevel17.Scale);
        }

        protected override void OnConfigureButtonClicked()
        {
            if (settingsController == null)
            {
                settingsController = new StyleSettingsController<LabelingLinesStyleSettings>();
                settingsController.PreferredContentSize = new CGSize(540, 620);
                settingsController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                settingsController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                settingsController.StyleSettingsChanged += SettingsControllerStyleSettingsChanged;
            }

            PresentViewController(settingsController, true, null);
        }

        private void SettingsControllerStyleSettingsChanged(object sender, StyleSettingsChangedStyleSettingsControllerEventArgs e)
        {
            LabelingLinesStyleSettings settings = (LabelingLinesStyleSettings)e.StyleSettings;

            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LabelingLine"];
            FeatureLayer featureLayer = (FeatureLayer)layerOverlay.Layers["street"];
            ClassBreakStyle classBreakStyle = featureLayer.ZoomLevelSet.ZoomLevel10.CustomStyles[0] as ClassBreakStyle;
            if (classBreakStyle != null)
            {
                foreach (var classBreak in classBreakStyle.ClassBreaks)
                {
                    classBreak.DefaultTextStyle.SplineType = settings.SplineType;
                    classBreak.DefaultTextStyle.TextLineSegmentRatio = settings.GetLineSegmentRatio();
                }
            }

            layerOverlay.Refresh();
        }

        private static ClassBreakStyle GetRoadStyle()
        {
            ClassBreakStyle roadStyle = new ClassBreakStyle("Type");

            ClassBreak pwyBreak = new ClassBreak();
            pwyBreak.Value = 1;
            pwyBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 12f), new GeoPen(GeoColor.FromHtml("#9e98b0"), 8f));

            pwyBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 12, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            pwyBreak.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 2);
            pwyBreak.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(pwyBreak);

            ClassBreak mainRoad = new ClassBreak();
            mainRoad.Value = 4;
            mainRoad.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 10f), new GeoPen(GeoColor.FromHtml("#e9cab0"), 7f));
            mainRoad.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            mainRoad.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 1);
            mainRoad.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(mainRoad);

            ClassBreak localRoadBreak = new ClassBreak();
            localRoadBreak.Value = 5;
            localRoadBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#bba7a2"), 6f), new GeoPen(GeoColor.FromHtml("#ffffff"), 4f));
            localRoadBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 8, DrawingFontStyles.Regular), new GeoSolidBrush(GeoColors.Black));
            localRoadBreak.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 2);
            localRoadBreak.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(localRoadBreak);

            return roadStyle;
        }
    }
}