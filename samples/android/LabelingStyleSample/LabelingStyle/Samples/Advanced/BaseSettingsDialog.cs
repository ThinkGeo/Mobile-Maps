using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;

namespace LabelingStyle
{
    public abstract class BaseSettingsDialog<T> : AlertDialog
    {
        public event EventHandler<EventArgs> ApplyingSettings;

        private int width;
        private T settings;
        private TextView titleTextView;
        private View settingsDialogView;

        public BaseSettingsDialog(Context context, T settings)
            : base(context)
        {
            this.settings = settings;
            settingsDialogView = View.Inflate(context, Resource.Layout.SettingsDialogBaseLayout, null);
            settingsDialogView.FindViewById<Button>(Resource.Id.OkButton).Click += OkButtonClick;
            settingsDialogView.FindViewById<Button>(Resource.Id.CancelButton).Click += CancelButtonClick;
            titleTextView = settingsDialogView.FindViewById<TextView>(Resource.Id.TitleTextView);

            SetCanceledOnTouchOutside(false);
            SetView(settingsDialogView);
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public T Settings
        {
            get { return settings; }
        }

        public string Title
        {
            get { return titleTextView.Text; }
            set { titleTextView.Text = value; }
        }

        public override void Show()
        {
            base.Show();
            InitalizeDialog();

            int dialogWidth = (int)(Width * Context.Resources.DisplayMetrics.Density);
            WindowManagerLayoutParams windowLayoutParams = Window.Attributes;
            windowLayoutParams.Gravity = GravityFlags.Center;
            windowLayoutParams.Width = dialogWidth > Window.WindowManager.DefaultDisplay.Width ? Window.WindowManager.DefaultDisplay.Width : dialogWidth;

            Window.Attributes = windowLayoutParams;
            Window.SetGravity(GravityFlags.Center);
        }

        protected ViewGroup SettingsContainerView
        {
            get { return settingsDialogView.FindViewById<FrameLayout>(Resource.Id.mainLayout); }
        }

        protected abstract void SaveSettings();
        protected abstract void InitalizeDialog();

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Cancel();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            SaveSettings();
            Cancel();

            OnApplyingSettings(this, e);
        }

        private void OnApplyingSettings(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = ApplyingSettings;
            if (handler != null) handler(sender, e);
        }
    }
}