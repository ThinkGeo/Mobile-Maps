/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteEarthquakeStatistics
{
    [Activity(Label = "US Earthquake", Icon = "@drawable/sampleIcon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private SelectBaseMapTypeDialog selectBaseMapTypeDialog;
        private SelectDisplayTypeDialog selectDisplayTypeDialog;

        private RadioButton panRadioButton;
        private RadioButton polygonRadioButton;
        private RadioButton rectangleRadioButton;

        private LayerOverlay highlightOverlay;
        private LayerOverlay earthquakeOverlay;

        private ShapeFileFeatureLayer earthquakeHeatLayer;
        private ShapeFileFeatureLayer earthquakePointLayer;
        private InMemoryFeatureLayer selectedMarkerLayer;
        private InMemoryFeatureLayer highlightMarkerLayer;

        readonly string[] StoragePermissions =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        const int RequestStorageId = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            TryShowMapAsync();
        }

        async Task TryShowMapAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await ShowMapAsync();
                return;
            }

            await GetStoragePermissionsAsync();
        }

        async Task GetStoragePermissionsAsync()
        {
            const string readPermission = Manifest.Permission.ReadExternalStorage;
            const string writePermission = Manifest.Permission.WriteExternalStorage;

            if (!(CheckSelfPermission(readPermission) == (int)Permission.Granted) || !(CheckSelfPermission(writePermission) == (int)Permission.Granted))
            {
                RequestPermissions(StoragePermissions, RequestStorageId);
            }
            else
            {
                ShowMapAsync();
            }
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestStorageId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            await ShowMapAsync();
                        }
                    }
                    break;
            }
        }

        async Task ShowMapAsync()
        {
            SetContentView(Resource.Layout.Main);
            await InitializeAndroidMap();
            await InitializeDialogs();

            panRadioButton = FindViewById<RadioButton>(Resource.Id.PanButton);
            polygonRadioButton = FindViewById<RadioButton>(Resource.Id.DrawPolygonButton);
            rectangleRadioButton = FindViewById<RadioButton>(Resource.Id.DrawRectangleButton);

            Button clearButton = FindViewById<Button>(Resource.Id.ClearButton);
            Button moreOptionsButton = FindViewById<Button>(Resource.Id.MoreOptionsButton);

            panRadioButton.CheckedChange += ControlMode_CheckedChange;
            polygonRadioButton.CheckedChange += ControlMode_CheckedChange;
            rectangleRadioButton.CheckedChange += ControlMode_CheckedChange;

            clearButton.Click += ClearButton_Click;
            moreOptionsButton.Click += (sender, e) => StartActivity(typeof(ConfigurationActivity));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(Menu.None, Menu.First + 1, 1, "Base Map").SetIcon(Resource.Drawable.basemap);
            menu.Add(Menu.None, Menu.First + 2, 1, "Display Type").SetIcon(Resource.Drawable.displaytype);
            menu.Add(Menu.None, Menu.First + 3, 1, "Query Configuration").SetIcon(Resource.Drawable.searchicon);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Menu.First + 1:
                    selectBaseMapTypeDialog.Show();
                    break;
                case Menu.First + 2:
                    selectDisplayTypeDialog.Show();
                    break;
                case Menu.First + 3:
                    StartActivity(typeof(ConfigurationActivity));
                    break;
            }
            return false;
        }

        private void ControlMode_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (sender.Equals(panRadioButton) && panRadioButton.Checked)
            {
                Global.MapView.TrackOverlay.TrackMode = TrackMode.None;
            }
            else if (sender.Equals(polygonRadioButton) && polygonRadioButton.Checked)
            {
                Global.MapView.TrackOverlay.TrackMode = TrackMode.Polygon;
            }
            else if (sender.Equals(rectangleRadioButton) && rectangleRadioButton.Checked)
            {
                Global.MapView.TrackOverlay.TrackMode = TrackMode.Rectangle;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Global.ClearBackupQueriedFeatures();
            selectedMarkerLayer.InternalFeatures.Clear();
            highlightMarkerLayer.InternalFeatures.Clear();
            Global.MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

            highlightOverlay.Refresh();
            Global.MapView.TrackOverlay.Refresh();
        }

        private void TrackOverlay_TrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            MultipolygonShape resultShape = PolygonShape.Union(Global.MapView.TrackOverlay.TrackShapeLayer.InternalFeatures);

            FeatureLayer featureLayer = earthquakePointLayer;
            if (!featureLayer.IsOpen)
            { featureLayer.Open(); }
            Collection<Feature> features = featureLayer.FeatureSource.GetFeaturesWithinDistanceOf(new Feature(resultShape), Global.MapView.MapUnit, DistanceUnit.Meter, 0.0001, ReturningColumnsType.AllColumns);

            Global.BackupQueriedFeatures(features);
            Global.FilterSelectedEarthquakeFeatures(Global.GetBackupQueriedFeatures());
        }

        async Task InitializeDialogs()
        {
            Global.BaseMapType = BaseMapType.ThinkGeoCloudLightMap;

            selectBaseMapTypeDialog = new SelectBaseMapTypeDialog(this, GetSharedPreferences(Global.PREFS_NAME, 0));
            selectDisplayTypeDialog = new SelectDisplayTypeDialog(this, DisplayType.Point);
        }

        async Task InitializeAndroidMap()
        {
            string baseFolder = Application.Context.ExternalCacheDir.AbsolutePath;
            string cachePathFilename = Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);
            ProjectionConverter proj4 = Global.GetWgs84ToMercatorProjection();

            // Maps
            Global.MapView = FindViewById<MapView>(Resource.Id.androidMap);
            Global.MapView.MapUnit = GeographyUnit.Meter;
            Global.MapView.MapTools.ZoomMapTool.Visibility = ViewStates.Invisible;
            Global.MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Global.MapView.CurrentExtent = new RectangleShape(-19062735.6816748, 9273256.52450252, -5746827.16371793, 2673516.56066139);
            Global.MapView.SetBackgroundColor(new Android.Graphics.Color(255, 244, 242, 238));

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");

            // Bing - Aerial
            BingMapsOverlay bingMapsAerialOverlay = new BingMapsOverlay();
            bingMapsAerialOverlay.IsVisible = false;
            bingMapsAerialOverlay.MapType = BingMapsMapType.AerialWithLabels;
            if (isWriteable) bingMapsAerialOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "BingAerialWithLabels");

            // Bing - Road
            BingMapsOverlay bingMapsRoadOverlay = new BingMapsOverlay();
            bingMapsRoadOverlay.IsVisible = false;
            bingMapsRoadOverlay.MapType = BingMapsMapType.Road;
            if (isWriteable) bingMapsRoadOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "BingRoad");

            // Earthquake points
            earthquakePointLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usEarthquake.shp"));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(PointStyle.CreateSimpleCircleStyle(GeoColors.Red, 5, GeoColors.White, 1));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            earthquakePointLayer.FeatureSource.ProjectionConverter = proj4;

            earthquakeHeatLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usEarthquake_Simplify.shp"));
            earthquakeHeatLayer.FeatureSource.ProjectionConverter = proj4;
            earthquakeHeatLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            earthquakeHeatLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 180, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer));
            earthquakeHeatLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            earthquakeHeatLayer.IsVisible = false;

            earthquakeOverlay = new LayerOverlay();
            earthquakeOverlay.Layers.Add(Global.EarthquakePointLayerKey, earthquakePointLayer);
            earthquakeOverlay.Layers.Add(Global.EarthquakeHeatLayerKey, earthquakeHeatLayer);

            // Highlighted points
            selectedMarkerLayer = new InMemoryFeatureLayer();
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Orange, 8, GeoColors.White, 2);
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            PointStyle highLightMarkerStyle = new PointStyle();
            highLightMarkerStyle.CustomPointStyles.Add(PointStyle.CreateSimpleCircleStyle(GeoColor.FromArgb(50, GeoColors.Blue), 20, GeoColors.LightBlue, 1));
            highLightMarkerStyle.CustomPointStyles.Add(PointStyle.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 0, 122, 255), 10, GeoColors.White, 2));

            highlightMarkerLayer = new InMemoryFeatureLayer();
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = highLightMarkerStyle;
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add(Global.SelectMarkerLayerKey, selectedMarkerLayer);
            highlightOverlay.Layers.Add(Global.HighlightMarkerLayerKey, highlightMarkerLayer);

            Global.MapView.Overlays.Add(Global.ThinkGeoCloudMapsOverlayKey, thinkGeoCloudMapsOverlay);
            Global.MapView.Overlays.Add(Global.BingMapsAerialOverlayKey, bingMapsAerialOverlay);
            Global.MapView.Overlays.Add(Global.BingMapsRoadOverlayKey, bingMapsRoadOverlay);
            Global.MapView.Overlays.Add(Global.EarthquakeOverlayKey, earthquakeOverlay);
            Global.MapView.Overlays.Add(Global.HighlightOverlayKey, highlightOverlay);

            Global.MapView.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            Global.MapView.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColor.FromArgb(80, GeoColors.LightGreen), 8);
            Global.MapView.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3, true);
            Global.MapView.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.LightGreen), GeoColors.White, 2);
            Global.MapView.TrackOverlay.TrackEnded += TrackOverlay_TrackEnded;
        }
    }
}