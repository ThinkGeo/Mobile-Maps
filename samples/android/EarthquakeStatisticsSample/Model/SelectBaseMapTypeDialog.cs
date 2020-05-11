using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteEarthquakeStatistics
{
    public class SelectBaseMapTypeDialog : AlertDialog
    {
        private ISharedPreferences preferences;
        private View selectBaseMapTypeView;
        private string tempBaseMap;

        private Context context;
        private Button baseMapOkButton;
        private Button baseMapCancelButton;

        private RadioButton wmkRoadRadioButton;
        private RadioButton wmkAerialRadioButton;
        private RadioButton wmkAerialWithLabelsRadioButton;
        private RadioButton bingaRadioButton;
        private RadioButton bingrRadioButton;

        public SelectBaseMapTypeDialog(Context context, ISharedPreferences preferences)
            : base(context)
        {
            this.context = context;
            this.preferences = preferences;

            selectBaseMapTypeView = View.Inflate(context, Resource.Layout.SelectBaseMapTypeLayout, null);
            this.SetView(selectBaseMapTypeView);

            wmkRoadRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkRoadRadioButton);
            wmkAerialRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkAerialRadioButton);
            wmkAerialWithLabelsRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.wmkAerialWithLabelsRadioButton);
            bingaRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.bingaRadioButton);
            bingrRadioButton = selectBaseMapTypeView.FindViewById<RadioButton>(Resource.Id.bingrRadioButton);

            wmkRoadRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            wmkAerialRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            wmkAerialWithLabelsRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;

            bingaRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            bingrRadioButton.CheckedChange += BaseMapRadioButton_CheckedChange;
            baseMapOkButton = selectBaseMapTypeView.FindViewById<Button>(Resource.Id.OkButton);
            baseMapCancelButton = selectBaseMapTypeView.FindViewById<Button>(Resource.Id.CancelButton);

            baseMapOkButton.Click += BaseMapOkButton_Click;
            baseMapCancelButton.Click += (sender, args) => Cancel();
        }

        public override void Show()
        {
            RefreshBaseMapTypeControls();
            base.Show();
        }

        private void BaseMapRadioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                tempBaseMap = radioButton.Text;
            }
        }

        private void RefreshBaseMap()
        {
            BaseMapType mapType;
            string baseMapTypeString = tempBaseMap.Replace(" ", "");
            if (Enum.TryParse(baseMapTypeString, true, out mapType)) Global.BaseMapType = mapType;

            switch (Global.BaseMapType)
            {
                case BaseMapType.ThinkGeoCloudLightMap:
                    Global.MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                    Global.ThinkGeoCloudMapsOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.ThinkGeoCloudAerialMap:
                    Global.MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                    Global.ThinkGeoCloudMapsOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.ThinkGeoCloudHybridMap:
                    Global.MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                    Global.ThinkGeoCloudMapsOverlay.IsVisible = true;
                    Global.OpenStreetMapOverlay.IsVisible = false;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.OpenStreetMap:
                    Global.MapView.ZoomLevelSet = new SphericalMercatorZoomLevelSet();
                    Global.ThinkGeoCloudMapsOverlay.IsVisible = false;
                    Global.OpenStreetMapOverlay.IsVisible = true;
                    Global.BingMapsAerialOverlay.IsVisible = false;
                    Global.BingMapsRoadOverlay.IsVisible = false;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.BingMapsAerial:
                case BaseMapType.BingMapsRoad:
                    Global.MapView.ZoomLevelSet = new SphericalMercatorZoomLevelSet();
                    string result = preferences.GetString(Global.PREFS_BINGMAPKEY, string.Empty);
                    if (!string.IsNullOrEmpty(result))
                    {
                        ((BingMapsOverlay)Global.MapView.Overlays[Global.BingMapsAerialOverlayKey]).ApplicationId = result;
                        ((BingMapsOverlay)Global.MapView.Overlays[Global.BingMapsRoadOverlayKey]).ApplicationId = result;

                        Global.MapView.Overlays[Global.ThinkGeoCloudMapsOverlayKey].IsVisible = false;
                        Global.MapView.Overlays[Global.OpenStreetMapOverlayKey].IsVisible = false;
                        Global.MapView.Overlays[Global.BingMapsAerialOverlayKey].IsVisible = Global.BaseMapType == BaseMapType.BingMapsAerial;
                        Global.MapView.Overlays[Global.BingMapsRoadOverlayKey].IsVisible = Global.BaseMapType == BaseMapType.BingMapsRoad;
                    }
                    else
                    {
                        InputBingMapKeyDialog dialog = new InputBingMapKeyDialog(context, this, preferences);
                        dialog.Show();
                    }
                    break;
            }
        }

        private void BaseMapOkButton_Click(object sender, EventArgs e)
        {
            RefreshBaseMap();
            Cancel();
        }

        private void RefreshBaseMapTypeControls()
        {
            switch (Global.BaseMapType)
            {
                case BaseMapType.ThinkGeoCloudLightMap:
                    wmkRoadRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudAerialMap:
                    wmkAerialRadioButton.Checked = true;
                    break;
                case BaseMapType.ThinkGeoCloudHybridMap:
                    wmkAerialWithLabelsRadioButton.Checked = true;
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