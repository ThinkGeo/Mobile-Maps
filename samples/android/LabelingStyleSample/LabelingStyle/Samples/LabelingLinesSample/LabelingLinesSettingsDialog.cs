using Android.Content;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    public class LabelingLinesSettingsDialog : BaseSettingsDialog<LabelingLinesStyleSettings>
    {
        private static readonly string[] SplineTypeItems = new string[] { "Default", "None", "StandardSplining", "ForceSplining" };

        private Spinner splineTypeSpinner;
        private EditText lineSegmentRatioEditText;

        public LabelingLinesSettingsDialog(Context context, LabelingLinesStyleSettings settings)
            : base(context, settings)
        {
            Width = 500;

            Title = settings.Title;
            View.Inflate(context, Resource.Layout.LabelingLinesSettingsLayout, SettingsContainerView);
            splineTypeSpinner = SettingsContainerView.FindViewById<Spinner>(Resource.Id.splineTypeSpinner);
            lineSegmentRatioEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.lineSegmentRatioEditText);
        }

        protected override void InitalizeDialog()
        {
            ArrayAdapter<string> splineTypeAdapter = new ArrayAdapter<string>(Context, Resource.Layout.SampleSpinnerCheckedText, SplineTypeItems);
            splineTypeSpinner.Adapter = splineTypeAdapter;

            splineTypeSpinner.SetSelection((int)Settings.SplineType);
            lineSegmentRatioEditText.Text = Settings.LineSegmentRatio;
        }

        protected override void SaveSettings()
        {
            Settings.SplineType = (SplineType)splineTypeSpinner.SelectedItemPosition;
            Settings.LineSegmentRatio = lineSegmentRatioEditText.Text;
        }
    }
}