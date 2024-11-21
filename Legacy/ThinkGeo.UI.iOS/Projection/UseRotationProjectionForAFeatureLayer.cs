using CoreGraphics;
using System;
using System.Drawing;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class UseRotationProjectionForAFeatureLayer : BaseViewController
    {
        private RotationProjectionConverter rotateProjection;

        public UseRotationProjectionForAFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.BackgroundColor = UIColor.FromRGB(250, 247, 243);

            rotateProjection = new RotationProjectionConverter();
            MapView.CurrentExtent = rotateProjection.GetUpdatedExtent(new RectangleShape(-180.0, 83.0, 180.0, -90.0));

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.FeatureSource.ProjectionConverter = rotateProjection;
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColor.FromArgb(100, GeoColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            MapView.Refresh();

            InitializeFunctionButtons();
        }

        private void InitializeFunctionButtons()
        {
            UIButton zoomInButton = GetUIButton(0, "Plus", (sender, o) => MapView.ZoomIn(50));
            UIButton zoomOutButton = GetUIButton(40, "Minus", (sender, o) => MapView.ZoomOut(50));

            UIView zoomButtonsView = new UIView(new CGRect(View.Frame.Width - 50, 80, 40, 80));
            zoomButtonsView.BackgroundColor = UIColor.FromRGBA(215, 215, 215, 255);
            zoomButtonsView.Add(zoomInButton);
            zoomButtonsView.Add(zoomOutButton);
            View.AddSubview(zoomButtonsView);

            UIButton counterclockwiseButton = GetUITextButton(0, "CounterClockwise", rotateCounterclockwiseClick);
            UIButton clockwiseButton = GetUITextButton(40, "Clockwise", rotateClockwiseClick);

            UIView trackButtonsView = new UIView(new CGRect(10, 80, 40, 100));
            trackButtonsView.Layer.CornerRadius = 8;
            trackButtonsView.BackgroundColor = UIColor.FromRGBA(215, 215, 215, 255);
            trackButtonsView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin;
            trackButtonsView.AddSubview(counterclockwiseButton);
            trackButtonsView.AddSubview(clockwiseButton);

            View.AddSubview(trackButtonsView);
        }

        private void rotateCounterclockwiseClick(object sender, EventArgs e)
        {
            rotateProjection.Angle += 20;
            MapView.CurrentExtent = rotateProjection.GetUpdatedExtent(MapView.CurrentExtent);

            MapView.Refresh();
        }

        private void rotateClockwiseClick(object sender, EventArgs e)
        {
            rotateProjection.Angle -= 20;
            MapView.CurrentExtent = rotateProjection.GetUpdatedExtent(MapView.CurrentExtent);

            MapView.Refresh();
        }

        private static UIButton GetUITextButton(int topLocation, string imageName, EventHandler handler)
        {
            CGSize buttonSize = new CGSize(40, 40);
            UIButton button = new UIButton(new CGRect(new Point(0, topLocation), buttonSize));
            button.SetTitle(imageName, UIControlState.Application);
            button.SetTitleColor(UIColor.White, UIControlState.Application);
            button.SetImage(UIImage.FromBundle("Plus"), UIControlState.Normal);
            button.TouchUpInside += handler;
            return button;
        }

        private static UIButton GetUIButton(int topLocation, string imageName, EventHandler handler)
        {
            CGSize buttonSize = new CGSize(40, 40);
            UIButton button = new UIButton(new CGRect(new Point(0, topLocation), buttonSize));
            button.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
            button.SetTitle(imageName, UIControlState.Application);
            button.TouchUpInside += handler;
            return button;
        }
    }
}