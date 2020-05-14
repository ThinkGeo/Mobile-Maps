/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using System;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using UIKit;

namespace ThinkGeoCloudMapsSample
{
    public partial class MainViewController : UIViewController
    {
        public MapView MapView;
        public ThinkGeoCloudRasterMapsOverlay worldOverlay;

        public MainViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView = new MapView(View.Frame);
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            View.AddSubview(MapView);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            worldOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            // Tiles will be cached in the MyDocuments folder (Such as %APPPATH%/Documents/) by default if the TileCache property is not set.
            worldOverlay.TileCache = new XyzFileBitmapTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ThinkGeoCloudMapsTileCache"));
            MapView.Overlays.Add(worldOverlay);
            MapView.Refresh();

            UISegmentedControl segmentedControl = new UISegmentedControl(new string[] { "Light", "Dark", "Aerial", "Hybrid", "TransparentBackground" });
            segmentedControl.BackgroundColor = UIColor.FromRGB(244, 242, 238);
            segmentedControl.Frame = new CGRect(60, 20, View.Frame.Width - 70, 20);
            segmentedControl.ControlStyle = UISegmentedControlStyle.Plain;
            segmentedControl.ValueChanged += SegmentedControl_ValueChanged;
            View.AddSubview(segmentedControl);
        }

        private void SegmentedControl_ValueChanged(object sender, EventArgs e)
        {
            UISegmentedControl segmentedControl = sender as UISegmentedControl;
            worldOverlay.MapType = (ThinkGeoCloudRasterMapsMapType)(int)(segmentedControl.SelectedSegment);
            MapView.Refresh();
        }
    }
}