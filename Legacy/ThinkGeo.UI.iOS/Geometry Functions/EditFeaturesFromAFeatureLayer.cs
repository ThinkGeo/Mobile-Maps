using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class EditFeaturesFromAFeatureLayer : BaseViewController
    {
        public EditFeaturesFromAFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = (new RectangleShape(0, 100, 100, 0));

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.InternalFeatures.Add("Polygon", new Feature("POLYGON((10 60,40 70,30 85, 10 60))"));
            inMemoryLayer.InternalFeatures.Add("Multipoint", new Feature("MULTIPOINT(10 20, 30 20,40 20, 10 30, 30 30, 40 30)"));
            inMemoryLayer.InternalFeatures.Add("Line", new Feature("LINESTRING(60 60, 70 70,75 60, 80 70, 85 60,95 80)"));
            inMemoryLayer.InternalFeatures.Add("Rectangle", new Feature(new RectangleShape(65, 30, 95, 15)));

            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.RoyalBlue));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen = new GeoPen(GeoColor.FromArgb(200, GeoColors.Red), 5);
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.Green), 8);
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay inmemoryOverlay = new LayerOverlay();
            inmemoryOverlay.TileType = TileType.SingleTile;
            inmemoryOverlay.Layers.Add(new BackgroundLayer(new GeoSolidBrush(GeoColors.White)));
            inmemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InMemoryOverlay", inmemoryOverlay);

            MapView.Refresh();
        }

        private void editFeatureButton_TouchDown(object sender, EventArgs e)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)MapView.Overlays["InMemoryOverlay"];
            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];

            inMemoryLayer.Open();
            inMemoryLayer.EditTools.BeginTransaction();
            inMemoryLayer.EditTools.Update(new Feature("POLYGON((10 60,40 70,30 85,20 90,10 60))", "Polygon"));
            inMemoryLayer.EditTools.CommitTransaction();
            inMemoryLayer.Close();

            inMemoryOverlay.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, (contentView) =>
            {
                UIButton editFeatureButton = UIButton.FromType(UIButtonType.RoundedRect);
                editFeatureButton.Frame = new CGRect(0, 0, 100, 30);
                editFeatureButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                editFeatureButton.TouchDown += editFeatureButton_TouchDown;
                editFeatureButton.SetTitle("Edit a feature", UIControlState.Normal);

                contentView.AddSubview(editFeatureButton);
            });
        }
    }
}