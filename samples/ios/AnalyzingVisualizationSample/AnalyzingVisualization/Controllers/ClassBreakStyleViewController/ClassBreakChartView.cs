using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using ThinkGeo.Core;
using UIKit;

namespace AnalyzingVisualization
{
    public class ClassBreakChartView : UIView
    {
        public ClassBreakChartView(CGPoint lowerLeftPoint, ClassBreakStyle classBreakStyle)
        {
            ContentScaleFactor = UIScreen.MainScreen.Scale;
            InitializeClassBreakChartView(lowerLeftPoint, classBreakStyle);
        }

        private void InitializeClassBreakChartView(CGPoint lowerLeftPoint, ClassBreakStyle classBreakStyle)
        {
            ClassBreakChartLayer chartLayer = new ClassBreakChartLayer(classBreakStyle);
            chartLayer.Opacity = .9f;
            chartLayer.ContentsScale = ContentScaleFactor;
            chartLayer.SetNeedsDisplay();
            Layer.AddSublayer(chartLayer);
            Frame = new CGRect(new CGPoint(lowerLeftPoint.X, lowerLeftPoint.Y - chartLayer.Frame.Height), chartLayer.Frame.Size);
        }

        private class ClassBreakChartLayer : CALayer
        {
            private static readonly UIFont font = UIFont.FromName("Arial", 12);
            private ClassBreakStyle classBreakStyle;
            private float paddingTop;
            private float paddingLeft;
            private float segmentWidth;
            private float segmentHeight;
            private float textMargin;

            public ClassBreakChartLayer(ClassBreakStyle classBreakStyle)
            {
                this.classBreakStyle = classBreakStyle;

                float paddingRight = 2;
                float paddingBottom = 5;

                paddingTop = 2;
                paddingLeft = 5;
                segmentWidth = 24;
                segmentHeight = 20;
                textMargin = 4;

                nfloat maxTextWidth = classBreakStyle.ClassBreaks.Select(c => new NSString(c.Value.ToString("R0")).StringSize(font).Width).Max();
                Frame = new CGRect(0, 0, paddingLeft + segmentWidth + textMargin + maxTextWidth + paddingRight, paddingTop + paddingBottom + segmentHeight * classBreakStyle.ClassBreaks.Count);
            }

            public override void DrawInContext(CGContext ctx)
            {
                base.DrawInContext(ctx);

                ctx.SetFillColor(UIColor.White.CGColor);
                ctx.FillRect(new CGRect(paddingLeft - 2, paddingTop - 2, segmentWidth + 4, segmentHeight * classBreakStyle.ClassBreaks.Count + 4));

                for (int i = 0; i < classBreakStyle.ClassBreaks.Count; i++)
                {
                    int currentIndex = classBreakStyle.ClassBreaks.Count - i - 1;
                    float barLeft = paddingLeft;
                    float barTop = paddingTop + i * segmentHeight;

                    ctx.SetFillColor(GetCGColor(classBreakStyle.ClassBreaks[currentIndex]));
                    ctx.FillRect(new CGRect(barLeft, barTop, segmentWidth, segmentHeight));

                    NSString text = new NSString(classBreakStyle.ClassBreaks[currentIndex].Value.ToString("R0"));
                    CGSize textSize = text.StringSize(font);

                    nfloat textTop = barTop + segmentHeight - textSize.Height;
                    nfloat textLeft = barLeft + segmentWidth + textMargin;

                    ctx.SetTextDrawingMode(CGTextDrawingMode.Stroke);
                    ctx.SetFillColor(UIColor.White.CGColor);
                    ctx.SetLineWidth(1);

                    UIGraphics.PushContext(ctx);
                    text.DrawString(new CGPoint(textLeft, textTop), font);
                    UIGraphics.PopContext();

                    ctx.SetTextDrawingMode(CGTextDrawingMode.Fill);
                    ctx.SetFillColor(UIColor.Black.CGColor);

                    UIGraphics.PushContext(ctx);
                    text.DrawString(new CGPoint(textLeft, textTop), font);
                    UIGraphics.PopContext();
                }
            }

            private static CGColor GetCGColor(ClassBreak classBreak)
            {
                GeoColor color = ((GeoSolidBrush)classBreak.DefaultAreaStyle.FillBrush).Color;
                return UIColor.FromRGBA(color.RedComponent, color.GreenComponent, color.BlueComponent, color.AlphaComponent).CGColor;
            }
        }
    }
}