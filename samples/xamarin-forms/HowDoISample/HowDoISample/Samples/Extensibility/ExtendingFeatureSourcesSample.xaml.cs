using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendingFeatureSourcesSample : ContentPage
    {
        public ExtendingFeatureSourcesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // See the implementation of the new layer and feature source below.
            var csvLayer = new SimpleCsvFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Data/Csv/vehicle-route.csv"));

            // Set the points image to an car icon and then apply it to all zoomlevels
            var vehiclePointStyle = new PointStyle(new GeoImage(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Resources/vehicle-location.png")));
            vehiclePointStyle.YOffsetInPixel = -12;

            csvLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = vehiclePointStyle;
            csvLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(csvLayer);
            mapView.Overlays.Add(layerOverlay);

            csvLayer.Open();
            mapView.CurrentExtent = csvLayer.GetBoundingBox();
        }
    }
    // Here we are creating a simple CVS feature source using the minimum set of overloads.
    // Since CSV doesn't include a way to do spatial queries we only need to return all the features
    // in the method below and the base class will do the rest.  Of course if you had large dataset this
    // would be slow so I recommend you look at other overloads and implement optimized versions of these methods

    public class SimpleCsvFeatureSource : FeatureSource
    {
        private readonly Collection<Feature> features;

        public SimpleCsvFeatureSource(string csvPathFileName)
        {
            CsvPathFileName = csvPathFileName;
            features = new Collection<Feature>();
        }

        public string CsvPathFileName { get; set; }

        protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        {
            // If we haven't loaded the CSV then load it and return all the features
            if (features.Count == 0)
            {
                var locations = File.ReadAllLines(CsvPathFileName);

                foreach (var location in locations)
                    features.Add(
                        new Feature(double.Parse(location.Split(',')[0]), double.Parse(location.Split(',')[1])));
            }

            return features;
        }
    }

    // We need to create a layer that wraps the feature source.  FeatureLayer has everything we need we just need
    // to provide a constructor and set the feature source and all of the methods on the feature layer just work.
    public class SimpleCsvFeatureLayer : FeatureLayer
    {
        public SimpleCsvFeatureLayer(string csvPathFileName)
        {
            FeatureSource = new SimpleCsvFeatureSource(csvPathFileName);
        }

        public override bool HasBoundingBox => true;
    }
}