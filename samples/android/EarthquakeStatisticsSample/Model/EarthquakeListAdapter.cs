using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace MapSuiteEarthquakeStatistics
{
    internal class EarthquakeListAdapter : BaseAdapter
    {
        private LayoutInflater mInflater;
        private List<Dictionary<string, object>> data;

        public EarthquakeListAdapter(Context context)
        {
            mInflater = LayoutInflater.From(context);
        }

        public override int Count
        {
            get { return data.Count; }
        }

        public List<Dictionary<string, object>> Data
        {
            get
            {
                if (data == null)
                {
                    data = new List<Dictionary<string, object>>();
                }
                return data;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            Java.Util.HashMap map = new Java.Util.HashMap();

            foreach (var item in data[position])
            {
                map.Put(item.Key, item.Value.ToString());
            }

            return map;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            EarthquakeResultHolder holder = null;
            if (convertView == null)
            {
                holder = new EarthquakeResultHolder();
                convertView = mInflater.Inflate(Resource.Layout.listviewLayout, null);
                holder.YearValue = convertView.FindViewById<TextView>(Resource.Id.yearValue);
                holder.LongitudeValue = convertView.FindViewById<TextView>(Resource.Id.longitudeValue);
                holder.LatitudeValue = convertView.FindViewById<TextView>(Resource.Id.latitudeValue);
                holder.DepthValue = convertView.FindViewById<TextView>(Resource.Id.depthValue);
                holder.MagnitudeValue = convertView.FindViewById<TextView>(Resource.Id.magnitudeValue);
                holder.LocationValue = convertView.FindViewById<TextView>(Resource.Id.locationValue);
                convertView.Tag = holder;
            }
            else
            {
                holder = (EarthquakeResultHolder)convertView.Tag;
            }

            holder.YearValue.Text = data[position]["yearValue"].ToString();
            holder.LongitudeValue.Text = data[position]["longitudeValue"].ToString();
            holder.LatitudeValue.Text = data[position]["latitudeValue"].ToString();
            holder.DepthValue.Text = data[position]["depthValue"].ToString();
            holder.MagnitudeValue.Text = data[position]["magnitudeValue"].ToString();
            holder.LocationValue.Text = data[position]["locationValue"].ToString();

            return convertView;
        }
    }
}