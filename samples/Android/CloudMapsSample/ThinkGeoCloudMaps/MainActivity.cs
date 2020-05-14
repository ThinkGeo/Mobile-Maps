/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeoCloudMapsSample
{
    [Activity(Label = "ThinkGeoCloudMapsSample", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MapView map1;
        private ThinkGeoCloudRasterMapsOverlay worldOverlay;
        private readonly static string SampleDataDictionary = @"/mnt/sdcard/MapSuiteSampleData/";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            ImageButton btnMapType = FindViewById<ImageButton>(Resource.Id.SettingsButton);
            btnMapType.Click += (s, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, btnMapType);
                menu.Inflate(Resource.Menu.popup_menu);
                menu.MenuItemClick += MapTypeChanged;
                menu.Show();
            };

            map1 = FindViewById<MapView>(Resource.Id.map1);
            map1.MapUnit = GeographyUnit.Meter;
            map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            worldOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            // Tiles will be cached in the MyDocuments folder (Such as %APPPATH%/Documents/) by default if the TileCache property is not set.
            worldOverlay.TileCache = new XyzFileBitmapTileCache(Path.Combine(SampleDataDictionary, "ThinkGeoTileCache"));
            map1.Overlays.Add(worldOverlay);

            map1.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);
            map1.Refresh();
        }

        private void MapTypeChanged(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            worldOverlay.MapType = (ThinkGeoCloudRasterMapsMapType)Enum.Parse(typeof(ThinkGeoCloudRasterMapsMapType), e.Item.ToString());
            map1.Refresh();
        }
    }
}

