using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class GetDistanceBetweenTwoFeatures : BaseViewController
    {
        private UILabel distanceLabel;

        public GetDistanceBetweenTwoFeatures()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(worldLayer);

            Marker usMarker = new Marker();
            usMarker.Position = new PointShape(-98.58, 39.57);
            usMarker.XOffset = -22;
            usMarker.SetImage(UIImage.FromBundle("Pin"), UIControlState.Normal);

            Marker chinaMarker = new Marker();
            chinaMarker.Position = new PointShape(104.72, 34.45);
            chinaMarker.YOffset = -22;
            chinaMarker.SetImage(UIImage.FromBundle("Pin"), UIControlState.Normal);

            SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay();
            markerOverlay.Markers.Add(usMarker);
            markerOverlay.Markers.Add(chinaMarker);
            MapView.Overlays.Add("Marker", markerOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, contentView =>
            {
                UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
                button.Frame = new CGRect(0, 0, 100, 30);
                button.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                button.TouchUpInside += Button_TouchUpInside;
                button.SetTitle("Get Distance", UIControlState.Normal);

                distanceLabel = new UILabel(new CGRect(110, 0, 200, 30));
                distanceLabel.TextColor = UIColor.White;
                distanceLabel.ShadowColor = UIColor.Gray;
                distanceLabel.ShadowOffset = new CGSize(1, 1);

                contentView.AddSubviews(new UIView[] { button, distanceLabel });
            });
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            SimpleMarkerOverlay markerOverlayr = MapView.Overlays["Marker"] as SimpleMarkerOverlay;

            Marker usMarker = markerOverlayr.Markers[0];
            Marker chinaMarker = markerOverlayr.Markers[1];

            double distance = usMarker.Position.GetDistanceTo(chinaMarker.Position, GeographyUnit.DecimalDegree, DistanceUnit.Kilometer);
            distanceLabel.Text = string.Format("{0:N4} Km", distance);
        }
    }
}