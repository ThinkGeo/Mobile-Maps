using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteEarthquakeStatistics
{
    [Activity(Label = "Query Configuration", Icon = "@drawable/MapSuite", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConfigurationActivity : Activity
    {
        private readonly string UnknownString = "Unknown";

        private Feature queryFeature;
        private RangeSeekBar dateRangeSeekBar;
        private RangeSeekBar depthRangeSeekBar;
        private RangeSeekBar magnitudeRangeSeekBar;

        public ConfigurationActivity()
            : base()
        { }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Configuration);

            RefreshQueryResultList();

            Button queryButton = FindViewById<Button>(Resource.Id.queryButton);
            ListView resultListView = FindViewById<ListView>(Resource.Id.resultListView);

            magnitudeRangeSeekBar = FindViewById<RangeSeekBar>(Resource.Id.magnitudeRange);
            dateRangeSeekBar = FindViewById<RangeSeekBar>(Resource.Id.dateRange);
            depthRangeSeekBar = FindViewById<RangeSeekBar>(Resource.Id.depthRange);

            dateRangeSeekBar.SmallRange = Global.QueryConfiguration.LowerYear;
            depthRangeSeekBar.SmallRange = Global.QueryConfiguration.LowerDepth;
            magnitudeRangeSeekBar.SmallRange = Global.QueryConfiguration.LowerMagnitude;
            dateRangeSeekBar.BigRange = Global.QueryConfiguration.UpperYear;
            depthRangeSeekBar.BigRange = Global.QueryConfiguration.UpperDepth;
            magnitudeRangeSeekBar.BigRange = Global.QueryConfiguration.UpperMagnitude;

            dateRangeSeekBar.SmallValue = 1568;
            depthRangeSeekBar.SmallValue = 0;
            magnitudeRangeSeekBar.SmallValue = 0;
            dateRangeSeekBar.BigValue = 2010;
            depthRangeSeekBar.BigValue = 300;
            magnitudeRangeSeekBar.BigValue = 12;

            dateRangeSeekBar.RangeChanged += OptionRangeSeekBar_RangeChanged;
            depthRangeSeekBar.RangeChanged += OptionRangeSeekBar_RangeChanged;
            magnitudeRangeSeekBar.RangeChanged += OptionRangeSeekBar_RangeChanged;
            queryButton.Click += QueryButton_Click;
            resultListView.ItemLongClick += ResultListView_ItemLongClick;

            RegisterForContextMenu(resultListView);
            RefreshQueryResultList();
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.Id.resultListView && queryFeature != null)
            {
                menu.SetHeaderTitle("Options");
                menu.SetHeaderIcon(Android.Resource.Drawable.IcMenuInfoDetails);
                menu.Add(0, Menu.First + 1, 1, "Center at").SetIcon(Android.Resource.Drawable.IcMenuInfoDetails);
                menu.Add(0, Menu.First + 2, 2, "Zoom to").SetIcon(Android.Resource.Drawable.IcMenuInfoDetails);
            }
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            if (queryFeature != null)
            {
                Finish();

                LayerOverlay highLightOverlay = Global.MapView.Overlays[Global.HighlightOverlayKey] as LayerOverlay;
                InMemoryFeatureLayer highlightMarkerLayer = highLightOverlay.Layers[Global.HighlightMarkerLayerKey] as InMemoryFeatureLayer;

                highlightMarkerLayer.InternalFeatures.Clear();

                switch (item.ItemId)
                {
                    case Menu.First + 1:
                        highlightMarkerLayer.InternalFeatures.Add(queryFeature);
                        Global.MapView.CenterAt(queryFeature);
                        highLightOverlay.Refresh();
                        break;
                    case Menu.First + 2:
                        highlightMarkerLayer.InternalFeatures.Add(queryFeature);
                        Global.MapView.ZoomTo(queryFeature);
                        break;
                    default:
                        break;
                }
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        private void OptionRangeSeekBar_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            if (sender.Equals(dateRangeSeekBar))
            {
                Global.QueryConfiguration.LowerYear = e.LowerValue;
                Global.QueryConfiguration.UpperYear = e.UpperValue;
            }
            else if (sender.Equals(magnitudeRangeSeekBar))
            {
                Global.QueryConfiguration.LowerMagnitude = e.LowerValue;
                Global.QueryConfiguration.UpperMagnitude = e.UpperValue;
            }
            else if (sender.Equals(depthRangeSeekBar))
            {
                Global.QueryConfiguration.LowerDepth = e.LowerValue;
                Global.QueryConfiguration.UpperDepth = e.UpperValue;
            }
        }

        private void ResultListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            LayerOverlay highLightOverlay = Global.MapView.Overlays[Global.HighlightOverlayKey] as LayerOverlay;
            InMemoryFeatureLayer selectMarkerLayer = highLightOverlay.Layers[Global.SelectMarkerLayerKey] as InMemoryFeatureLayer;

            if (selectMarkerLayer != null)
            {
                queryFeature = selectMarkerLayer.InternalFeatures[e.Position];
                e.Handled = false;
            }
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            RefreshQueryResultList();
        }

        private void RefreshQueryResultList()
        {
            TextView title = FindViewById<TextView>(Resource.Id.queryResultTitleTextView);
            ListView view = FindViewById<ListView>(Resource.Id.resultListView);
            EarthquakeListAdapter adapter = view.Adapter == null ? new EarthquakeListAdapter(this) : (EarthquakeListAdapter)view.Adapter;

            adapter.Data.Clear();

            Proj4Projection mercatorToWgs84Projection = Global.GetWgs84ToMercatorProjection();
            mercatorToWgs84Projection.Open();

            try
            {
                Global.FilterSelectedEarthquakeFeatures(Global.GetBackupQueriedFeatures());

                LayerOverlay highLightOverlay = Global.MapView.Overlays[Global.HighlightOverlayKey] as LayerOverlay;
                InMemoryFeatureLayer selectMarkerLayer = highLightOverlay.Layers[Global.SelectMarkerLayerKey] as InMemoryFeatureLayer;

                foreach (var feature in selectMarkerLayer.InternalFeatures)
                {
                    double longitude = 0, latitude = 0;

                    if (double.TryParse(feature.ColumnValues["LONGITUDE"], out longitude) && double.TryParse(feature.ColumnValues["LATITIUDE"], out latitude))
                    {
                        PointShape point = new PointShape(longitude, latitude);
                        point = (PointShape)mercatorToWgs84Projection.ConvertToInternalProjection(point);
                        longitude = point.X;
                        latitude = point.Y;
                    }

                    double year, depth, magnitude;
                    double.TryParse(feature.ColumnValues["MAGNITUDE"], out magnitude);
                    double.TryParse(feature.ColumnValues["DEPTH_KM"], out depth);
                    double.TryParse(feature.ColumnValues["YEAR"], out year);

                    Dictionary<String, Object> result = new Dictionary<string, object>();
                    result["yearValue"] = year != -9999 ? year.ToString(CultureInfo.InvariantCulture) : UnknownString;
                    result["longitudeValue"] = longitude.ToString("f2", CultureInfo.InvariantCulture);
                    result["latitudeValue"] = latitude.ToString("f2", CultureInfo.InvariantCulture);
                    result["depthValue"] = depth != -9999 ? depth.ToString(CultureInfo.InvariantCulture) : UnknownString;
                    result["magnitudeValue"] = magnitude != -9999 ? magnitude.ToString(CultureInfo.InvariantCulture) : UnknownString;
                    result["locationValue"] = feature.ColumnValues["LOCATION"];

                    adapter.Data.Add(result);
                }

                view.Adapter = adapter;
                title.Text = String.Format("Query Result (Find {0} results)", adapter.Data.Count);
            }
            finally
            {
                mercatorToWgs84Projection.Close();
            }
        }
    }
}