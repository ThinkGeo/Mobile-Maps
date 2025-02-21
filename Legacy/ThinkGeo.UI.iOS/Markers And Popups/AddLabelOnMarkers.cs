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
            MapView.TouchDown += MapViewOnTouchDown;

            // ThinkGeoCloudRasterMapsOverlay worldOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            // MapView.Overlays.Add(worldOverlay);

            SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay();
            MapView.Overlays.Add("MarkerOverlay", markerOverlay);

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        private void MapViewOnTouchDown(object? sender, TouchMapViewEventArgs e)
        {
            var position = e.PointInWorldCoordinate;
            
            SimpleMarkerOverlay markerOverlay = (SimpleMarkerOverlay)MapView.Overlays["MarkerOverlay"];

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