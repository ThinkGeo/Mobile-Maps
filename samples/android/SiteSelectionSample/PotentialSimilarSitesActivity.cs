using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteSiteSelection
{
    [Activity(Label = "Potential Similar Sites", Icon = "@drawable/MapSuite", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class PotentialSimilarSitesActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PotentialSimilarSitesLayout);

            RefreshQueryResultList();
        }

        private void RefreshQueryResultList()
        {
            TextView title = FindViewById<TextView>(Resource.Id.similarSiteTitleTextView);
            ListView view = FindViewById<ListView>(Resource.Id.listView);
            PotentialSimilarSitesAdapter adapter = view.Adapter == null ? new PotentialSimilarSitesAdapter(this) : (PotentialSimilarSitesAdapter)view.Adapter;
            adapter.ZoomingToFeature += Adapter_ZoomingToFeature;
            adapter.Data.Clear();

            InMemoryFeatureLayer highlightMarkerLayer = SampleMapView.Current.FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightMarkerLayer);
            foreach (Feature feature in highlightMarkerLayer.InternalFeatures)
            {
                Dictionary<string, object> record = new Dictionary<string, object>();

                record.Add("nameView", feature.ColumnValues["Name"]);
                record.Add("Feature", feature);

                adapter.Data.Add(record);
            }

            view.Adapter = adapter;
            title.Text = String.Format("Potential Similar Sites (Find {0} results)", adapter.Data.Count);
        }

        private void Adapter_ZoomingToFeature(object sender, ZoomToFeatureEventArgs e)
        {
            SampleMapView.Current.CenterAt(e.ZoomToFeature);
            SampleMapView.Current.ZoomToScale(SampleMapView.Current.ZoomLevelSet.ZoomLevel19.Scale);
            SampleMapView.Current.Refresh();

            Finish();
        }
    }
}