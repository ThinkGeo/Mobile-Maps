using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace AnalyzingVisualization
{
    public class ActivityListItemAdapter : ArrayAdapter<BaseSample>
    {
        private Activity context;
        private float ratio;

        public ActivityListItemAdapter(Activity context, IList<BaseSample> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ratio = context.Resources.DisplayMetrics.Density;

            LinearLayout layout = (LinearLayout)context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            var item = GetItem(position);

            TextView textView = layout.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = item.Title;
            textView.SetTextColor(Color.Black);
            textView.SetTextSize(Android.Util.ComplexUnitType.Px, 14 * ratio);
            ((LinearLayout.LayoutParams)(textView.LayoutParameters)).Gravity = GravityFlags.CenterVertical;

            ImageView imageView = layout.FindViewById<ImageView>(Android.Resource.Id.Icon);
            imageView.SetImageResource(item.ImageId);
            imageView.LayoutParameters.Width = (int)(30 * ratio);
            imageView.LayoutParameters.Height = (int)(30 * ratio);
            ((LinearLayout.LayoutParams)imageView.LayoutParameters).SetMargins(0, 2, 0, 2);

            return layout;
        }
    }
}