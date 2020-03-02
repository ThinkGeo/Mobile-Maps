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
    public class LabelingPolygonsSample : BaseSample
    {
        private LabelingPolygonsSettingsDialog labelingPolygonsSettingsDialog;

        public LabelingPolygonsSample(Context context)
            : base(context)
        {
            Title = "Labeling Plygons";
        }

        protected override void ApplySettings()
        {
            if (labelingPolygonsSettingsDialog == null)
            {
                labelingPolygonsSettingsDialog = new LabelingPolygonsSettingsDialog(Context, new LabelingPolygonsStyleSettings());
                labelingPolygonsSettingsDialog.ApplyingSettings += labelingPolygonsSettingsDialog_ApplyingSettings;
            }
            labelingPolygonsSettingsDialog.Show();
        }

        private void labelingPolygonsSettingsDialog_ApplyingSettings(object sender, EventArgs e)
        {
            LayerOverlay labelingStyleOverlay = MapView.Overlays["LabelingPolygons"] as LayerOverlay;
            if (labelingStyleOverlay != null)
            {
                UpdateLabelStylingOverlay(labelingStyleOverlay, labelingPolygonsSettingsDialog.Settings);
                MapView.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-10777472.620674, 3909177.1327916, -10776518.8812938, 3907779.90459956);

            WkbFileFeatureLayer subdivisionsLayer = new WkbFileFeatureLayer(SampleHelper.GetDataPath("WkbFiles", "Subdivisions.wkb"));
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.White, GeoColor.FromHtml("#9C9C9C"), 1);
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("NAME_COMMO", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 1);
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.BestPlacement = true;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 3000;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            subdivisionsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay labelingPolygonsOverlay = new LayerOverlay();
            labelingPolygonsOverlay.Layers.Add("subdivision", subdivisionsLayer);
            MapView.Overlays.Add("LabelingPolygons", labelingPolygonsOverlay);
        }

        private void UpdateLabelStylingOverlay(LayerOverlay layerOverlay, LabelingPolygonsStyleSettings settings)
        {
            FeatureLayer featureLayer = (FeatureLayer)layerOverlay.Layers["subdivision"];
            featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = settings.FittingFactorsOnlyWithin;
            featureLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.LabelAllPolygonParts = settings.LabelAllPolygonParts;
            layerOverlay.Refresh();
        }
    }
}