using CoreGraphics;
using System.Globalization;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class AddLabelOnMarkers : BaseViewController
    {
        private int index = 1;

        public AddLabelOnMarkers()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.MapTouchDown += MapView_MapTouchDown;

            ThinkGeoCloudRasterMapsOverlay worldOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            MapView.Overlays.Add(worldOverlay);

            MarkerOverlay markerOverlay = new MarkerOverlay();
            MapView.Overlays.Add("MarkerOverlay", markerOverlay);

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        private void MapView_MapTouchDown(object sender, UIGestureRecognizer e)
        {
            CGPoint location = e.LocationInView(MapView);
            PointShape position = MapUtil.ToWorldCoordinate(MapView.CurrentExtent, (float)location.X,
                (float)location.Y, (float)MapView.Frame.Width, (float)MapView.Frame.Height);

            MarkerOverlay markerOverlay = (MarkerOverlay)MapView.Overlays["MarkerOverlay"];

            Marker marker = new Marker();
            marker.Position = position;
            marker.SetImage(UIImage.FromFile("AQUABLANK.png"), UIControlState.Normal);

            UILabel content = new UILabel();
            content.Text = (index++).ToString(CultureInfo.InvariantCulture);

            marker.TitleLabel.Add(content);

            markerOverlay.Markers.Add(marker);
            markerOverlay.Refresh();
        }
    }
}