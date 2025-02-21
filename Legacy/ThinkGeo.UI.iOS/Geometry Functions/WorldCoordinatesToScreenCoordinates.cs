using CoreGraphics;
using System;
using System.Globalization;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class WorldCoordinatesToScreenCoordinates : BaseViewController
    {
        private UILabel resultView;
        private UITextView longitudeTextView;
        private UITextView latitudeTextView;

        public WorldCoordinatesToScreenCoordinates()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay();
            MapView.Overlays.Add(markerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(worldLayer);

            Marker thinkGeoMarker = new Marker();
            thinkGeoMarker.Position = new PointShape(-95.2806, 38.9554);
            thinkGeoMarker.YOffset = -22;
            thinkGeoMarker.SetImage(UIImage.FromBundle("Pin"), UIControlState.Normal);
            markerOverlay.Markers.Add(thinkGeoMarker);

            MapView.Refresh();
        }

        private void ConvertButton_TouchUpInside(object sender, EventArgs e)
        {
            ScreenPointF screenPoint = MapUtil.ToScreenCoordinate(MapView.CurrentExtent, new PointShape(Double.Parse(longitudeTextView.Text, CultureInfo.InvariantCulture), Double.Parse(latitudeTextView.Text, CultureInfo.InvariantCulture)), (float)MapView.Frame.Width, (float)MapView.Frame.Height);
            resultView.Text = string.Format("Screen Position:({0:N4},{1:N4})", screenPoint.X, screenPoint.Y);
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 220 : 160, (contentView) =>
            {
                UILabel longitudeLabelView = new UILabel(new CGRect(0, 0, 100, 35));
                longitudeLabelView.Text = "Longitude:";
                longitudeLabelView.TextColor = UIColor.White;
                longitudeLabelView.ShadowColor = UIColor.Gray;
                longitudeLabelView.ShadowOffset = new CGSize(1, 1);

                longitudeTextView = new UITextView(new CGRect(100, 0, 150, 28));
                longitudeTextView.Text = "-95.2806";

                UILabel latitudeLabelView = new UILabel(new CGRect(0, 35, 100, 35));
                latitudeLabelView.Text = "Latitude:";
                latitudeLabelView.TextColor = UIColor.White;
                latitudeLabelView.ShadowColor = UIColor.Gray;
                latitudeLabelView.ShadowOffset = new CGSize(1, 1);

                latitudeTextView = new UITextView(new CGRect(100, 35, 150, 28));
                latitudeTextView.Text = "38.9554";

                UIButton convertButton = UIButton.FromType(UIButtonType.RoundedRect);
                convertButton.Frame = new CGRect(270, 2, 80, 58);
                convertButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                convertButton.TouchUpInside += ConvertButton_TouchUpInside;
                convertButton.SetTitle("Convert", UIControlState.Normal);

                resultView = new UILabel();
                resultView.Frame = new CGRect(360, 20, 400, 30);
                resultView.TextColor = UIColor.White;
                resultView.ShadowColor = UIColor.Gray;
                resultView.ShadowOffset = new CGSize(1, 1);

                if (SampleUIHelper.IsOnIPhone)
                {
                    convertButton.Frame = new CGRect(0, 70, 80, 30);
                    resultView.Frame = new CGRect(0, 100, 400, 30);
                }

                contentView.AddSubviews(new UIView[] { longitudeLabelView, latitudeLabelView, longitudeTextView, latitudeTextView, resultView, convertButton });
            });
        }
    }
}