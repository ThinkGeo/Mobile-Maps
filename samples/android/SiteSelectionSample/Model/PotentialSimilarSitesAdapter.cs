using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteSiteSelection
{
    public class PotentialSimilarSitesAdapter : BaseAdapter
    {
        public event EventHandler<ZoomToFeatureEventArgs> ZoomingToFeature;

        private LayoutInflater mInflater;
        private List<Dictionary<string, object>> data;

        public PotentialSimilarSitesAdapter(Context context)
        {
            mInflater = LayoutInflater.From(context);
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

        public override int Count
        {
            get { return data.Count; }
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
            PotentialSimilarSiteResultHolder holder = null;

            if (convertView == null)
            {
                holder = new PotentialSimilarSiteResultHolder();
                convertView = mInflater.Inflate(Resource.Layout.ListItemTemplate, null);
                holder.NameValue = convertView.FindViewById<TextView>(Resource.Id.nameView);
                holder.IconValue = convertView.FindViewById<ImageButton>(Resource.Id.iconView);

                convertView.Tag = holder;
                holder.IconValue.Tag = holder;

                holder.IconValue.Click += IconValue_Click;
            }
            else
            {
                holder = (PotentialSimilarSiteResultHolder)convertView.Tag;
            }

            holder.NameValue.Text = data[position]["nameView"] as string;
            holder.SelectedFeature = data[position]["Feature"] as Feature;

            return convertView;
        }

        public void IconValue_Click(object sender, EventArgs e)
        {
            ImageButton iconButton = sender as ImageButton;
            PotentialSimilarSiteResultHolder holder = iconButton.Tag as PotentialSimilarSiteResultHolder;

            if (holder != null)
            {
                OnZoomingToFeature(new ZoomToFeatureEventArgs(holder.SelectedFeature));
            }
        }

        private void OnZoomingToFeature(ZoomToFeatureEventArgs e)
        {
            if (ZoomingToFeature != null)
            {
                ZoomingToFeature(this, e);
            }
        }
    }
}