using Android.App;
using Android.OS;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;

namespace Building3D
{
    [Activity(Label = "Building3D", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly static string AssetsDataDictionary = @"AppData";
        private readonly static string SampleDataDictionary = @"mnt/sdcard/MapSuiteSampleData/";

        private MapView mapView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            UploadDataFiles(AssetsDataDictionary, SampleDataDictionary);

            mapView = FindViewById<MapView>(Resource.Id.Map);
            mapView.MapUnit = GeographyUnit.Meter;

            //OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay();
            //mapView.Overlays.Add(osmOverlay);

            OsmBuildingOverlay buildingOverlay = new OsmBuildingOverlay();
            string buildingFilePath = @"osm_buildings_900913_min.shp";
            var shapeFileFeatureSource = new ShapeFileFeatureSource(Path.Combine(SampleDataDictionary, AssetsDataDictionary, buildingFilePath));
            buildingOverlay.BuildingFeatureSource = shapeFileFeatureSource;
            mapView.Overlays.Add(buildingOverlay);

            shapeFileFeatureSource.Open();
            mapView.CurrentExtent = shapeFileFeatureSource.GetBoundingBoxById("1");
            mapView.Refresh();
        }

        private void UploadDataFiles(string assetsDataDictionary, string targetDirectory)
        {
            foreach (string filename in Assets.List(assetsDataDictionary))
            {
                string targetPathFilename = Path.Combine(targetDirectory, assetsDataDictionary, filename);
                if (!File.Exists(targetPathFilename))
                {
                    string targetPath = Path.GetDirectoryName(targetPathFilename);
                    if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                    Stream sourceStream = Assets.Open(Path.Combine(assetsDataDictionary, filename));
                    FileStream fileStream = File.Create(targetPathFilename);
                    sourceStream.CopyTo(fileStream);
                    fileStream.Close();
                    sourceStream.Close();
                }
            }
        }
    }
}

