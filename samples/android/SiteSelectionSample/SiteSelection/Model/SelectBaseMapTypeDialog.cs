using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using ThinkGeo.MapSuite.Android;

namespace MapSuiteSiteSelection
{
    public class SelectBaseMapTypeDialog : AlertDialog
    {
        private View selectBaseMapTypeView;
        private string tempBaseMap;

        private Context context;
        private Button baseMapOkButton;
        private Button baseMapCancelButton;

        private RadioButton wmkRoadRadioButton;
        private RadioButton wmkAerialRadioButton;
        private RadioButton wmkAerialWithLabelsRadioButton;
        private RadioButton osmRadioButton;
        private RadioButton bingaRadioButton;
        private RadioButton bingrRadioButton;

        public SelectBaseMapTypeDialog(Context context)
            : base(context)
        {
            this.context = context;
            selectBaseMapTypeView = View.Inflate(context, Resource.Layout.SelectBaseMapTypeLayout, null);
            this.SetView(selectBaseMapTypeView);

            wmkRoadRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkRoadRadioButton);
            wmkAerialRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkAerialRadioButton);
            wmkAerialWithLabelsRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkAerialWithLabelsRadioButton);
            osmRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.osmRadioButton);
            bingaRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.bingaRadioButton);
            bingrRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.bingrRadioButton);

            wmkRoadRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            wmkAerialRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            wmkAerialWithLabelsRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            osmRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            bingaRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            bingrRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;

            baseMapOkButton = selectBaseMapTypeView.FindViewById<Button>(Resource.Id.OkButton);
            baseMapCancelButton = selectBaseMapTypeView.FindViewById<Button>(Resource.Id.CancelButton);

            baseMapOkButton.Click += BaseMapOkButton_Click;
            baseMapCancelButton.Click += (sender, args) => Cancel();
        }

        private void BaseMapRadioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                tempBaseMap = radioButton.Text;
            }
        }

        public override void Show()
        {
            RefreshBaseMapTypeControls();
            base.Show();
        }

        private void RefreshBaseMap()
        {
            BaseMapType mapType;
            string baseMapTypeString = tempBaseMap.Replace(" ", "");

            if (Enum.TryParse(baseMapTypeString, true, out mapType))
            {
                if (mapType == BaseMapType.BingMapsAerial || mapType == BaseMapType.BingMapsRoad)
                {
                    ISharedPreferences preferences = context.GetSharedPreferences(SettingKey.PrefsFile, 0);
                    string result = preferences.GetString(SettingKey.PrefsBingMapKey, string.Empty);
                    if (!string.IsNullOrEmpty(result))
                    {
                        SampleMapView.Current.FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsAerialOverlay).ApplicationId = result;
                        SampleMapView.Current.FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsRoadOverlay).ApplicationId = result;
                        SampleMapView.Current.SwitchBaseMapTo(mapType);
                    }
                    else
                    {
                        SampleMapView.Current.BaseMapType = mapType;
                        InputBingMapKeyDialog dialog = new InputBingMapKeyDialog(context);
                        dialog.Show();
                    }
                }
                else
                {
                    SampleMapView.Current.SwitchBaseMapTo(mapType);
                }
            }
        }

        private void BaseMapOkButton_Click(object sender, EventArgs e)
        {
            RefreshBaseMap();

            Cancel();
        }

        private void RefreshBaseMapTypeControls()
        {
            switch (SampleMapView.Current.BaseMapType)
            {
                case BaseMapType.ThinkGeoCloudMapLight:
                    wmkRoadRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudMapAerial:
                    wmkAerialRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudMapHybrid:
                    wmkAerialWithLabelsRadioButton.Checked = true;
                    break;
                case BaseMapType.OpenStreetMap:
                    osmRadioButton.Checked = true;
                    break;
                case BaseMapType.BingMapsRoad:
                    bingrRadioButton.Checked = true;
                    break;
                case BaseMapType.BingMapsAerial:
                    bingaRadioButton.Checked = true;
                    break;

                default:
                    break;
            }
        }
    }
}