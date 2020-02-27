using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace GeometricFunctions
{
    public class ActivityListItemAdapter : ArrayAdapter<Tuple<string, int>>
    {
        private Activity context;
        private float ratio;

        public ActivityListItemAdapter(Activity context, IList<Tuple<string, int>> objects)
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
            textView.Text = item.Item1;
            textView.SetTextColor(Color.Black);
            textView.SetTextSize(Android.Util.ComplexUnitType.Px, 14 * ratio);
            ((LinearLayout.LayoutParams)(textView.LayoutParameters)).Gravity = GravityFlags.CenterVertical;

            ImageView imageView = layout.FindViewById<ImageView>(Android.Resource.Id.Icon);
            imageView.SetImageResource(item.Item2);
            imageView.LayoutParameters.Width = (int)(30 * ratio);
            imageView.LayoutParameters.Height = (int)(30 * ratio);
            ((LinearLayout.LayoutParams)imageView.LayoutParameters).SetMargins(0, (int)(2 * ratio), 0, (int)(2 * ratio));

            return layout;
        }
    }
}