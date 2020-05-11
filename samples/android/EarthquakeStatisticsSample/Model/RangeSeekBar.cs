using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using System;

namespace MapSuiteEarthquakeStatistics
{
    public class RangeSeekBar : View
    {
        public event EventHandler<RangeChangedEventArgs> RangeChanged;

        private readonly UInt32 inRangeColor = 0xff007AFF;
        private readonly UInt32 outRangeColor = 0xff777777;
        private readonly UInt32 textColor = 0xffffffff;

        private float lineWidth = 5.0f;
        private float textSize = 25.0f;

        private int lowerCenterX;
        private int upperCenterX;

        private int bmpWidth;
        private int bmpHeight;

        private Bitmap lowerBmp;
        private Bitmap upperBmp;

        private bool isInit;
        private bool isLowerMoving;
        private bool isUpperMoving;

        private int paddingLeft;
        private int paddingRight;
        private int paddingTop;
        private int paddingBottom;

        private int lineHeight;
        private int lineLength;
        private int lineStart;
        private int lineEnd;

        private float smallValue;
        private float bigValue;

        private float smallRange;
        private float bigRange;

        public RangeSeekBar(Context content)
            : this(content, null)
        { }

        public RangeSeekBar(Context content, IAttributeSet attrs)
            : base(content, attrs)
        {
            isInit = false;
            isLowerMoving = false;
            isUpperMoving = false;

            smallValue = 0.0f;
            bigValue = 100.0f;

            paddingLeft = 100;
            paddingRight = 100;
            paddingTop = 10;
            paddingBottom = 10;

            smallRange = smallValue;
            bigRange = bigValue;

            lowerBmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Slider);
            upperBmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Slider);

            bmpWidth = upperBmp.Width;
            bmpHeight = upperBmp.Height;
        }

        public float SmallValue
        {
            get { return smallValue; }
            set { smallValue = value; }
        }

        public float BigValue
        {
            get { return bigValue; }
            set { bigValue = value; }
        }

        public float SmallRange
        {
            get { return smallRange; }
            set { smallRange = value; }
        }

        public float BigRange
        {
            get { return bigRange; }
            set { bigRange = value; }
        }

        private int MeasureWidth(int measureSpec)
        {
            int result = 0;

            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpecMode.Exactly)
            {
                result = specSize;
            }
            else
            {
                result = paddingLeft + paddingRight + bmpWidth * 2;

                if (specMode == MeasureSpecMode.AtMost)
                {
                    result = System.Math.Min(result, specSize);
                }
            }

            return result;
        }

        private int MeasureHeight(int measureHeight)
        {
            int result = 0;

            MeasureSpecMode specMode = MeasureSpec.GetMode(measureHeight);
            int specSize = MeasureSpec.GetSize(measureHeight);

            if (specMode == MeasureSpecMode.Exactly)
            {
                result = bmpHeight * 2;
            }
            else
            {
                result = bmpHeight + paddingTop + paddingBottom;

                if (specMode == MeasureSpecMode.AtMost)
                {
                    result = System.Math.Min(result, specSize);
                }
            }

            return result;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            widthMeasureSpec = MeasureWidth(widthMeasureSpec);
            heightMeasureSpec = MeasureHeight(heightMeasureSpec);
            SetMeasuredDimension(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!isInit)
            { Init(canvas); }

            bmpWidth = upperBmp.Width;
            bmpHeight = upperBmp.Height;

            lineHeight = this.Height - paddingBottom - lowerBmp.Height / 2;
            int barHeight = this.Height - paddingBottom - lowerBmp.Height / 2;

            float textHeight = (this.Height / 2f) + (textSize) / 2 - 3;

            // Line Style
            Paint linePaint = new Paint();
            linePaint.AntiAlias = true;
            linePaint.StrokeWidth = lineWidth;

            // Draw Highlight Line  
            linePaint.Color = new Color((int)inRangeColor);
            canvas.DrawLine(lowerCenterX, barHeight, upperCenterX, barHeight, linePaint);

            // Draw Line
            linePaint.Color = new Color((int)outRangeColor);
            canvas.DrawLine(lineStart, barHeight, lowerCenterX, barHeight, linePaint);
            canvas.DrawLine(upperCenterX, barHeight, lineEnd, barHeight, linePaint);

            // Draw Slider  
            Paint bmpPaint = new Paint();
            canvas.DrawBitmap(lowerBmp, lowerCenterX - bmpWidth / 2, barHeight - bmpHeight / 2, bmpPaint);
            canvas.DrawBitmap(lowerBmp, upperCenterX - bmpWidth / 2, barHeight - bmpHeight / 2, bmpPaint);

            // Draw Text
            Paint textPaint = new Paint();
            textPaint.Color = new Color((int)textColor);
            textPaint.TextSize = textSize;
            textPaint.AntiAlias = true;
            textPaint.StrokeWidth = lineWidth;

            int small = (int)Math.Round(smallRange);
            int big = (int)Math.Round(bigRange);

            canvas.DrawText(small.ToString(), 2, textHeight, textPaint);
            canvas.DrawText(big.ToString(), Width - textSize / 2 * big.ToString().Length - 5, textHeight, textPaint);
        }

        private void Init(Canvas canvas)
        {
            textSize = this.Height / 4;
            paddingLeft = canvas.Width / 7;
            paddingRight = canvas.Width / 7;

            lineLength = this.Width - paddingLeft - paddingRight;
            lineStart = paddingLeft;
            lineEnd = lineLength + paddingLeft;

            lowerCenterX = lineStart;
            upperCenterX = lineEnd;

            lineHeight = this.Height - paddingBottom - lowerBmp.Height / 2;

            lowerCenterX = InvertComputRange(smallRange);
            upperCenterX = InvertComputRange(bigRange);
            isInit = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);

            float xPos = e.GetX();
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    float yPos = e.GetY();
                    if (System.Math.Abs(yPos - lineHeight) > bmpHeight / 1.5)
                    {
                        return false;
                    }

                    if (System.Math.Abs(xPos - lowerCenterX) < bmpWidth / 1.5)
                    {
                        isLowerMoving = true;
                    }

                    if (System.Math.Abs(xPos - upperCenterX) < bmpWidth / 1.5)
                    {
                        isUpperMoving = true;
                    }

                    if (xPos >= lineStart && xPos <= lowerCenterX - bmpWidth / 2)
                    {
                        lowerCenterX = (int)xPos;
                        UpdateRange();
                        PostInvalidate();
                    }

                    if (xPos <= lineEnd && xPos >= upperCenterX + bmpWidth / 2)
                    {
                        upperCenterX = (int)xPos;
                        UpdateRange();
                        PostInvalidate();
                    }
                    break;
                case MotionEventActions.Move:
                    if (isLowerMoving)
                    {
                        if (xPos >= lineStart && xPos < upperCenterX - bmpWidth)
                        {
                            lowerCenterX = (int)xPos;
                            UpdateRange();
                            PostInvalidate();
                        }
                    }

                    if (isUpperMoving)
                    {
                        if (xPos > lowerCenterX + bmpWidth && xPos < lineEnd)
                        {
                            upperCenterX = (int)xPos;
                            UpdateRange();
                            PostInvalidate();
                        }
                    }

                    break;
                case MotionEventActions.Up:
                    isLowerMoving = false;
                    isUpperMoving = false;
                    break;
                default:
                    break;
            }

            return true;
        }

        private void UpdateRange()
        {
            smallRange = ComputRange(lowerCenterX);
            bigRange = ComputRange(upperCenterX);

            OnRangeChanged(new RangeChangedEventArgs() { LowerValue = (int)Math.Round(smallRange), UpperValue = (int)Math.Round(bigRange) });
        }

        private float ComputRange(float range)
        {
            return (range - lineStart) * (bigValue - smallValue) / lineLength
                    + smallValue;
        }

        private int InvertComputRange(float range)
        {
            return (int)((range - smallValue) * lineLength / (bigValue - smallValue) + lineStart);
        }

        protected virtual void OnRangeChanged(RangeChangedEventArgs e)
        {
            if (RangeChanged != null)
            {
                RangeChanged(this, e);
            }
        }
    }
}