using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;

namespace GettingStartedSample
{
    public class SelectBaseMapTypeDialog : AlertDialog
    {
        private View selectBaseMapTypeView;
        private string tempBaseMap;

        private Context context;
        private Button baseMapOkButton;
        private Button baseMapCancelButton;

        private RadioButton cloudMapLightRadioButton;
        private RadioButton cloudMapAerialRadioButton;
        private RadioButton cloudMapHybridRadioButton;

        public SelectBaseMapTypeDialog(Context context)
            : base(context)
        {
            this.context = context;
            selectBaseMapTypeView = View.Inflate(context, Resource.Layout.SelectBaseMapTypeLayout, null);
            this.SetView(selectBaseMapTypeView);

            cloudMapLightRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.cloudMapLightRadioButton);
            cloudMapAerialRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.cloudMapAerialRadioButton);
            cloudMapHybridRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.cloudMapHybridRadioButton);

            cloudMapLightRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            cloudMapAerialRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            cloudMapHybridRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;

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
                CustomMapView.Current.SwitchBaseMapTo(mapType);
            }
        }

        private void BaseMapOkButton_Click(object sender, EventArgs e)
        {
            RefreshBaseMap();
            CustomMapView.Current.Refresh();

            Cancel();
        }

        private void RefreshBaseMapTypeControls()
        {
            switch (CustomMapView.Current.BaseMapType)
            {
                case BaseMapType.ThinkGeoCloudMapLight:
                    cloudMapLightRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudMapAerial:
                    cloudMapAerialRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudMapBybrid:
                    cloudMapHybridRadioButton.Checked = true;
                    break;
                default:
                    break;
            }
        }
    }
}