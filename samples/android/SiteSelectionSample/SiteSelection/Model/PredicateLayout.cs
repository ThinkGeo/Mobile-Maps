using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MapSuiteSiteSelection
{
    internal class PredicateLayout : LinearLayout
    {
        Dictionary<View, Position> map = new Dictionary<View, Position>();

        private readonly int dividerLine = 5;
        private readonly int dividerCol = 3;

        public PredicateLayout(Context context)
            : base(context)
        { }

        public PredicateLayout(Context context, int horizontalSpacing, int verticalSpacing)
            : base(context)
        { }

        public PredicateLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        { }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int mWidth = MeasureSpec.GetSize(widthMeasureSpec);
            int mCount = this.ChildCount;
            int j = 0;
            int mLeft = 0;
            int mRight = 0;
            int mTop = 5;
            int mBottom = 0;

            for (int i = 0; i < mCount; i++)
            {
                View child = GetChildAt(i);

                child.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
                int childWidth = child.MeasuredWidth;
                int childHeight = child.MeasuredHeight;
                mRight += childWidth;

                Position position = new Position();
                mLeft = GetPosition(i - j, i);
                mRight = mLeft + child.MeasuredWidth;
                if (mRight >= mWidth)
                {
                    j = i;
                    mLeft = this.PaddingLeft;
                    mRight = mLeft + child.MeasuredWidth;
                    mTop += childHeight + dividerLine;
                }
                mBottom = mTop + child.MeasuredHeight;
                position.Left = mLeft;
                position.Top = mTop;
                position.Right = mRight;
                position.Bottom = mBottom;

                if (!map.ContainsKey(child))
                {
                    map.Add(child, position);
                }
            }
            SetMeasuredDimension(mWidth, mBottom + this.PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int count = this.ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                Position pos = map[child];
                if (pos != null)
                {
                    child.Layout(pos.Left, pos.Top, pos.Right, pos.Bottom);
                }
                else
                {
                    throw new Exception("MyLayout Error!");
                }
            }
        }

        private int GetPosition(int IndexInRow, int childIndex)
        {
            if (IndexInRow > 0)
            {
                return GetPosition(IndexInRow - 1, childIndex - 1)
                        + GetChildAt(childIndex - 1).MeasuredWidth + dividerCol;
            }
            return this.PaddingLeft;
        }

        private class Position
        {
            public int Left, Top, Right, Bottom;
        }
    }
}