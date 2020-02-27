using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using ThinkGeo.MapSuite.Android;

namespace LabelingStyle
{
    public abstract class BaseSample
    {
        private Context context;
        private View sampleView;
        private MapView mapView;
        private TextView titleTextView;
        private ImageButton settingsButton;
        public EventHandler<EventArgs> SampleListButtonClick;

        public BaseSample(Context context)
        {
            this.context = context;
            sampleView = View.Inflate(context, Resource.Layout.SampleBaseLayout, null);
            settingsButton = sampleView.FindViewById<ImageButton>(Resource.Id.SettingsButton);
            titleTextView = sampleView.FindViewById<TextView>(Resource.Id.TitleTextView);
            settingsButton.Click += (s, e) => ApplySettings();
            sampleView.FindViewById<ImageButton>(Resource.Id.SampleListButton).Click += OnSampleListButtonClick;
        }

        public Context Context
        {
            get { return context; }
        }

        public string Title
        {
            get { return titleTextView.Text; }
            set { titleTextView.Text = value; }
        }

        public int ImageId { get; set; }

        /// <summary>
        /// This is map view, if it doesn't exist, we will create one.
        /// </summary>
        public MapView MapView
        {
            get { return mapView ?? (mapView = new MapView(Application.Context)); }
        }

        /// <summary>
        /// This is a container for a concrete sample. Including map and corresponding components for interaction.
        /// </summary>
        public View SampleView
        {
            get { return sampleView; }
        }

        /// <summary>
        /// This method updates the sample layout; including creating new map with default themes.
        /// </summary>
        public void UpdateSampleLayout()
        {
            try
            {
                FrameLayout mapContainerView = sampleView.FindViewById<FrameLayout>(Resource.Id.MapContainerView);
                mapContainerView.RemoveAllViews();
                mapView = new MapView(Application.Context);
                mapView.SetBackgroundColor(Color.Argb(255, 244, 242, 238));
                InitalizeMap();

                mapContainerView.AddView(mapView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
            }
            catch (Exception ex)
            {
                Log.Debug("Sample Changed", ex.Message);
            }

            // Customize the sample layout with specific sample.
            UpdateSampleLayoutCore();
        }

        /// <summary>
        /// Allows user to customize the sample layout.
        /// </summary>
        protected virtual void UpdateSampleLayoutCore()
        { }

        /// <summary>
        /// Disposes the map view. 
        /// It is necessary to dispose the map resources when current sample is changed to avoid the OOM issue.
        /// </summary>
        public void DisposeMap()
        {
            if (mapView != null && mapView.Parent != null)
            {
                FrameLayout mapContainerView = sampleView.FindViewById<FrameLayout>(Resource.Id.MapContainerView);
                mapContainerView.RemoveAllViews();

                mapView.Dispose();
                mapView = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        protected virtual void OnSampleListButtonClick(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = SampleListButtonClick;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// This method customizes the map for a specific sample.
        /// We could add overlays, layers, styles inside of this method.
        /// </summary>
        protected abstract void InitalizeMap();

        /// <summary>
        /// Applies the settings when clicked the settings apply button.
        /// </summary>
        protected virtual void ApplySettings() { }
    }
}