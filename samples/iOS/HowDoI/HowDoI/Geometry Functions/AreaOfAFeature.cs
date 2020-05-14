using CoreGraphics;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class AreaOfAFeature : BaseViewController
    {
        private UILabel messageLabel;

        public AreaOfAFeature()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = (new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625));
            MapView.MapSingleTap += MapView_MapSingleTap;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            MapView.Refresh();
        }

        private void MapView_MapSingleTap(object sender, UIGestureRecognizer e)
        {
            CGPoint location = e.LocationInView(MapView);
            PointShape position = MapUtil.ToWorldCoordinate(MapView.CurrentExtent, (float)location.X, (float)location.Y, (float)MapView.Frame.Width, (float)MapView.Frame.Height);

            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(position, new[] { "CNTRY_NAME" });

            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                double area = areaShape.GetArea(GeographyUnit.DecimalDegree, AreaUnit.SquareKilometers);
                messageLabel.Text = string.Format(CultureInfo.InvariantCulture, "{0} has an area of {1:N0} square kilometers.", selectedFeatures[0].ColumnValues["CNTRY_NAME"].Trim(), area);

                highlightLayer.InternalFeatures.Clear();
                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
                highlightOverlay.Refresh();
            }
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

                contentView.AddSubview(messageLabel);
            });
        }
    }
}