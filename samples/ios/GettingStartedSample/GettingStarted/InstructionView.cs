using CoreGraphics;
using System;
using UIKit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the toolbar instruction view for the MapView.
    /// </summary>
    internal class InstructionView : UIView
    {
        private nfloat fullHeight;
        private UIView containerView;
        private readonly Action<CGRect, bool> instructionViewTopChanged;
        private readonly Action<UIView> resetChildrenLayout;

        private static int DescriptionMarginLeft
        {
            get
            {
                return IsOnIPhone ? 10 : 20;
            }
        }

        public InstructionView(UIView containerView, Action<UIView> resetChildrenLayout, Action<CGRect, bool> instructionViewTopChanged)
        {
            this.containerView = containerView;
            this.resetChildrenLayout = resetChildrenLayout;
            this.instructionViewTopChanged = instructionViewTopChanged;

            ResetChildrenLayout();
        }

        internal void DidRotate()
        {
            foreach (var subview in Subviews)
            {
                subview.RemoveFromSuperview();
            }

            ResetChildrenLayout();
        }

        private void CollapseButton_TouchDown(object sender, EventArgs e)
        {
            nfloat barHeight = fullHeight - Frame.Height;
            Animate(.2, () =>
            {
                CGPoint upperLeft = new CGPoint(0, Frame.Bottom - barHeight);
                CGSize size = new CGSize(Frame.Width, barHeight);
                Frame = new CGRect(upperLeft, size);
            });

            instructionViewTopChanged(Frame, true);
        }

        private void ResetChildrenLayout()
        {
            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            nfloat titleHeight = 40;
            nfloat instructionHeight = orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight ? 50 : 90;
            Frame = new CGRect(0, containerView.Bounds.Bottom - instructionHeight, containerView.Bounds.Width, instructionHeight);

            Alpha = 0.6f;
            BackgroundColor = UIColor.FromRGBA(126, 124, 129, 255);
            fullHeight = Frame.Height + titleHeight;

            UIView topLine = new UIView(new CGRect(0, 0, Frame.Width, 2))
            {
                BackgroundColor = UIColor.DarkGray
            };
            topLine.Layer.CornerRadius = 4;
            topLine.Layer.ShadowRadius = 3;
            topLine.Layer.ShadowOpacity = .8f;
            topLine.Layer.ShadowColor = UIColor.Black.CGColor;
            topLine.Layer.ShadowOffset = new CGSize(1, 0);

            UIView contentView;

            UILabel instructionLabel = new UILabel
            {
                BackgroundColor = BackgroundColor,
                Text = "Getting Started",
                Font = UIFont.FromName("Helvetica-Bold", 20),
                TextColor = UIColor.White,
                ShadowColor = UIColor.Gray,
                ShadowOffset = new CGSize(1, 1),
                TextAlignment = UITextAlignment.Left
            };
            Add(instructionLabel);

            if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
            {
                instructionLabel.Frame = new CGRect(DescriptionMarginLeft, 5, 160, titleHeight);

                contentView = new UIView(new CGRect(DescriptionMarginLeft, 10, Frame.Width - 2 * DescriptionMarginLeft,
                        Frame.Height));
            }
            else
            {
                UIButton collapseButton = UIButton.FromType(UIButtonType.RoundedRect);
                collapseButton.Frame = new CGRect(new CGPoint(Frame.Width - 60, 10), new CGSize(55, 30));
                collapseButton.SetImage(UIImage.FromBundle("more"), UIControlState.Normal);
                collapseButton.TintColor = UIColor.White;
                collapseButton.TouchUpInside += CollapseButton_TouchDown;

                instructionLabel.Frame = new CGRect(DescriptionMarginLeft, 0, Frame.Width, titleHeight);

                UIButton titleView = new UIButton
                {
                    Frame = new CGRect(0, 0, Frame.Width, titleHeight)
                };
                titleView.TouchUpInside += CollapseButton_TouchDown;
                titleView.Add(instructionLabel);
                titleView.Add(collapseButton);
                Add(titleView);

                nfloat contentViewTop = titleView.Frame.Bottom + 5;
                contentView = new UIView(new CGRect(DescriptionMarginLeft, contentViewTop, Frame.Width - 2 * DescriptionMarginLeft, Frame.Height - (contentViewTop)));
            }
            Add(topLine);
            Add(contentView);
            resetChildrenLayout(contentView);
            instructionViewTopChanged(Frame, false);
        }

        public static bool IsOnIPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }
    }
}