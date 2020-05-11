using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteSiteSelection
{
    internal class FilterByTypeDialog : AlertDialog
    {
        private readonly string[] poiLayerNames = new string[] { "Hotels", "Medical Facilites", "Restaurants", "Schools", "Public Facilites" };

        private Context context;
        private string selectColumnName;
        private string selectColumnValue;
        private Collection<string> columns;
        private Spinner poiLayerTypeCandidatesSpinner;
        private FeatureLayer queryFeatureLayer;

        public FilterByTypeDialog(Context context)
            : base(context)
        {
            this.context = context;

            View filterByTypeDialogLayout = View.Inflate(context, Resource.Layout.FilterByTypeDialogLayout, null);
            SetView(filterByTypeDialogLayout);

            Spinner PoiLayerTypeSpinner = filterByTypeDialogLayout.FindViewById<Spinner>(Resource.Id.FeatureLayerTypeSpinner);
            poiLayerTypeCandidatesSpinner = filterByTypeDialogLayout.FindViewById<Spinner>(Resource.Id.ColumnTypeSpinner);
            Button cancelButton = filterByTypeDialogLayout.FindViewById<Button>(Resource.Id.CancelButton);
            Button okButton = filterByTypeDialogLayout.FindViewById<Button>(Resource.Id.OkButton);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, poiLayerNames);
            PoiLayerTypeSpinner.Adapter = adapter;

            Collection<string> columns = GetColumnValueCandidates(0);
            ArrayAdapter<string> candidatesAdapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, columns);
            poiLayerTypeCandidatesSpinner.Adapter = candidatesAdapter;

            PoiLayerTypeSpinner.ItemSelected += PoiLayerTypeSpinner_ItemSelected;
            poiLayerTypeCandidatesSpinner.ItemSelected += PoiLayerTypeCandidatesSpinner_ItemSelected;

            cancelButton.Click += CancelButton_Click;
            okButton.Click += OkButton_Click;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SampleMapView.Current.FilterConfiguration.QueryColumnName = selectColumnName;
            SampleMapView.Current.FilterConfiguration.QueryColumnValue = selectColumnValue;
            SampleMapView.Current.FilterConfiguration.QueryFeatureLayer = queryFeatureLayer;
            Cancel();

            SampleMapView.Current.UpdateHighlightOverlay();
        }

        private void PoiLayerTypeCandidatesSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (columns != null)
            {
                selectColumnValue = columns[e.Position];
            }
        }

        private void PoiLayerTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            LayerOverlay poiOverlay = SampleMapView.Current.FindOverlay<LayerOverlay>(OverlayKey.HighlightOverlay);
            switch (e.Position)
            {
                case 0:
                    queryFeatureLayer = (FeatureLayer)poiOverlay.Layers[LayerKey.HotelsLayer];
                    break;
                case 1:
                    queryFeatureLayer = (FeatureLayer)poiOverlay.Layers[LayerKey.MedicalFacilitiesLayer];
                    break;
                case 2:
                    queryFeatureLayer = (FeatureLayer)poiOverlay.Layers[LayerKey.RestaurantsLayer];
                    break;
                case 3:
                    queryFeatureLayer = (FeatureLayer)poiOverlay.Layers[LayerKey.SchoolsLayer];
                    break;
                default:
                    queryFeatureLayer = (FeatureLayer)poiOverlay.Layers[LayerKey.PublicFacilitiesLayer];
                    break;
            }

            columns = GetColumnValueCandidates(e.Position);
            ArrayAdapter<string> candidatesAdapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, columns);
            poiLayerTypeCandidatesSpinner.Adapter = candidatesAdapter;
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            Cancel();
        }

        private Collection<string> GetColumnValueCandidates(int selectLayerIndex)
        {
            selectColumnName = GetDefaultColumnNameByPoiType(poiLayerNames[selectLayerIndex]);

            Collection<string> candidates = new Collection<string>();
            candidates.Add(SettingKey.AllFeature);
            if (selectColumnName.Equals("ROOMS"))
            {
                candidates.Add("1 ~ 50");
                candidates.Add("50 ~ 100");
                candidates.Add("100 ~ 150");
                candidates.Add("150 ~ 200");
                candidates.Add("200 ~ 300");
                candidates.Add("300 ~ 400");
                candidates.Add("400 ~ 500");
                candidates.Add(">= 500");
            }
            else
            {
                SampleMapView.Current.FilterConfiguration.QueryFeatureLayer.Open();
                IEnumerable<string> distinctColumnValues = queryFeatureLayer.FeatureSource.GetDistinctColumnValues(selectColumnName).Select(v => v.ColumnValue);
                foreach (var distinctColumnValue in distinctColumnValues)
                {
                    candidates.Add(distinctColumnValue);
                }
            }
            candidates.Remove(string.Empty);

            return candidates;
        }

        private string GetDefaultColumnNameByPoiType(string poiType)
        {
            string result = string.Empty;
            if (poiType.Equals("Restaurants", StringComparison.OrdinalIgnoreCase))
            {
                result = "FoodType";
            }
            else if (poiType.Equals("Medical Facilites", StringComparison.OrdinalIgnoreCase)
                || poiType.Equals("Schools", StringComparison.OrdinalIgnoreCase))
            {
                result = "TYPE";
            }
            else if (poiType.Equals("Public Facilites", StringComparison.OrdinalIgnoreCase))
            {
                result = "AGENCY";
            }
            else if (poiType.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
            {
                result = "ROOMS";
            }
            return result;
        }
    }
}