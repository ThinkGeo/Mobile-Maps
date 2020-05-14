using CoreGraphics;
using System;
using System.Globalization;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ZoomToAScale : BaseViewController
    {
        public ZoomToAScale()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);
            MapView.Overlays.Add(layerOverlay);

            MapView.Refresh();
        }

        private void Button_TouchDown(object sender, EventArgs e)
        {
            var button = sender as UIButton;
            string zoomLevelScale = button.Title(UIControlState.Normal);
            double scale = Convert.ToDouble(zoomLevelScale.Split(':')[1], CultureInfo.InvariantCulture);
            MapView.ZoomToScale(scale);
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 150 : 130, contentView =>
            {
                UILabel zoomToolLabelView = new UILabel(new CGRect(0, 0, 100, 30));
                zoomToolLabelView.Text = "Zoom tool:";
                zoomToolLabelView.TextColor = UIColor.White;
                zoomToolLabelView.ShadowColor = UIColor.Gray;
                zoomToolLabelView.ShadowOffset = new CGSize(1, 1);

                float buttonHeight = 30;

                if (!SampleUIHelper.IsOnIPhone)
                {
                    UIButton button = GetUIButton(0, 0, "1:1,000,000", buttonHeight, 100);
                    UIButton containButton = GetUIButton(110, 0, "1:5,000,000", buttonHeight, 100);
                    UIButton disjoinButton = GetUIButton(220, 0, "1:10,000,000", buttonHeight, 100);
                    UIButton intersectingButton = GetUIButton(330, 0, "1:50,000,000", buttonHeight, 100);
                    UIButton overlappingButton = GetUIButton(440, 0, "1:100,000,000", buttonHeight, 100);
                    UIButton topologicalEqualButton = GetUIButton(550, 0, "1:500,000,000", buttonHeight, 100);
                    contentView.AddSubviews(new UIView[] { button, containButton, disjoinButton, intersectingButton, intersectingButton, overlappingButton, topologicalEqualButton });
                }
                else
                {
                    UIButton button = GetUIButton(0, 0, "1:1,000,000", buttonHeight, 100);
                    UIButton containButton = GetUIButton(110, 0, "1:5,000,000", buttonHeight, 100);
                    contentView.AddSubviews(new UIView[] { button, containButton });
                }
            });
        }

        private UIButton GetUIButton(float leftLocation, float topLocation, string title, float height, float width)
        {
            UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
            button.Frame = new CGRect(leftLocation, topLocation, width, height);
            button.BackgroundColor = UIColor.FromRGB(241, 241, 241);
            button.SetTitle(title, UIControlState.Normal);
            button.TouchDown += Button_TouchDown;
            return button;
        }
    }
}