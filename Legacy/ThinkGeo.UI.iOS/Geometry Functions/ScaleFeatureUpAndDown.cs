using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ScaleFeatureUpAndDown : BaseViewController
    {
        public ScaleFeatureUpAndDown()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            worldLayer.Open();
            Feature feature = worldLayer.QueryTools.GetFeatureById("135", ReturningColumnsType.NoColumns);

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Green;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            inMemoryLayer.InternalFeatures.Add("135", feature);

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            inMemoryOverlay.TileType = TileType.SingleTile;
            inMemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InMemoryOverlay", inMemoryOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, (contentView) =>
            {
                UIButton scaleUpButton = UIButton.FromType(UIButtonType.RoundedRect);
                scaleUpButton.Frame = new CGRect(0, 0, 100, 30);
                scaleUpButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                scaleUpButton.TouchUpInside += ScaleUpButton_TouchUpInside;
                scaleUpButton.SetTitle("ScaleUp", UIControlState.Normal);

                UIButton scaleDownButton = UIButton.FromType(UIButtonType.RoundedRect);
                scaleDownButton.Frame = new CGRect(110, 0, 100, 30);
                scaleDownButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                scaleDownButton.TouchUpInside += ScaleDownButton_TouchUpInside;
                scaleDownButton.SetTitle("ScaleDown", UIControlState.Normal);

                contentView.AddSubviews(new UIView[] { scaleUpButton, scaleDownButton });
            });
        }

        private void ScaleUpButton_TouchUpInside(object sender, EventArgs e)
        {
            UpdateFeatureByScale(25, true);
        }

        private void ScaleDownButton_TouchUpInside(object sender, EventArgs e)
        {
            UpdateFeatureByScale(20, false);
        }

        private void UpdateFeatureByScale(double percentage, bool isScaleUp)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)MapView.Overlays["InMemoryOverlay"];
            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];

            inMemoryLayer.Open();
            inMemoryLayer.EditTools.BeginTransaction();
            if (isScaleUp) inMemoryLayer.EditTools.ScaleUp("135", percentage);
            else inMemoryLayer.EditTools.ScaleDown("135", percentage);
            inMemoryLayer.EditTools.CommitTransaction();

            inMemoryOverlay.Refresh();
        }
    }
}