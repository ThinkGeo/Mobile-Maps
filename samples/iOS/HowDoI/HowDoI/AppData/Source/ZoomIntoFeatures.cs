using CoreGraphics;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ZoomIntoFeatures : BaseViewController
    {
        public ZoomIntoFeatures()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = (new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625));

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, contentView =>
            {
                UIButton multiFeaturesButton = GetUIButton(0, "MultiFeatures", ZoomToMultiFeaturesClick);
                UIButton oneFeatureButton = GetUIButton(140, "OneFeature", ZoomToOneFeatureClick);

                contentView.AddSubviews(new UIView[] { oneFeatureButton, multiFeaturesButton });
            });
        }

        private void ZoomToMultiFeaturesClick(object sender, EventArgs e)
        {
            Collection<string> featureIDs = new Collection<string>();
            featureIDs.Add("63");  // For US
            featureIDs.Add("6");   // For Canada
            featureIDs.Add("137"); // For Mexico

            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            lock (worldLayer)
            {
                worldLayer.Open();
                Collection<Feature> features = worldLayer.FeatureSource.GetFeaturesByIds(featureIDs, ReturningColumnsType.NoColumns);
                MapView.CurrentExtent = MapUtil.GetBoundingBoxOfItems(features);

                highlightLayer.InternalFeatures.Clear();
                foreach (var feature in features)
                {
                    highlightLayer.InternalFeatures.Add(feature);
                }
            }

            MapView.Refresh();
        }

        private void ZoomToOneFeatureClick(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            lock (worldLayer)
            {
                worldLayer.Open();
                MapView.CurrentExtent = worldLayer.FeatureSource.GetBoundingBoxById("137");

                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                Feature feature = worldLayer.FeatureSource.GetFeatureById("137", ReturningColumnsType.NoColumns);
                if (feature != null)
                {
                    highlightLayer.InternalFeatures.Add(feature);
                }
            }

            MapView.Refresh();
        }

        private static UIButton GetUIButton(int leftLocation, string title, EventHandler handler)
        {
            UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
            button.Frame = new CGRect(leftLocation, 0, 130, 30);
            button.BackgroundColor = UIColor.FromRGB(241, 241, 241);
            button.TouchUpInside += handler;
            button.SetTitle(title, UIControlState.Normal);

            return button;
        }
    }
}