using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ConvertToFromWellKnownBinary : BaseViewController
    {
        private UITextView wkbView;
        private UITextView txtView;

        public ConvertToFromWellKnownBinary()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = (new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625));

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(worldLayer);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 150, (contentView) =>
            {
                wkbView = new UITextView(new CGRect(0, 0, 100, 55));
                wkbView.Text = "AQEAAAAAAAAAAAAkQAAAAAAAADRA";

                UIButton convertToButton = UIButton.FromType(UIButtonType.RoundedRect);
                convertToButton.Frame = new CGRect(110, 0, 80, 55);
                convertToButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                convertToButton.TouchDown += convertToButton_TouchDown;
                convertToButton.SetTitle("Convert", UIControlState.Normal);

                txtView = new UITextView(new CGRect(200, 0, 100, 55));

                contentView.AddSubviews(new UIView[] { wkbView, convertToButton, txtView });
            });
        }

        private void convertToButton_TouchDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(wkbView.Text))
            {
                byte[] wellKnownBinary = Convert.FromBase64String(wkbView.Text);
                Feature feature = new Feature(wellKnownBinary);

                txtView.Text = feature.GetWellKnownText();
                wkbView.Text = string.Empty;
            }
            else if (!string.IsNullOrEmpty(txtView.Text))
            {
                Feature feature = new Feature(txtView.Text);
                byte[] wellKnownBinary = feature.GetWellKnownBinary();

                wkbView.Text = Convert.ToBase64String(wellKnownBinary);
                txtView.Text = string.Empty;
            }
        }
    }
}