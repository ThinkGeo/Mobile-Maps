using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class EnvelopeOfAFeature : BaseViewController
    {
        public EnvelopeOfAFeature()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            LayerOverlay boundingboxOverlay = new LayerOverlay();
            boundingboxOverlay.TileType = TileType.SingleTile;
            MapView.Overlays.Add("BoundingBoxOverlay", boundingboxOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer boundingBoxLayer = new InMemoryFeatureLayer();
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Green;
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            boundingboxOverlay.Layers.Add("BoundingBoxLayer", boundingBoxLayer);

            MapView.Refresh();

        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, (contentView) =>
            {
                UIButton getEnvelopeButton = UIButton.FromType(UIButtonType.RoundedRect);
                getEnvelopeButton.Frame = new CGRect(0, 0, 120, 30);
                getEnvelopeButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                getEnvelopeButton.TouchUpInside += GetEnvelopeButton_TouchDown;
                getEnvelopeButton.SetTitle("Get Envelope", UIControlState.Normal);

                contentView.AddSubview(getEnvelopeButton);
            });
        }

        private void GetEnvelopeButton_TouchDown(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            LayerOverlay boundingBoxOverlay = (LayerOverlay)MapView.Overlays["BoundingBoxOverlay"];
            InMemoryFeatureLayer boundingBoxLayer = (InMemoryFeatureLayer)boundingBoxOverlay.Layers["BoundingBoxLayer"];

            if (!boundingBoxLayer.InternalFeatures.Contains("BoundingBox"))
            {
                worldLayer.Open();
                RectangleShape usBoundingBox = worldLayer.QueryTools.GetFeatureById("137", ReturningColumnsType.NoColumns).GetBoundingBox();
                boundingBoxLayer.InternalFeatures.Add("BoundingBox", new Feature(usBoundingBox.GetWellKnownBinary(), "BoundingBox"));
                MapView.Overlays["BoundingBoxOverlay"].Refresh();
            }
        }
    }
}