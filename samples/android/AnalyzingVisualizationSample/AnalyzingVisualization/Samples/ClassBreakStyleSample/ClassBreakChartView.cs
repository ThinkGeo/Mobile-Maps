using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class ClassBreakChartView : View
    {
        private float paddingTop;
        private float paddingLeft;
        private float paddingRight;
        private float paddingBottom;
        private float segmentWidth;
        private float segmentHeight;
        private float textMargin;
        private float maxTextWidth;

        private float offsetX;
        private float offsetY;

        private TextPaint textPaint;
        private ClassBreakStyle classBreakStyle;

        public ClassBreakChartView(Context context, ClassBreakStyle classBreakStyle)
            : this(context, null, classBreakStyle)
        {
        }

        public ClassBreakChartView(Context context, IAttributeSet attrs, ClassBreakStyle classBreakStyle)
            : base(context, attrs)
        {
            this.classBreakStyle = classBreakStyle;
            this.Alpha = .9f;

            float ratio = context.Resources.DisplayMetrics.Density;
            paddingTop = 2 * ratio;
            paddingLeft = 2 * ratio;
            paddingRight = 2 * ratio;
            paddingBottom = 2 * ratio;
            segmentWidth = 40 * ratio;
            segmentHeight = 20 * ratio;
            textMargin = 4 * ratio;

            textPaint = new TextPaint();
            textPaint.Color = Color.Black;
            textPaint.AntiAlias = true;
            textPaint.TextSize = 9 * ratio;

            maxTextWidth = classBreakStyle.ClassBreaks.Select(c => MeasureText(c.Value.ToString("R0"), textPaint).Width).Max();
            offsetX = 5 * ratio;
            offsetY = -5 * ratio;
        }

        public override void Draw(Canvas canvas)
        {
            float width = paddingLeft + segmentWidth + paddingRight;
            float height = paddingTop + segmentHeight * classBreakStyle.ClassBreaks.Count + paddingBottom;

            Paint backgroundPaint = new Paint();
            backgroundPaint.Color = Color.White;
            backgroundPaint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new RectF(offsetX, offsetY + (Height - height), offsetX + width, offsetY + Height), backgroundPaint);

            Paint segmentPaint = new Paint();
            segmentPaint.SetStyle(Paint.Style.Fill);

            TextPaint haloTextPaint = new TextPaint();
            haloTextPaint.Color = Color.White;
            haloTextPaint.SetStyle(Paint.Style.Stroke);
            haloTextPaint.StrokeWidth = 2;
            haloTextPaint.AntiAlias = true;
            haloTextPaint.TextSize = textPaint.TextSize;

            for (int i = 0; i < classBreakStyle.ClassBreaks.Count; i++)
            {
                int currentIndex = i;
                float barLeft = offsetX + paddingLeft;
                float barTop = offsetY + Height - (paddingBottom + (i + 1) * segmentHeight);
                float barRight = barLeft + segmentWidth;
                float barBottom = barTop + segmentHeight;

                segmentPaint.Color = GetColor(classBreakStyle.ClassBreaks[currentIndex]);
                canvas.DrawRect(new RectF(barLeft, barTop, barRight, barBottom), segmentPaint);

                float x = barRight + paddingRight + textMargin;
                float y = barTop + segmentHeight / 2 + textPaint.TextSize / 2;

                canvas.DrawText(classBreakStyle.ClassBreaks[currentIndex].Value.ToString("R0"), x, y, haloTextPaint);
                canvas.DrawText(classBreakStyle.ClassBreaks[currentIndex].Value.ToString("R0"), x, y, textPaint);
            }
        }

        private static Color GetColor(ClassBreak classBreak)
        {
            GeoColor color = classBreak.DefaultAreaStyle.FillSolidBrush.Color;
            return new Color(color.RedComponent, color.GreenComponent, color.BlueComponent, color.AlphaComponent);
        }

        private DrawingRectangleF MeasureText(string text, Paint paint)
        {
            Paint textPaint = paint;
            Rect bounds = new Rect();
            textPaint.GetTextBounds(text != " " ? text : "a", 0, text.Length, bounds);

            float width = bounds.Right - bounds.Left;
            float height = bounds.Bottom - bounds.Top;
            return new DrawingRectangleF(width / 2, height / 2, width, height);
        }
    }
}