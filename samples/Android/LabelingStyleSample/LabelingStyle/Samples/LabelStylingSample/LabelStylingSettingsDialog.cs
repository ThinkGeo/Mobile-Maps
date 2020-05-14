using Android.Content;
using Android.Views;
using Android.Widget;
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelStylingSettingsDialog : BaseSettingsDialog<LabelStylingStyleSettings>
    {
        private static readonly string[] GridSizeLabelItems = { "Small", "Medium", "Large" };
        private static readonly string[] DuplicateRuleItems = { "OneDuplicateLabelPerQuadrant", "NoDuplicateLabels", "UnlimitedDuplicateLabels" };

        private Spinner duplicateRuleSpinner;
        private Spinner gridSizeSpinner;
        private CheckBox overlappingCheckBox;
        private CheckBox backgroundMaskCheckBox;
        private CheckBox outlineCheckBox;
        private EditText drawingMarginsEditText;

        public LabelStylingSettingsDialog(Context context, LabelStylingStyleSettings settings)
            : base(context, settings)
        {
            Width = 500;

            Title = settings.Title;
            View.Inflate(context, Resource.Layout.LabelStylingSettingsLayout, SettingsContainerView);
            duplicateRuleSpinner = SettingsContainerView.FindViewById<Spinner>(Resource.Id.DuplicateRuleSpinner);
            gridSizeSpinner = SettingsContainerView.FindViewById<Spinner>(Resource.Id.GridSizeSpinner);
            overlappingCheckBox = SettingsContainerView.FindViewById<CheckBox>(Resource.Id.OverlappingCheckBox);
            backgroundMaskCheckBox = SettingsContainerView.FindViewById<CheckBox>(Resource.Id.MaskCheckBox);
            outlineCheckBox = SettingsContainerView.FindViewById<CheckBox>(Resource.Id.OutlineColorCheckBox);
            drawingMarginsEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.drawingMarginsText);
        }

        protected override void InitalizeDialog()
        {
            ArrayAdapter<string> duplicateRuleAdapter = new ArrayAdapter<string>(Context, Resource.Layout.SampleSpinnerCheckedText, DuplicateRuleItems);
            duplicateRuleSpinner.Adapter = duplicateRuleAdapter;

            ArrayAdapter<string> gridSizeAdapter = new ArrayAdapter<string>(Context, Resource.Layout.SampleSpinnerCheckedText, GridSizeLabelItems);
            gridSizeSpinner.Adapter = gridSizeAdapter;

            overlappingCheckBox.Checked = Settings.LabelsOverlappingEachOther;
            backgroundMaskCheckBox.Checked = Settings.ApplyBackgroundMask;
            outlineCheckBox.Checked = Settings.ApplyOutlineColor;
            gridSizeSpinner.SetSelection((int)Settings.GridSize);
            duplicateRuleSpinner.SetSelection((int)Settings.DuplicateRule);
            drawingMarginsEditText.Text = Settings.DrawingMarginPercentage;
        }

        protected override void SaveSettings()
        {
            Settings.LabelsOverlappingEachOther = overlappingCheckBox.Checked;
            Settings.ApplyBackgroundMask = backgroundMaskCheckBox.Checked;
            Settings.ApplyOutlineColor = outlineCheckBox.Checked;
            Settings.GridSize = (LabelGridSize)gridSizeSpinner.SelectedItemPosition;
            Settings.DuplicateRule = (LabelDuplicateRule)duplicateRuleSpinner.SelectedItemPosition;
            Settings.DrawingMarginPercentage = drawingMarginsEditText.Text;
        }
    }
}