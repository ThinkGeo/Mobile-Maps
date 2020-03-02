
using Android.Content;
using Android.Views;
using Android.Widget;

namespace LabelingStyle
{
    public class CustomLabelingStyleSettingsDialog : BaseSettingsDialog<CustomLabelingStyleSettings>
    {
        private EditText maxFontSizeEditText;
        private EditText minFontSizeEditText;

        public CustomLabelingStyleSettingsDialog(Context context, CustomLabelingStyleSettings settings)
            : base(context, settings)
        {
            Width = 500;

            Title = settings.Title;
            View.Inflate(context, Resource.Layout.CustomLabelingSettingsLayout, SettingsContainerView);
            maxFontSizeEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.MaxFontSizeEditText);
            minFontSizeEditText = SettingsContainerView.FindViewById<EditText>(Resource.Id.MinFontSizeEditText);
        }

        protected override void InitalizeDialog()
        {
            maxFontSizeEditText.Text = Settings.MaxFontSize;
            minFontSizeEditText.Text = Settings.MinFontSize;
        }

        protected override void SaveSettings()
        {
            Settings.MaxFontSize = maxFontSizeEditText.Text;
            Settings.MinFontSize = minFontSizeEditText.Text;
        }
    }
}