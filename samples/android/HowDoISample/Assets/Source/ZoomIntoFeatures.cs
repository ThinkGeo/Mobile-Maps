using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class ZoomIntoFeatures : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            //
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add("WorldOverlay", layerOverlay);
            androidMap.Overlays.Add("HighlightOverlay", highlightOverlay);

            Button oneFeatureButton = new Button(this.Context);
            oneFeatureButton.Text = "OneFeature";
            oneFeatureButton.Click += OneFeatureButtonClick;

            Button mulitFeaturesButton = new Button(this.Context);
            mulitFeaturesButton.Text = "MultiFeatures";
            mulitFeaturesButton.Click += MulitFeaturesButtonClick;

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            linearLayout.AddView(oneFeatureButton);
            linearLayout.AddView(mulitFeaturesButton);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void OneFeatureButtonClick(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)androidMap.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)androidMap.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            lock (worldLayer)
            {
                if (!worldLayer.IsOpen) worldLayer.Open();
                androidMap.CurrentExtent = worldLayer.FeatureSource.GetBoundingBoxById("137");

                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                Feature feature = worldLayer.FeatureSource.GetFeatureById("137", ReturningColumnsType.NoColumns);
                if (feature != null)
                {
                    highlightLayer.InternalFeatures.Add(feature);
                }
                highlightLayer.Close();
            }

            androidMap.Refresh();
        }

        private void MulitFeaturesButtonClick(object sender, EventArgs e)
        {
            Collection<string> featureIDs = new Collection<string>();
            featureIDs.Add("63");  // For US
            featureIDs.Add("6");   // For Canada
            featureIDs.Add("137"); // For Mexico

            LayerOverlay worldOverlay = (LayerOverlay)androidMap.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)androidMap.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            lock (worldLayer)
            {
                if (!worldLayer.IsOpen) worldLayer.Open();
                Collection<Feature> features = worldLayer.FeatureSource.GetFeaturesByIds(featureIDs, new string[0]);
                androidMap.CurrentExtent = MapUtil.GetBoundingBoxOfItems(features);

                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                if (features.Count > 0)
                {
                    foreach (var feature in features)
                    {
                        highlightLayer.InternalFeatures.Add(feature);
                    }
                }
                highlightLayer.Close();
            }

            androidMap.Refresh();
        }
    }
}