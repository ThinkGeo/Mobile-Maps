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
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;

namespace GeometricFunctions
{
    public abstract class BaseSample
    {
        protected static readonly GeoColor BrushColor = GeoColor.FromArgb(100, 0, 147, 221);

        private MapView mapView;
        private View sampleView;
        private TextView titleTextView;
        private SliderView sliderView;
        private Button executeButton;
        private Collection<Feature> geometrySource;

        protected BaseSample(Context context, SliderView sliderView)
        {
            this.sliderView = sliderView;
            this.sampleView = View.Inflate(context, Resource.Layout.SampleBaseLayout, null);
            this.executeButton = sampleView.FindViewById<Button>(Resource.Id.ExecuteButton);
            this.titleTextView = sampleView.FindViewById<TextView>(Resource.Id.TitleTextView);

            ImageButton sampleListButton = sampleView.FindViewById<ImageButton>(Resource.Id.SampleListButton);
            sampleListButton.Click += SampleListButtonClick;
            executeButton.Click += ExecuteButtonClick;
        }

        public string TitleText
        {
            get { return titleTextView.Text; }
            set { titleTextView.Text = value; }
        }

        public SliderView SliderView
        {
            get { return sliderView; }
        }

        public Collection<Feature> GeometrySource
        {
            get { return geometrySource ?? (geometrySource = new Collection<Feature>()); }
        }

        protected MapView MapView
        {
            get { return mapView ?? (mapView = new MapView(Application.Context)); }
        }

        public virtual View GetSampleView()
        {
            try
            {
                FrameLayout mapContainerView = sampleView.FindViewById<FrameLayout>(Resource.Id.MapContainerView);

                mapContainerView.RemoveAllViews();
                mapView = new MapView(Application.Context);
                InitalizeBaseMap();
                InitalizeMap();

                mapContainerView.AddView(mapView);
            }
            catch (Exception ex)
            {
                Log.Debug("Sample Changed", ex.Message);
            }
            return sampleView;
        }

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

        protected abstract void InitalizeMap();

        protected virtual void Execute()
        { }

        protected virtual RectangleShape GetBoundingBox()
        {
            RectangleShape mapExtent = (RectangleShape)ExtentHelper.GetBoundingBoxOfItems(GeometrySource).CloneDeep();
            mapExtent.ScaleUp(100);
            return mapExtent;
        }

        private void InitalizeBaseMap()
        {
            if (!MapView.Overlays.Contains("WMK"))
            {
                MapView.SetBackgroundColor(Color.Argb(255, 244, 242, 238));
                // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
                //string baseFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //string cachePathFilename = System.IO.Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
                //thinkGeoCloudMapsOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename);
                MapView.Overlays.Insert(0, "WMK", thinkGeoCloudMapsOverlay);
            }
        }

        private void SampleListButtonClick(object sender, EventArgs e)
        {
            sliderView.SetSlided(!sliderView.IsSlided());
        }

        private void ExecuteButtonClick(object sender, EventArgs e)
        {
            Execute();
        }
    }
}