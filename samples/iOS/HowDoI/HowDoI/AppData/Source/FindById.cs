using CoreGraphics;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class FindById : BaseViewController
    {
        public FindById()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, contentView =>
            {
                UILabel label = new UILabel();
                label.Frame = new CGRect(0, 0, 140, 30);
                label.Text = "Feature Id: 137";
                label.TextColor = UIColor.White;
                label.ShadowColor = UIColor.Gray;
                label.ShadowOffset = new CGSize(1, 1);

                UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
                button.Frame = new CGRect(140, 0, 80, 30);
                button.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                button.TouchUpInside += Button_TouchDown;
                button.SetTitle("Find", UIControlState.Normal);

                contentView.AddSubviews(new UIView[] { label, button });
            });
        }

        private void Button_TouchDown(object sender, System.EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];

            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Feature feature = worldLayer.FeatureSource.GetFeatureById("137", new[] { "LONG_NAME" });

            if (feature != null)
            {
                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                highlightLayer.InternalFeatures.Add(feature);
                highlightOverlay.Refresh();
                MapView.ZoomTo(feature.GetBoundingBox());
            }
        }
    }
}