using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // See the implementation of the new layer and feature source below.
            SimpleCsvFeatureLayer csvLayer = new SimpleCsvFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Data/Csv/vehicle-route.csv"));

            // Set the points image to an car icon and then apply it to all zoomlevels
            PointStyle vehiclePointStyle = new PointStyle(new GeoImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Resources/vehicle-location.png")));
            vehiclePointStyle.YOffsetInPixel = -12;

            csvLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = vehiclePointStyle;
            csvLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
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
        public string CsvPathFileName { get; set; }

        private Collection<Feature> features;

        public SimpleCsvFeatureSource(string csvPathFileName)
        {
            this.CsvPathFileName = csvPathFileName;
            features = new Collection<Feature>();
        }

        protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        {
            // If we haven't loaded the CSV then load it and return all the features
            if (features.Count == 0)
            {
                string[] locations = File.ReadAllLines(CsvPathFileName);

                foreach (var location in locations)
                {
                    features.Add(new Feature(double.Parse(location.Split(',')[0]), double.Parse(location.Split(',')[1])));
                }
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
            this.FeatureSource = new SimpleCsvFeatureSource(csvPathFileName);
        }

        public override bool HasBoundingBox => true;
    }
}
