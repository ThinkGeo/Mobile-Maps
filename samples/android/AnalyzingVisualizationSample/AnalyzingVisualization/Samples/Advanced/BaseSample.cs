/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;

namespace AnalyzingVisualization
{
    public abstract class BaseSample
    {
        private MapView mapView;
        private View sampleView;
        private TextView titleTextView;
        private ImageButton settingsButton;
        public EventHandler<EventArgs> SampleListButtonClick;

        public BaseSample(Context context)
        {
            sampleView = View.Inflate(context, Resource.Layout.SampleBaseLayout, null);
            titleTextView = sampleView.FindViewById<TextView>(Resource.Id.TitleTextView);
            settingsButton = sampleView.FindViewById<ImageButton>(Resource.Id.SettingsButton);
            settingsButton.Click += (s, e) => ApplySettings();
            sampleView.FindViewById<ImageButton>(Resource.Id.SampleListButton).Click += OnSampleListButtonClick;
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
        protected MapView MapView
        {
            get { return mapView; }
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
                mapView.MapUnit = GeographyUnit.Meter;
                mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                mapView.SetBackgroundColor(Color.Argb(255, 244, 242, 238));
                mapContainerView.AddView(mapView);

                InitializeBackgroundMap();
                InitializeMap();
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

        /// <summary>
        /// This method customizes the map for a specific sample.
        /// We could add overlays, layers, styles inside of this method.
        /// </summary>
        protected abstract void InitializeMap();

        /// <summary>
        /// Applies the settings when clicked the settings apply button.
        /// </summary>
        protected virtual void ApplySettings()
        { }

        protected virtual void OnSampleListButtonClick(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = SampleListButtonClick;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Initalizes the base map for the MapView.
        /// </summary>
        private void InitializeBackgroundMap()
        {
            if (!MapView.Overlays.Contains("WMK"))
            {
                // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");

                string baseFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                string cachePathFilename = System.IO.Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
                thinkGeoCloudMapsOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "SphericalMercator");
                MapView.Overlays.Insert(0, "WMK", thinkGeoCloudMapsOverlay);
            }
        }
    }
}