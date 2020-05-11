using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace AnalyzingVisualization
{
    public class SliderView : ViewGroup
    {
        private static readonly int VELOCITY_X_SPEED = 800;

        private float x;
        private Context context;
        private Scroller scroller;
        private bool dispatched;
        private bool slided;
        private int leftViewWidth;
        private int startScrollLeftOffset;
        private VelocityTracker mVelocityTracker;

        public SliderView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.context = context;
            this.leftViewWidth = 200;
            this.scroller = new Scroller(context);
        }

        public int LeftViewWidth
        {
            get { return leftViewWidth; }
            set { leftViewWidth = value; }
        }

        public View LeftView
        {
            get { return this.GetChildAt(0); }
        }

        public ViewGroup MainView
        {
            get { return this.GetChildAt(1) as ViewGroup; }
        }

        protected override void OnLayout(bool arg0, int arg1, int arg2, int arg3, int arg4)
        {
            int childCount = this.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                View view = this.GetChildAt(i);
                if (view.Visibility != ViewStates.Gone)
                {
                    view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
                }
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (mVelocityTracker == null)
            {
                mVelocityTracker = VelocityTracker.Obtain();
            }
            mVelocityTracker.AddMovement(e);
            MotionEventActions action = e.Action;
            if (action == MotionEventActions.Down)
            {
                x = e.GetX();

                if (IsSlided())
                {
                    dispatched = DispatchTouchEventToView(GetChildAt(0), e);
                }
                else
                {
                    dispatched = DispatchTouchEventToView(GetChildAt(1), e);
                }
            }
            else if (action == MotionEventActions.Move)
            {
                if (dispatched)
                {
                    if (IsSlided())
                    {
                        DispatchTouchEventToView(GetChildAt(0), e);
                    }
                    else
                    {
                        DispatchTouchEventToView(GetChildAt(1), e);
                    }
                }
                else
                {
                    float dx = e.GetX() - x;
                    View view = this.GetChildAt(1);
                    int left = (int)(view.Left + dx);
                    if (left >= 0)
                    {
                        view.Layout(left, view.Top, view.Width + left,
                                view.Top + view.Height);
                    }
                }
                x = e.GetX();
            }
            else if (action == MotionEventActions.Cancel
                  || action == MotionEventActions.Up)
            {
                if (dispatched)
                {
                    if (IsSlided())
                    {
                        DispatchTouchEventToView(GetChildAt(0), e);
                    }
                    else
                    {
                        DispatchTouchEventToView(GetChildAt(1), e);
                    }
                }
                else
                {
                    mVelocityTracker.ComputeCurrentVelocity(1000);
                    int velocityX = (int)mVelocityTracker.GetXVelocity(0);
                    if (velocityX > VELOCITY_X_SPEED)
                    {
                        SetSlided(true);
                    }
                    else if (velocityX < -VELOCITY_X_SPEED)
                    {
                        SetSlided(false);
                    }
                    else
                    {
                        View view = GetChildAt(1);
                        if (view.Left >= view.Width / 2)
                        {
                            SetSlided(true);
                        }
                        else
                        {
                            SetSlided(false);
                        }
                    }
                    if (mVelocityTracker != null)
                    {
                        try
                        {
                            mVelocityTracker.Recycle();
                        }
                        catch
                        { }
                    }
                }
            }
            else if (!IsSlided())
            {
                DispatchTouchEventToView(GetChildAt(1), e);
            }
            return true;
        }

        public bool IsSlided()
        {
            return slided;
        }

        public void SetSlided(bool slided)
        {
            View view = GetChildAt(1);
            startScrollLeftOffset = view.Left;
            if (slided)
            {
                scroller.StartScroll(0, Top, (int)(leftViewWidth * context.Resources.DisplayMetrics.Density) - startScrollLeftOffset, 0);
            }
            else
            {
                scroller.StartScroll(0, Top, -startScrollLeftOffset, 0);
            }
            this.slided = slided;
            PostInvalidate();
        }

        public override void ComputeScroll()
        {
            if (scroller.ComputeScrollOffset())
            {
                View view = GetChildAt(1);
                int left = startScrollLeftOffset + scroller.CurrX;
                view.Layout(left, view.Top, left + view.Width, view.Height);
                PostInvalidate();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            for (int i = 0; i < ChildCount; ++i)
            {
                GetChildAt(i).Measure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        public bool DispatchTouchEventToView(View view, MotionEvent ev)
        {
            try
            {
                return view.DispatchTouchEvent(ev);
            }
            catch (Exception e)
            {
                // some machines will throw exception
            }
            return false;
        }
    }
}