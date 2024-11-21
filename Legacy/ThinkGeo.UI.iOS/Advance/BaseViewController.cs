using CoreGraphics;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class BaseViewController : UIViewController
    {
        protected MapView MapView;

        protected BaseViewController()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            MapView = new MapView(View.Frame);
            MapView.BackgroundColor = UIColor.FromRGB(244, 242, 238);

            View.AddSubview(MapView);
            InitializeInstruction();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(MapView.CurrentExtent.Width / MapView.Frame.Width, MapView.CurrentExtent.Height / MapView.Frame.Height);
            MapView.Frame = View.Bounds;
            MapView.CurrentExtent = GetExtentRetainScale(MapView.CurrentExtent.GetCenterPoint(), MapView.Frame, resolution);
            MapView.Refresh();
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            SampleUIHelper.InstructionContainer.Hidden = true;
        }


        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            InitializeInstruction();
            SampleUIHelper.InstructionContainer.Hidden = false;
        }

        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            double left = currentLocationInMecator.X - resolution * frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        protected virtual void InitializeInstruction()
        {
        }
    }
}