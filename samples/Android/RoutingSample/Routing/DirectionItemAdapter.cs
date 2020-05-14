using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RoutingSample
{
    class DirectionItemAdapter : BaseAdapter<DirectionDataItem>
    {
        private List<DirectionDataItem> directionItems;
        private Activity context;

        public DirectionItemAdapter(Activity context, List<DirectionDataItem> directionItems)
        {
            this.context = context;
            this.directionItems = directionItems;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override DirectionDataItem this[int position]
        {
            get
            {
                return directionItems[position];
            }
        }

        public override int Count
        {
            get
            {
                return directionItems.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var currentItem = directionItems[position];

            var currentView = convertView;
            if (currentView == null)
                currentView = context.LayoutInflater.Inflate(Resource.Layout.Main, null);

            currentView.FindViewById<TextView>(Resource.Id.RoadNameView).Text = currentItem.RoadName;
            currentView.FindViewById<TextView>(Resource.Id.DirectionView).Text = currentItem.Direction;
            currentView.FindViewById<TextView>(Resource.Id.LengthView).Text = currentItem.Length.ToString();

            return currentView;
        }
    }
}