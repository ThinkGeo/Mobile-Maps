using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Graphics;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the Select ZoomLevel List Dialog which would be popped up when tabbing and holding on the map.
    /// </summary>
    public class SelectZoomLevelListDialog : AlertDialog
    {
        private ZoomLevel currentZoomLevel;
        private MapView currentMapView;

        public SelectZoomLevelListDialog(Context context, MapView mapView)
            : base(context)
        {
            currentMapView = mapView;
            View selectZoomLevelView = View.Inflate(context, Resource.Layout.SelectZoomLevelLayout, null);
            this.SetView(selectZoomLevelView);

            ImageButton closeButton = selectZoomLevelView.FindViewById<ImageButton>(Resource.Id.closeButton);
            ListView zoomLevelList = selectZoomLevelView.FindViewById<ListView>(Resource.Id.zoomLevelListView);
            ZoomLevelListAdapter adapter = new ZoomLevelListAdapter(context);

            foreach (var item in GetData())
            {
                adapter.Data.Add(item);
            }

            zoomLevelList.Adapter = adapter;
            closeButton.Click += CloseButtonClick;
            zoomLevelList.ItemClick += ZoomLevelListItemClick;
        }

        public override void Show()
        {
            base.Show();

            Point size = new Point();
            Window.WindowManager.DefaultDisplay.GetSize(size);

            int width = size.X / 3;

            WindowManagerLayoutParams windowLayoutParams = this.Window.Attributes;
            windowLayoutParams.Gravity = GravityFlags.Center;
            windowLayoutParams.Width = width < 250 * MapView.DisplayDensity ? (int)(250 * MapView.DisplayDensity) : width;

            Window.Attributes = windowLayoutParams;
            Window.SetGravity(GravityFlags.Center);
        }

        public ZoomLevel CurrentZoomLevel
        {
            get { return currentZoomLevel; }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Cancel();
        }

        private void ZoomLevelListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    currentZoomLevel = currentMapView.ZoomLevelSet.ZoomLevel02;
                    break;

                case 1:
                    currentZoomLevel = currentMapView.ZoomLevelSet.ZoomLevel05;
                    break;

                case 2:
                    currentZoomLevel = currentMapView.ZoomLevelSet.ZoomLevel10;
                    break;

                case 3:
                    currentZoomLevel = currentMapView.ZoomLevelSet.ZoomLevel16;
                    break;

                case 4:
                    currentZoomLevel = currentMapView.ZoomLevelSet.ZoomLevel18;
                    break;
            }
            Cancel();
        }

        private static List<Dictionary<string, object>> GetData()
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

            Dictionary<string, object> level2 = new Dictionary<string, object>();
            Dictionary<string, object> level5 = new Dictionary<string, object>();
            Dictionary<string, object> level10 = new Dictionary<string, object>();
            Dictionary<string, object> level16 = new Dictionary<string, object>();
            Dictionary<string, object> level18 = new Dictionary<string, object>();

            level2.Add("ZoomLevel", "Level2:(1:295295895)");
            level5.Add("ZoomLevel", "Level5:(1:36911986)");
            level10.Add("ZoomLevel", "Level10:(1:1153499)");
            level16.Add("ZoomLevel", "Level16:(1:180123)");
            level18.Add("ZoomLevel", "Level18:(1:4505)");

            data.Add(level2);
            data.Add(level5);
            data.Add(level10);
            data.Add(level16);
            data.Add(level18);

            return data;
        }

        /// <summary>
        /// This class represents the adapter used in SelectZoomLevelListDialog.
        /// </summary>
        private class ZoomLevelListAdapter : BaseAdapter
        {
            private LayoutInflater mInflater;
            private List<Dictionary<string, object>> data;

            public ZoomLevelListAdapter(Context context)
            {
                mInflater = LayoutInflater.From(context);
            }

            public List<Dictionary<string, object>> Data
            {
                get { return data ?? (data = new List<Dictionary<string, object>>()); }
            }

            public override int Count
            {
                get { return Data.Count; }
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
                ZoomLevelItemsHolder holder = null;

                if (convertView == null)
                {
                    holder = new ZoomLevelItemsHolder();
                    convertView = mInflater.Inflate(Resource.Layout.ZoomLevelListTemplate, null);

                    holder.ZoomLevelValue = convertView.FindViewById<TextView>(Resource.Id.ZoomLevelTextView);
                    convertView.Tag = holder;
                }
                else
                {
                    holder = (ZoomLevelItemsHolder)convertView.Tag;
                }
                AbsListView.LayoutParams lp = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, 35);
                convertView.LayoutParameters = lp;

                holder.ZoomLevelValue.Text = data[position]["ZoomLevel"] as string;

                return convertView;
            }

            private class ZoomLevelItemsHolder : Java.Lang.Object
            {
                public TextView ZoomLevelValue;
            }
        }
    }
}