using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class SampleViewHelper
    {
        private static int contentHeight;
        private static TextView instructionTextView;
        private static TextView descriptionTextView;
        private static LinearLayout instructionLayout;

        public static void InitializeInstruction(Context context, ViewGroup containerView, SampleInfo sampleNode, Collection<View> contentViews = null)
        {
            contentHeight = 0;

            LayoutInflater inflater = LayoutInflater.From(context);
            View instructionLayoutView = inflater.Inflate(Resource.Layout.Instruction, containerView);

            instructionTextView = instructionLayoutView.FindViewById<TextView>(Resource.Id.instructionTextView);
            descriptionTextView = instructionLayoutView.FindViewById<TextView>(Resource.Id.descriptionTextView);
            descriptionTextView.Text = sampleNode.Description;

            instructionLayout = instructionLayoutView.FindViewById<LinearLayout>(Resource.Id.instructionLinearLayout);
            LinearLayout contentLayout = instructionLayoutView.FindViewById<LinearLayout>(Resource.Id.contentLinearLayout);

            RelativeLayout headerRelativeLayout = instructionLayoutView.FindViewById<RelativeLayout>(Resource.Id.headerRelativeLayout);
            headerRelativeLayout.Click += HeaderRelativeLayoutClick;

            if (contentViews != null)
            {
                foreach (View view in contentViews)
                {
                    contentLayout.AddView(view);
                }
            }
        }

        private static void HeaderRelativeLayoutClick(object sender, EventArgs e)
        {
            contentHeight = contentHeight == 0 ? instructionLayout.Height - instructionTextView.Height : -contentHeight;
            instructionLayout.Layout(instructionLayout.Left, instructionLayout.Top + contentHeight, instructionLayout.Right, instructionLayout.Bottom);
        }
    }
}