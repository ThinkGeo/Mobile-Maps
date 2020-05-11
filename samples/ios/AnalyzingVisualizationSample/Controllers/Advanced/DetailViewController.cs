using CoreGraphics;
using Foundation;
using System;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace AnalyzingVisualization
{
    [Register("DetailViewController")]
    public class DetailViewController : UIViewController
    {
        private UIView settingContainerView;
        private UIBarButtonItem settingButton;

        protected MapView MapView;
        public Action SettingButtonClick;
        public Action DetailButtonClick;

        public DetailViewController() { }

        protected UIView SettingContainerView
        {
            get { return settingContainerView; }
        }

        protected UIBarButtonItem SettingButton
        {
            get { return settingButton; }
        }

        protected float SettingContainerHeight { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Red;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            MapView = new MapView(View.Frame);
            MapView.BackgroundColor = UIColor.FromRGB(244, 242, 238);
          //  MapView.MapTools.ZoomMapTool.Center = new CGPoint(MapView.MapTools.ZoomMapTool.Center.X + 10, MapView.MapTools.ZoomMapTool.Center.Y + 55);
            View.Add(MapView);

            InitializeMap();
            InitializeToolbar();
            InitializeSettingView();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(MapView.CurrentExtent.Width / MapView.Frame.Width, MapView.CurrentExtent.Height / MapView.Frame.Height);
            MapView.Frame = View.Bounds;
            MapView.CurrentExtent = GetExtentRetainScale(MapView.CurrentExtent.GetCenterPoint(), MapView.Frame, resolution);
            MapView.Refresh();
        }

        /// <summary>
        /// This method customizes the map for a specific sample.
        /// We could add overlays, layers, styles inside of this method.
        /// </summary>
        protected virtual void InitializeMap()
        { }

        /// <summary>
        /// Gets the extent retain scale.
        /// </summary>
        /// <param name="currentLocationInMecator">The current location in mecator.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns>RectangleShape.</returns>
        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            double left = currentLocationInMecator.X - resolution * frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private void InitializeToolbar()
        {
            UIBarButtonItem detailButton = new UIBarButtonItem(UIImage.FromBundle("detail"), UIBarButtonItemStyle.Plain,
                OnDetailButtonClick);
            detailButton.TintColor = UIColor.Black;
            NavigationItem.SetLeftBarButtonItem(detailButton, true);

            settingButton = new UIBarButtonItem(UIImage.FromBundle("settings"), UIBarButtonItemStyle.Plain,
                OnSettingButtonClick);
            settingButton.TintColor = UIColor.Black;
            settingButton.Enabled = false;
            NavigationItem.SetRightBarButtonItems(new[] { settingButton }, true);
        }

        private void InitializeSettingView()
        {
            if (settingContainerView == null)
            {
                settingContainerView = new UIView();
                settingContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
                NSLayoutConstraint[] settingViewConstraints =
                {
                    NSLayoutConstraint.Create(settingContainerView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1, 0),
                    NSLayoutConstraint.Create(settingContainerView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1, 0),
                    NSLayoutConstraint.Create(settingContainerView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, SettingContainerHeight),
                    NSLayoutConstraint.Create(settingContainerView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0)
                };

                View.Add(settingContainerView);
                View.AddConstraints(settingViewConstraints);
            }
        }

        private void OnSettingButtonClick(object sender, EventArgs e)
        {
            SettingButtonClick?.Invoke();
        }

        private void OnDetailButtonClick(object sender, EventArgs e)
        {
            DetailButtonClick?.Invoke();
        }
    }
}