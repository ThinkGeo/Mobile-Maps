using Android.Content;
using Android.Views;
using Android.Widget;

namespace LabelingStyle
{
    public class LabelingPolygonsSettingsDialog : BaseSettingsDialog<LabelingPolygonsStyleSettings>
    {
        private CheckBox onlyWithinCheckBox;
        private CheckBox polygonPartsCheckBox;

        public LabelingPolygonsSettingsDialog(Context context, LabelingPolygonsStyleSettings settings)
            : base(context, settings)
        {
            Width = 500;

            Title = settings.Title;
            View.Inflate(context, Resource.Layout.LabelingPolygonsSettingsLayout, SettingsContainerView);
            onlyWithinCheckBox = SettingsContainerView.FindViewById<CheckBox>(Resource.Id.OnlyWithinCheckBox);
            polygonPartsCheckBox = SettingsContainerView.FindViewById<CheckBox>(Resource.Id.PolygonPartsCheckBox);
        }

        protected override void InitalizeDialog()
        {
            onlyWithinCheckBox.Checked = Settings.FittingFactorsOnlyWithin;
            polygonPartsCheckBox.Checked = Settings.LabelAllPolygonParts;
        }

        protected override void SaveSettings()
        {
            Settings.FittingFactorsOnlyWithin = onlyWithinCheckBox.Checked;
            Settings.LabelAllPolygonParts = polygonPartsCheckBox.Checked;
        }
    }
}