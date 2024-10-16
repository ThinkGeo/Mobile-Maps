using CoreGraphics;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LengthOfAFeature : BaseViewController
    {
        private UILabel messageLabel;

        public LengthOfAFeature()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);
            MapView.SingleTap += MapViewOnSingleTap;

            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer("AppData/Frisco/TXlkaA40.shp");
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlka40FeatureLayer.DrawingMarginInPixel = 128;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("RoadOverlay", layerOverlay);
            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 9.2F, GeoColors.DarkGray, 12.2F, true);
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(150, GeoColors.Blue));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            PopupOverlay popupOverlay = new PopupOverlay();
            MapView.Overlays.Add("popupOverlay", popupOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, contentView =>
            {
                int labelHeight = SampleUIHelper.IsOnIPhone ? 45 : 30;
                messageLabel = new UILabel(new CGRect(0, 0, contentView.Frame.Width, labelHeight));
                messageLabel.TextColor = UIColor.White;
                messageLabel.ShadowColor = UIColor.Gray;
                messageLabel.ShadowOffset = new CGSize(1, 1);
                messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
                messageLabel.Lines = 0;
                messageLabel.PreferredMaxLayoutWidth = contentView.Frame.Width;
                contentView.AddSubview(messageLabel);
            });
        }

        private void MapViewOnSingleTap(object? sender, TouchMapViewEventArgs e)
        {
            var position = e.PointInWorldCoordinate;
            
            LayerOverlay overlay = (LayerOverlay)MapView.Overlays["RoadOverlay"];
            FeatureLayer roadLayer = (FeatureLayer)overlay.Layers["TXlkaA40"];

            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            roadLayer.Open();
            Collection<Feature> selectedFeatures = roadLayer.QueryTools.GetFeaturesNearestTo(position, GeographyUnit.DecimalDegree, 1, new[] { "fename" });

            if (selectedFeatures.Count > 0)
            {
                LineBaseShape lineShape = (LineBaseShape)selectedFeatures[0].GetShape();
                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                highlightLayer.InternalFeatures.Add(new Feature(lineShape));

                double length = lineShape.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Meter);
                string lengthMessage = string.Format(CultureInfo.InvariantCulture, "{0} has a length of {1:F2} meters.", selectedFeatures[0].ColumnValues["fename"].Trim(), length);

                messageLabel.Text = lengthMessage;
                highlightOverlay.Refresh();
            }
        }
    }
}