using System;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace ThinkGeo.UI.Android.HowDoI
{
    public abstract class SampleFragment : Fragment
    {
        private BottomSheetBehavior bottomSheetBehavior;

        public abstract int Layout { get; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Layout, container, false);

            var bottomSheet = view.FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
            bottomSheetBehavior.PeekHeight = bottomSheet.Height;
            bottomSheetBehavior.Hideable = true;

            var fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += Fab_Click;

            return view;
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            switch (bottomSheetBehavior.State)
            {
                case BottomSheetBehavior.StateHalfExpanded:
                case BottomSheetBehavior.StateCollapsed:
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    break;
                case BottomSheetBehavior.StateExpanded:
                    bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                    break;
                default:
                    break;
            }
        }
    }
}