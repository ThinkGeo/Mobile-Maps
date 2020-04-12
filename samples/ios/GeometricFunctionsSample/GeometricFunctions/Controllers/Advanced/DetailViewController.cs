/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using Foundation;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GeometricFunctions
{
    [Register("DetailViewController")]
    public class DetailViewController : UIViewController
    {
        private GeoColor brushColor;
        private UIBarButtonItem settingButton;
        private Collection<Feature> geometrySource;

        protected MapView MapView;
        public Action DetailButtonClick;

        protected DetailViewController()
        {
            geometrySource = new Collection<Feature>();
        }

        private void InitializeToolbar()
        {
            brushColor = GeoColor.FromArgb(100, 0, 147, 221);

            UIBarButtonItem detailButton = new UIBarButtonItem(UIImage.FromBundle("detail"), UIBarButtonItemStyle.Plain,
                OnDetailItemClick);
            detailButton.TintColor = UIColor.Black;
            NavigationItem.SetLeftBarButtonItem(detailButton, true);

            settingButton = new UIBarButtonItem("GO", UIBarButtonItemStyle.Plain, OnGoItemClick);
            settingButton.TintColor = UIColor.Black;
            NavigationItem.SetRightBarButtonItems(new[] { settingButton }, true);
        }

        public Collection<Feature> GeometrySource
        {
            get { return geometrySource; }
        }

        protected GeoColor BrushColor
        {
            get { return brushColor; }
        }

        protected RectangleShape GetBoundingBox()
        {
            RectangleShape mapExtent = (RectangleShape)MapUtil.GetBoundingBoxOfItems(GeometrySource).CloneDeep();
            mapExtent.ScaleUp(140);
            return mapExtent;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            MapView = new MapView(View.Frame);
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);
            MapView.BackgroundColor = UIColor.FromRGB(244, 242, 238);
            MapView.MapTools.ZoomMapTool.Center = new CGPoint(MapView.MapTools.ZoomMapTool.Center.X + 10, MapView.MapTools.ZoomMapTool.Center.Y + 55);

            InitializeToolbar();
            LoadBackgroundLayer();
            InitializeMap();

            View.AddSubview(MapView);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(MapView.CurrentExtent.Width / MapView.Frame.Width, MapView.CurrentExtent.Height / MapView.Frame.Height);
            MapView.Frame = View.Bounds;
            MapView.CurrentExtent = GetExtentRetainScale(MapView.CurrentExtent.GetCenterPoint(), MapView.Frame, resolution);
            MapView.Refresh();
        }

        protected virtual void InitializeMap() { }

        protected virtual void Execute() { }

        private void LoadBackgroundLayer()
        {
            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);
        }

        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            double left = currentLocationInMecator.X - resolution * frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private void OnGoItemClick(object sender, EventArgs e)
        {
            settingButton.Enabled = false;
            Execute();
            settingButton.Enabled = true;
        }

        private void OnDetailItemClick(object sender, EventArgs e)
        {
            DetailButtonClick?.Invoke();
        }
    }
}