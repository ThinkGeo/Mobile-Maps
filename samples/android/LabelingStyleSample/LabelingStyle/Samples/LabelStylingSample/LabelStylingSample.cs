using Android.Content;
using System.Collections.Generic;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelStylingSample : BaseSample
    {
        private static Dictionary<string, int> gridSizeDictionary;

        private LabelStylingSettingsDialog labelStylingSettingsDialog;

        static LabelStylingSample()
        {
            gridSizeDictionary = new Dictionary<string, int>();
            gridSizeDictionary["Small"] = 100;
            gridSizeDictionary["Medium"] = 500;
            gridSizeDictionary["Large"] = 1000;
        }

        public LabelStylingSample(Context context)
            : base(context)
        {
            Title = "Label Styling";
        }

        protected override void ApplySettings()
        {
            if (labelStylingSettingsDialog == null)
            {
                labelStylingSettingsDialog = new LabelStylingSettingsDialog(Context, new LabelStylingStyleSettings());
                labelStylingSettingsDialog.ApplyingSettings += labelStylingSettingsDialog_ApplyingSettings;
            }

            labelStylingSettingsDialog.Show();
        }

        private void labelStylingSettingsDialog_ApplyingSettings(object sender, System.EventArgs e)
        {
            LayerOverlay labelingStyleOverlay = MapView.Overlays["LabelingStyle"] as LayerOverlay;
            if (labelingStyleOverlay != null)
            {
                UpdateLabelStylingOverlay(labelingStyleOverlay, labelStylingSettingsDialog.Settings);
                MapView.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-10777472.620674, 3909177.1327916, -10776518.8812938, 3907779.90459956);

            WkbFileFeatureLayer parcelLayer = new WkbFileFeatureLayer(SampleHelper.GetDataPath("WkbFiles", "Parcels.wkb"));
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.FromHtml("#666666"), 2), new GeoSolidBrush(GeoColor.SimpleColors.White), PenBrushDrawingOrder.PenFirst);
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("X_REF", new GeoFont("Arail", 6, DrawingFontStyles.Regular), new GeoSolidBrush(GeoColor.FromHtml("#7b7b78")));
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 1000;
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White, 1);
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            parcelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            parcelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            parcelLayer.DrawingMarginInPixel = 256;

            ShapeFileFeatureLayer streetLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("Street.shp"));
            streetLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(GetRoadStyle());
            streetLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            streetLayer.DrawingMarginInPixel = 256;

            ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs.shp"));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromHtml("#99cc33")), new GeoPen(GeoColor.FromHtml("#666666"), 1), 7);
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("Name", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White, 1);
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.XOffsetInPixel = 10;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle(new GeoPen(GeoColor.FromHtml("#999999"), 1), new GeoSolidBrush(new GeoColor(100, GeoColor.FromHtml("#cccc99"))), PenBrushDrawingOrder.PenFirst);
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restaurantsLayer.DrawingMarginInPixel = 256;

            LayerOverlay labelingStyleOverlay = new LayerOverlay();
            labelingStyleOverlay.Layers.Add("parcel", parcelLayer);
            labelingStyleOverlay.Layers.Add("street", streetLayer);
            labelingStyleOverlay.Layers.Add("poi", restaurantsLayer);
            MapView.Overlays.Add("LabelingStyle", labelingStyleOverlay);
        }

        private static void UpdateLabelStylingOverlay(LayerOverlay layerOverlay, LabelStylingStyleSettings settings)
        {
            string gridSize = settings.GridSize.ToString();
            string drawingMargin = settings.DrawingMarginPercentage;

            bool useHalopen = settings.ApplyOutlineColor;
            bool useMask = settings.ApplyBackgroundMask;
            bool allowOverlapping = settings.LabelsOverlappingEachOther;
            LabelDuplicateRule labelDuplicateRule = settings.DuplicateRule;
            double drawingMarginInPixel = settings.GetDrawingMarginPercentage();
            int gridSizeValue = gridSizeDictionary[gridSize];

            foreach (string layerKey in layerOverlay.Layers.GetKeys())
            {
                FeatureLayer featureLayer = (FeatureLayer)layerOverlay.Layers[layerKey];
                List<TextStyle> textStyles = new List<TextStyle>();
                if (featureLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Count > 0)
                {
                    ClassBreakStyle classBreakStyle = featureLayer.ZoomLevelSet.ZoomLevel10.CustomStyles[0] as ClassBreakStyle;
                    textStyles.AddRange(classBreakStyle.ClassBreaks.Select(c => c.DefaultTextStyle));
                }
                else
                {
                    textStyles.Add(featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle);
                }

                foreach (var textStyle in textStyles)
                {
                    int r = textStyle.HaloPen.Color.RedComponent;
                    int g = textStyle.HaloPen.Color.GreenComponent;
                    int b = textStyle.HaloPen.Color.BlueComponent;

                    if (useHalopen)
                    {
                        textStyle.HaloPen = new GeoPen(GeoColor.FromArgb(255, r, g, b), textStyle.HaloPen.Width);
                    }
                    else
                    {
                        textStyle.HaloPen = new GeoPen(GeoColor.FromArgb(0, r, g, b), textStyle.HaloPen.Width);
                    }

                    textStyle.Mask.IsActive = useMask;
                    textStyle.GridSize = gridSizeValue;
                    textStyle.DuplicateRule = labelDuplicateRule;
                    textStyle.OverlappingRule = allowOverlapping ? LabelOverlappingRule.AllowOverlapping : LabelOverlappingRule.NoOverlapping;
                    featureLayer.DrawingMarginInPixel = (float)drawingMarginInPixel;
                }
            }
        }

        private ClassBreakStyle GetRoadStyle()
        {
            ClassBreakStyle roadStyle = new ClassBreakStyle("Type");
            ClassBreak pwyBreak = new ClassBreak();
            pwyBreak.Value = 1;
            pwyBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 12f), new GeoPen(GeoColor.FromHtml("#9e98b0"), 8f));
            pwyBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 12, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            pwyBreak.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White, 2);
            pwyBreak.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(pwyBreak);

            ClassBreak mainRoad = new ClassBreak();
            mainRoad.Value = 4;
            mainRoad.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#544c63"), 10f), new GeoPen(GeoColor.FromHtml("#e9cab0"), 7f));
            mainRoad.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            mainRoad.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White, 1);
            mainRoad.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(mainRoad);

            ClassBreak localRoadBreak = new ClassBreak();
            localRoadBreak.Value = 5;
            localRoadBreak.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.FromHtml("#bba7a2"), 8f), new GeoPen(GeoColor.FromHtml("#ffffff"), 6f));
            localRoadBreak.DefaultTextStyle = new TextStyle("ROAD_NAME", new GeoFont("Arial", 8, DrawingFontStyles.Regular), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            localRoadBreak.DefaultTextStyle.Mask = new AreaStyle();
            roadStyle.ClassBreaks.Add(localRoadBreak);
            return roadStyle;
        }
    }
}