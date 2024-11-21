using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class FindTheDifferenceOfTwoFeatures : BaseViewController
    {
        public FindTheDifferenceOfTwoFeatures()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.RoyalBlue;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add("AreaShape1", new Feature(new RectangleShape(10, 50, 50, 10).GetWellKnownBinary(), "AreaShape1"));
            inMemoryLayer.InternalFeatures.Add("AreaShape2", new Feature(new RectangleShape(30, 80, 80, 30).GetWellKnownBinary(), "AreaShape2"));

            LayerOverlay inmemoryOverlay = new LayerOverlay();
            inmemoryOverlay.TileType = TileType.SingleTile;
            inmemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InMemoryOverlay", inmemoryOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, (contentView) =>
            {
                UIButton differenceButton = UIButton.FromType(UIButtonType.RoundedRect);
                differenceButton.Frame = new CGRect(0, 0, 80, 30);
                differenceButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                differenceButton.TouchUpInside += DifferenceButton_TouchUpInside;
                differenceButton.SetTitle("Difference", UIControlState.Normal);
                contentView.AddSubview(differenceButton);
            });
        }

        private void DifferenceButton_TouchUpInside(object sender, EventArgs e)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)MapView.Overlays["InMemoryOverlay"];
            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];

            if (inMemoryLayer.InternalFeatures.Count > 1)
            {
                AreaBaseShape sourceShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["AreaShape2"].GetShape();
                AreaBaseShape targetShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["AreaShape1"].GetShape();
                AreaBaseShape resultShape = sourceShape.GetDifference(targetShape);

                inMemoryLayer.InternalFeatures.Clear();
                inMemoryLayer.InternalFeatures.Add("ResultFeature", new Feature(resultShape.GetWellKnownBinary(), "ResultFeature"));
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Blue));
                inMemoryOverlay.Refresh();
            }
        }
    }
}