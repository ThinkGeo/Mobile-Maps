/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using System;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GettingStartedSample
{
    public partial class MainFormViewController : UIViewController
    {
        private MapView mapView;
        UIButton switchThemeButton;

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            mapView = new MapView(View.Frame)
            {
                MapUnit = GeographyUnit.Meter,
                CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962)
            };
            View.AddSubview(mapView);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string clientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string secret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(clientKey, secret);

            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");
            mapView.Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);
            mapView.Refresh();

            switchThemeButton = UIButton.FromType(UIButtonType.RoundedRect);
            switchThemeButton.Frame = new CGRect(View.Bounds.Width - 50, View.Bounds.Height - 50, 35, 35);
            switchThemeButton.SetImage(UIImage.FromBundle("switch-theme"), UIControlState.Normal);
            switchThemeButton.TouchUpInside += switchThemeButton_TouchUpInside;
            View.AddSubview(switchThemeButton);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            switchThemeButton.Frame = new CGRect(View.Bounds.Width - 50, View.Bounds.Height - 50, 35, 35);
            switchThemeButton.Hidden = false;

            double resolution = Math.Max(mapView.CurrentExtent.Width / mapView.Frame.Width, mapView.CurrentExtent.Height / mapView.Frame.Height);
            mapView.Frame = View.Bounds;
            mapView.CurrentExtent = GetExtentRetainScale(mapView.CurrentExtent.GetCenterPoint(), mapView.Frame, resolution);
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            switchThemeButton.Hidden = true;
        }

        private void switchThemeButton_TouchUpInside(object sender, EventArgs e)
        {
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = (ThinkGeoCloudVectorMapsOverlay)mapView.Overlays["ThinkGeoCloudMapsOverlay"];
            if (thinkGeoCloudMapsOverlay.MapType == ThinkGeoCloudVectorMapsMapType.Light)
            {
                thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Dark;
                thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_dark");
            }
            else
            {
                thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Light;
                thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            }
            thinkGeoCloudMapsOverlay.Refresh();
        }
     
        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMercator, CGRect frame, double resolution)
        {
            double left = currentLocationInMercator.X - resolution * frame.Width * .5;
            double right = currentLocationInMercator.X + resolution * frame.Width * .5;
            double top = currentLocationInMercator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMercator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }
    }
}