using CoreGraphics;
using System;
using UIKit;

namespace AnalyzingVisualization
{
    static class ExtensionHelper
    {
        public static void AnimatedShow(this UIView view, AnimateType animateType)
        {
            CGRect targetFrame = view.Frame;
            nfloat y = animateType == AnimateType.Up
                ? targetFrame.Y + targetFrame.Height
                : targetFrame.Y - targetFrame.Height;
            view.Frame = new CGRect(new CGPoint(targetFrame.X, y), targetFrame.Size);
            UIView.Animate(0.2, () =>
            {
                view.Hidden = false;
                view.Frame = targetFrame;
            });
        }

        public static void AnimatedHide(this UIView view, AnimateType animateType)
        {
            CGRect oldFrame = view.Frame;
            nfloat y = animateType == AnimateType.Up ? oldFrame.Y - oldFrame.Height : oldFrame.Y + oldFrame.Height;
            CGRect targetFrame = new CGRect(new CGPoint(view.Frame.X, y), view.Frame.Size);

            UIView.Animate(0.2, () =>
            {
                view.Frame = targetFrame;
            }, () =>
            {
                view.Hidden = true;
                view.Frame = oldFrame;
            });
        }
    }
}