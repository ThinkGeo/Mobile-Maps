using Android.Content;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    public class LabelingLinesSample : BaseSample
    {
        private LabelingLinesSettingsDialog labelingLinesSettingsDialog;

        public LabelingLinesSample(Context context)
            : base(context)
        {
            Title = "Labeling Lines";
        }

        protected override void ApplySettings()
        {
            if (labelingLinesSettingsDialog == null)
            {
                labelingLinesSettingsDialog = new LabelingLinesSettingsDialog(Context, new LabelingLinesStyleSettings());
                labelingLinesSettingsDialog.ApplyingSettings += labelingLinesSettingsDialog_ApplyingSettings;
            }
            labelingLinesSettingsDialog.Show();
        }

        private void labelingLinesSettingsDialog_ApplyingSettings(object sender, EventArgs e)
        {
            LayerOverlay labelingStyleOverlay = MapView.Overlays["LabelingLine"] as LayerOverlay;
            if (labelingStyleOverlay != null)
            {
                UpdateLabelStylingOverlay(labelingStyleOverlay, labelingLinesSettingsDialog.Settings);
                MapView.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-10777472.620674, 3909177.1327916, -10776518.8812938, 3907779.90459956);

            ShapeFileFeatureLayer streetLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("Street.shp"));
            streetLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(GetRoadStyle());
            streetLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            streetLayer.DrawingMarginInPixel = 256;

            LayerOverlay labelingLinesOverlay = new LayerOverlay();
            labelingLinesOverlay.Layers.Add("street", streetLayer);
            MapView.Overlays.Add("LabelingLine", labelingLinesOverlay);
        }

        private ClassBreakStyle GetRoadStyle()
        {
            ClassBreakStyle roadStyle = new ClassBreakStyle("Type");

            ClassBreak pwyBreak = new ClassBreak();
            pwyBreak.Value = 1;
            pwyBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 12f), new GeoPen(GeoColor.FromHtml("#9e98b0"), 8f));

            pwyBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 12, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            pwyBreak.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 2);
            pwyBreak.DefaultTextStyle.Mask = new AreaStyle();
            pwyBreak.DefaultTextStyle.TextPlacement = TextPlacement.AutoPlacement;
            roadStyle.ClassBreaks.Add(pwyBreak);

            ClassBreak mainRoad = new ClassBreak();
            mainRoad.Value = 4;
            mainRoad.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 10f), new GeoPen(GeoColor.FromHtml("#e9cab0"), 7f));
            mainRoad.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));
            mainRoad.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 1);
            mainRoad.DefaultTextStyle.Mask = new AreaStyle();
            mainRoad.DefaultTextStyle.TextPlacement = TextPlacement.AutoPlacement;
            roadStyle.ClassBreaks.Add(mainRoad);

            ClassBreak localRoadBreak = new ClassBreak();
            localRoadBreak.Value = 5;
            localRoadBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#bba7a2"), 6f), new GeoPen(GeoColor.FromHtml("#ffffff"), 4f));
            localRoadBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 8, DrawingFontStyles.Regular), new GeoSolidBrush(GeoColors.Black));
            localRoadBreak.DefaultTextStyle.HaloPen = new GeoPen(GeoColors.White, 2);
            localRoadBreak.DefaultTextStyle.Mask = new AreaStyle();
            localRoadBreak.DefaultTextStyle.TextPlacement = TextPlacement.AutoPlacement;
            roadStyle.ClassBreaks.Add(localRoadBreak);

            return roadStyle;
        }

        private void UpdateLabelStylingOverlay(LayerOverlay layerOverlay, LabelingLinesStyleSettings settings)
        {
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
    }
}