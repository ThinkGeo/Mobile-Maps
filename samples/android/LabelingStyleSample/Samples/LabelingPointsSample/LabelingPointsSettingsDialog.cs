
using Android.Content;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    class LabelingPointsSettingsDialog : BaseSettingsDialog<LabelingPointsStyleSettings>
    {
        private static readonly string[] PointPlacementItems = new string[] { "UpperLeft", "UpperCenter", "UpperRight", "CenterRight", "Center", "CenterLeft", "LowerLeft", "LowerCenter", "LowerRight" };

        private EditText xOffsetEditText;
        private EditText yOffsetEditText;
        private Spinner pointPlacementSpinner;

        public LabelingPointsSettingsDialog(Context context, LabelingPointsStyleSettings settings)
            : base(context, settings)
        {
            Width = 500;

            Title = settings.Title;
            View.Inflate(context, Resource.Layout.LabelingPointsSettingsLayout, SettingsContainerView);
            xOffsetEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.xOffsetEditText);
            yOffsetEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.yOffsetEditText);
            pointPlacementSpinner = SettingsContainerView.FindViewById<Spinner>(Resource.Id.PointPlacementSpinner);
        }

        protected override void InitalizeDialog()
        {
            ArrayAdapter<string> pointPlacementAdapter = new ArrayAdapter<string>(Context, Resource.Layout.SampleSpinnerCheckedText, PointPlacementItems);
            pointPlacementSpinner.Adapter = pointPlacementAdapter;

            xOffsetEditText.Text = Settings.XOffset;
            yOffsetEditText.Text = Settings.YOffset;
            pointPlacementSpinner.SetSelection((int)Settings.Placement);
        }

        protected override void SaveSettings()
        {
            Settings.XOffset = xOffsetEditText.Text;
            Settings.YOffset = yOffsetEditText.Text;
            Settings.Placement = (TextPlacement)pointPlacementSpinner.SelectedItemPosition;
        }
    }
}