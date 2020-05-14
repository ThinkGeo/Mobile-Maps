using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;

namespace MapSuiteEarthquakeStatistics
{
    public class SelectDisplayTypeDialog : AlertDialog
    {
        private DisplayType displayType;
        private DisplayType oldDisplayType;
        private View selectDisplayTypeView;

        private LayerOverlay earthquakeOverlay;
        private HeatLayer earthquakeHeatLayer;
        private ShapeFileFeatureLayer earthquakePointLayer;

        private RadioButton heatRadioButton;
        private RadioButton isoRadioButton;
        private RadioButton pointRadioButton;

        private Button displayTypeOkButton;
        private Button displayTypeCancelButton;

        public SelectDisplayTypeDialog(Context context, DisplayType displayType)
            : base(context)
        {
            selectDisplayTypeView = View.Inflate(context, Resource.Layout.SelectDisplayStyleTypeLayout, null);
            this.SetView(selectDisplayTypeView);

            this.displayType = displayType;

            earthquakeOverlay = Global.MapView.Overlays[Global.EarthquakeOverlayKey] as LayerOverlay;
            earthquakePointLayer = earthquakeOverlay.Layers[Global.EarthquakePointLayerKey] as ShapeFileFeatureLayer;
            earthquakeHeatLayer = earthquakeOverlay.Layers[Global.EarthquakeHeatLayerKey] as HeatLayer;

            heatRadioButton = selectDisplayTypeView.FindViewById<RadioButton>(Resource.Id.HeatStyleRadioButton);
            isoRadioButton = selectDisplayTypeView.FindViewById<RadioButton>(Resource.Id.IsoLineStyleRadioButton);
            pointRadioButton = selectDisplayTypeView.FindViewById<RadioButton>(Resource.Id.PointStyleRadioButton);

            displayTypeOkButton = selectDisplayTypeView.FindViewById<Button>(Resource.Id.OkButton);
            displayTypeCancelButton = selectDisplayTypeView.FindViewById<Button>(Resource.Id.CancelButton);

            heatRadioButton.CheckedChange += DisplayTypeRadioButton_CheckedChange;
            isoRadioButton.CheckedChange += DisplayTypeRadioButton_CheckedChange;
            pointRadioButton.CheckedChange += DisplayTypeRadioButton_CheckedChange;

            displayTypeCancelButton.Click += DisplayTypeCancelButton_Click;
            displayTypeOkButton.Click += DisplayTypeOkButton_Click;
        }

        public override void Show()
        {
            oldDisplayType = displayType;
            base.Show();
        }

        private void DisplayTypeRadioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (sender.Equals(heatRadioButton) && heatRadioButton.Checked)
            {
                displayType = DisplayType.Heat;
            }
            else if (sender.Equals(isoRadioButton) && isoRadioButton.Checked)
            {
                displayType = DisplayType.ISOLine;
            }
            else if (sender.Equals(pointRadioButton) && pointRadioButton.Checked)
            {
                displayType = DisplayType.Point;
            }
        }

        private void DisplayTypeOkButton_Click(object sender, EventArgs e)
        {
            this.Cancel();

            earthquakePointLayer.IsVisible = false;
            earthquakeHeatLayer.IsVisible = false;

            if (displayType == DisplayType.Heat)
            {
                earthquakeHeatLayer.IsVisible = true;
            }
            else if (displayType == DisplayType.Point)
            {
                earthquakePointLayer.IsVisible = true;
            }

            earthquakeOverlay.Refresh();
        }

        private void DisplayTypeCancelButton_Click(object sender, EventArgs e)
        {
            this.Cancel();

            displayType = oldDisplayType;
            switch (displayType)
            {
                case DisplayType.Heat:
                    heatRadioButton.Checked = true;
                    break;
                case DisplayType.ISOLine:
                    isoRadioButton.Checked = true;
                    break;
                case DisplayType.Point:
                    pointRadioButton.Checked = true;
                    break;
            }
        }
    }
}