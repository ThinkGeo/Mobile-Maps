using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;

namespace GettingStartedSample
{
    /// <summary>
    ///  This class represents the Open GPS Settings Dialog which will be only showed up if the GPS is not enable when this app starts.
    /// </summary>
    public class OpenGpsSettingsDialog : AlertDialog
    {
        private bool openSettings;

        public OpenGpsSettingsDialog(Context context)
            : base(context)
        {
            openSettings = false;

            View openGpsSettingsView = View.Inflate(context, Resource.Layout.OpenGpsSettingsLayout, null);
            SetView(openGpsSettingsView);

            Button cancelButton = openGpsSettingsView.FindViewById<Button>(Resource.Id.CancelButton);
            Button okButton = openGpsSettingsView.FindViewById<Button>(Resource.Id.OkButton);

            okButton.Click += OkButtonClick;
            cancelButton.Click += CancelButtonClick;
        }

        public bool OpenSettings
        {
            get { return openSettings; }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            openSettings = false;
            Cancel();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            openSettings = true;
            Cancel();
        }
    }
}