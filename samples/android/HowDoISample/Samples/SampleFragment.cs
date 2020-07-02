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

namespace ThinkGeo.UI.Android.HowDoI
{
    public abstract class SampleFragment : Fragment
    {
        protected MapView androidMap;
        protected View currentView;
        private bool isDisposed;

        public SampleFragment()
        { }

        public SampleInfo SampleInfo { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            currentView = inflater.Inflate(Resource.Layout.DisplayMapView, container, false);
            androidMap = currentView.FindViewById<MapView>(Resource.Id.androidmap);

            return currentView;
        }

        public override void OnDestroy()
        {
            if (androidMap != null && !isDisposed)
            {
                isDisposed = true;
                androidMap.Dispose();
            }

            base.OnDestroy();

        }
    }
}