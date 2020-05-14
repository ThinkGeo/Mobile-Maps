using CoreAnimation;
using CoreGraphics;
using System;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public class CERangeSliderKnobLayer : CALayer
    {
        public bool Highlight { get; set; }

        public UIRangeSlider Slider { get; set; }

        public override void DrawInContext(CGContext ctx)
        {
            nfloat cornerRadius = Bounds.Height * Slider.Curvaceousness / 2;
            UIBezierPath switchOutline = UIBezierPath.FromRoundedRect(Bounds, cornerRadius);

            ctx.AddPath(switchOutline.CGPath);
            ctx.Clip();

            ctx.SetFillColor(Slider.TrackColor.CGColor);
            ctx.AddPath(switchOutline.CGPath);
            ctx.FillPath();

            ctx.AddPath(switchOutline.CGPath);
            ctx.SetStrokeColor(UIColor.Gray.CGColor);
            ctx.SetLineWidth(0.5f);
            ctx.StrokePath();
        }
    }
}