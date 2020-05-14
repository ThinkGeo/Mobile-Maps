using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using System.IO;
using ThinkGeo.Cloud;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeoCloudVectorMapsSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string cloudServiceClientId = "Your-ThinkGeo-Cloud-Service-Cliend-ID";    // Get it from https://cloud.thinkgeo.com
        private const string cloudServiceClientSecret = "Your-ThinkGeo-Cloud-Service-Cliend-Secret";

        private ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay;
        private ThinkGeoCloudMapsOverlay satelliteOverlay;
        private string targetDirectory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Copy the required stylejson file to Device.
            targetDirectory = (@"/mnt/sdcard/ThinkGeoCloudVectorMaps/AppData/");
            CopySampleData(targetDirectory);

            // Get MapView From Activity's View.
            var androidMap = FindViewById<MapView>(Resource.Id.MapView);
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create the overlay for satellite and overlap with trasparent_background as hybrid map.
            this.satelliteOverlay = new ThinkGeoCloudMapsOverlay(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudMapsMapType.Aerial)
            {
                IsVisible = false,
                TileResolution = TileResolution.Standard,
                TileSizeMode = TileSizeMode.DefaultX2
            };
            androidMap.Overlays.Add(this.satelliteOverlay);

            // Create background world map with vector tile requested from ThinkGeo Cloud Service. 
            this.thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(cloudServiceClientId, cloudServiceClientSecret);
            androidMap.Overlays.Add(this.thinkGeoCloudVectorMapsOverlay);

            androidMap.CurrentExtent = new RectangleShape(-12922411.9716445, 8734539.23446158, -8568181.07911278, 687275.650686126);

            // Get UI Control From Activity's View
            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout1);
            tabLayout.TabSelected += TabLayout_TabSelected;
        }

        private void CopySampleData(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                foreach (var filename in Assets.List("AppData"))
                {
                    var stream = Assets.Open("AppData/" + filename);
                    var fileStream = File.Create(Path.Combine(targetDirectory, filename));
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    stream.Close();
                }
            }
        }

        private void TabLayout_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            var tabLayout = sender as TabLayout;
            switch (tabLayout.SelectedTabPosition)
            {
                case 0:
                    this.satelliteOverlay.IsVisible = false;
                    this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Light;
                    break;
                case 1:
                    this.satelliteOverlay.IsVisible = false;
                    this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Dark;
                    break;
                case 2:
                    this.satelliteOverlay.IsVisible = true;
                    this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.TransparentBackground;
                    break;
                case 3:
                    this.satelliteOverlay.IsVisible = false;
                    this.thinkGeoCloudVectorMapsOverlay.StyleJsonUri = new System.Uri(Path.Combine(targetDirectory, "mutedblue.json"));
                    break;
            }

            this.thinkGeoCloudVectorMapsOverlay.Refresh();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}

