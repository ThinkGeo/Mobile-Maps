using CoreGraphics;
using Foundation;
using System;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace LabelingStyle
{
    [Register("DetailViewController")]
    public class DetailViewController : UIViewController
    {
        private UIBarButtonItem rightSettingItem;
        protected MapView MapView;

        public DetailViewController()
            : this(null)
        {
        }

        public DetailViewController(SliderViewController navigation)
        {
            UIBarButtonItem leftDetailItem = new UIBarButtonItem(UIImage.FromBundle("detail"), UIBarButtonItemStyle.Plain, delegate { navigation.ToggleMenu(); });
            leftDetailItem.TintColor = UIColor.Black;
            NavigationItem.SetLeftBarButtonItem(leftDetailItem, true);

            rightSettingItem = new UIBarButtonItem(UIImage.FromBundle("settings"), UIBarButtonItemStyle.Plain, null);
            rightSettingItem.TintColor = UIColor.Black;
            rightSettingItem.Clicked += RightSettingItem_Clicked;
            NavigationItem.SetRightBarButtonItem(rightSettingItem, true);
        }

        private void RightSettingItem_Clicked(object sender, EventArgs e)
        {
            OnConfigureButtonClicked();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            InitializeMap();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(MapView.CurrentExtent.Width / MapView.Frame.Width, MapView.CurrentExtent.Height / MapView.Frame.Height);
            MapView.Frame = View.Bounds;
            MapView.CurrentExtent = GetExtentRetainScale(MapView.CurrentExtent.GetCenterPoint(), MapView.Frame, resolution);
            MapView.Refresh();
        }

        protected virtual void InitializeMap()
        {
            MapView = new MapView(View.Frame);
            MapView.BackgroundColor = UIColor.FromRGB(244, 242, 238);
            MapView.MapTools.ZoomMapTool.Center = new CGPoint(MapView.MapTools.ZoomMapTool.Center.X + 10, MapView.MapTools.ZoomMapTool.Center.Y + 55);
            View.AddSubview(MapView);
        }

        protected virtual void OnConfigureButtonClicked()
        { }

        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            double left = currentLocationInMecator.X - resolution * frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }
    }
}