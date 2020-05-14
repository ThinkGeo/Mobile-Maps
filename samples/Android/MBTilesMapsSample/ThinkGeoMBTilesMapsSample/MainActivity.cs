using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeoMBTilesMapsSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string targetDirectory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Copy the required stylejson file to Device.
            targetDirectory = (@"/mnt/sdcard/ThinkGeoMBTilesMapsSample/AppData/");
            CopySampleData(targetDirectory);

            // Get MapView From Activity's View.
            MapView androidMap = FindViewById<MapView>(Resource.Id.MapView);
            androidMap.MapUnit = GeographyUnit.Meter;

            // Create background map for Frisco with MB tile requested from mbtiles Database.  
            ThinkGeoMBTilesFeatureLayer thinkGeoMBTilesFeatureLayer = new ThinkGeoMBTilesFeatureLayer(
                Path.Combine(targetDirectory, "tiles_Frisco.mbtiles"),
                new Uri(Path.Combine(targetDirectory, "thinkgeo-world-streets-light.json"), UriKind.Relative)
                );

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.MaxExtent = thinkGeoMBTilesFeatureLayer.GetTileMatrixBoundingBox();
            layerOverlay.Layers.Add(thinkGeoMBTilesFeatureLayer);

            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.Overlays.Add(layerOverlay);
            androidMap.CurrentExtent = new RectangleShape(-10780508.5162109, 3916643.16078401, -10775922.2945393, 3914213.89649231);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void CopySampleData(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                foreach (string filename in Assets.List("AppData"))
                {
                    Stream stream = Assets.Open("AppData/" + filename);
                    FileStream fileStream = File.Create(Path.Combine(targetDirectory, filename));
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    stream.Close();
                }
            }
        }
    }
}

