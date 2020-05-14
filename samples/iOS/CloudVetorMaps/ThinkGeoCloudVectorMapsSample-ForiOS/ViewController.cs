using CoreGraphics;
using System;
using ThinkGeo.Cloud;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using UIKit;

namespace ThinkGeoCloudVectorMapsSample_ForiOS
{
    public partial class ViewController : UIViewController
    {
        private const string cloudServiceClientId = "Your-ThinkGeo-Cloud-Service-Cliend-ID";    // Get it from https://cloud.thinkgeo.com
        private const string cloudServiceClientSecret = "Your-ThinkGeo-Cloud-Service-Cliend-Secret";

        private ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay;
        private ThinkGeoCloudMapsOverlay satelliteOverlay;

        private MapView mapView;
        private UICollectionView uICollectionView;
        private UILabel uILabel;
        private UISegmentedControl uISegmentedControl;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            this.mapView = new MapView(new CoreGraphics.CGRect(new CGPoint(View.Frame.Left, View.Frame.Top), new CGSize(View.Frame.Width, View.Frame.Height - tabBar.Frame.Height)));
            View.AddSubview(this.mapView);

            this.mapView.MapUnit = GeographyUnit.Meter;
            this.mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create the overlay for satellite and overlap with trasparent_background as hybrid map.
            this.satelliteOverlay = new ThinkGeoCloudMapsOverlay(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudMapsMapType.Aerial)
            {
                IsVisible = false,
                TileResolution = TileResolution.Standard,
                TileSizeMode = TileSize.Medium
            };
            this.mapView.Overlays.Add("rasterOverlay", this.satelliteOverlay);

            // Create background world map with vector tile requested from ThinkGeo Cloud Service. 
            this.thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(cloudServiceClientId, cloudServiceClientSecret);
            this.mapView.Overlays.Add("vectorOverlay", this.thinkGeoCloudVectorMapsOverlay);

            this.mapView.CurrentExtent = new RectangleShape(-10779469.7489455, 3868633.24273409, -10770297.3056023, 3863774.71415071);
            this.mapView.Refresh();

            // Create UI View for switch the type of canvas between SKIA and GDI+
            var uICollectionViewFlowLayout = new UICollectionViewFlowLayout
            {
                MinimumLineSpacing = 20f,
                MinimumInteritemSpacing = 10f,
                SectionInset = new UIEdgeInsets(20, 20, 20, 20),
            };

            // Update the layout of tab bar.
            var tabBarItems = tabBar.Items;
            if (tabBarItems != null)
            {
                var device = UIDevice.CurrentDevice.Model;

                foreach (var tabBarItem in tabBarItems)
                {
                    if (!(UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight))
                    {
                        tabBarItem.TitlePositionAdjustment = device.Contains("iPad") ? new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, 0) : new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, -tabBar.Frame.Height / 2.5f);
                    }
                    else
                    {
                        tabBarItem.TitlePositionAdjustment = new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, 0);
                    }
                }
            }
            tabBar.ItemSelected += TabBar_ItemSelected;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);
            var resolution = Math.Max(this.mapView.CurrentExtent.Width / this.mapView.Frame.Width, this.mapView.CurrentExtent.Height / this.mapView.Frame.Height);
            this.mapView.Frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(View.Frame.Left, View.Frame.Top), new CoreGraphics.CGSize(View.Frame.Width, View.Frame.Height - tabBar.Frame.Height));
            this.mapView.CurrentExtent = GetExtentRetainScale(this.mapView.CurrentExtent.GetCenterPoint(), this.mapView.Frame, resolution);
            this.mapView.Refresh();

            var tabBarItems = tabBar.Items;
            if (tabBarItems != null)
            {
                var device = UIDevice.CurrentDevice.Model;

                foreach (var tabBarItem in tabBarItems)
                {
                    if (!(UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight))
                    {
                        tabBarItem.TitlePositionAdjustment = device.Contains("iPad") ? new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, 0) : new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, -tabBar.Frame.Height / 2.5f);
                    }
                    else
                    {
                        tabBarItem.TitlePositionAdjustment = new UIOffset(tabBarItem.TitlePositionAdjustment.Horizontal, 0);
                    }
                }
            }
            this.uICollectionView.Frame = new CGRect(this.mapView.Frame.Right - 208, this.mapView.Frame.Top + 40, 180, 90);
            this.uILabel.Frame = new CGRect(this.uICollectionView.Frame.Left + 10, this.uICollectionView.Frame.Top - 5, this.uICollectionView.Frame.Width - 20, this.uICollectionView.Frame.Height / 2);
            this.uISegmentedControl.Frame = new CGRect(this.uILabel.Frame.Left, this.uILabel.Frame.Bottom, this.uILabel.Frame.Width, this.uILabel.Frame.Height - 10);
            tabBar.Frame = new CGRect(this.mapView.Frame.Left, this.mapView.Frame.Bottom, this.mapView.Frame.Width, tabBar.Frame.Height);
        }

        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            var left = currentLocationInMecator.X - resolution * frame.Width * .5;
            var right = currentLocationInMecator.X + resolution * frame.Width * .5;
            var top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            var bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private void TabBar_ItemSelected(object sender, UITabBarItemEventArgs e)
        {
            var selectedTabBarItem = ((UITabBar)sender).SelectedItem;
            switch (selectedTabBarItem.Title.ToLower())
            {
                case "light":
                    if (this.thinkGeoCloudVectorMapsOverlay.MapType != ThinkGeoCloudVectorMapsMapType.Light)
                    {
                        this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Light;
                        lightStyleTabBarItem.Enabled = false;
                        darkStyleTabBarItem.Enabled = true;
                        hybridStyleTabBarItem.Enabled = true;
                        customStyleTabBarItem.Enabled = true;
                        this.satelliteOverlay.IsVisible = false;
                    }
                    break;
                case "dark":
                    if (this.thinkGeoCloudVectorMapsOverlay.MapType != ThinkGeoCloudVectorMapsMapType.Dark)
                    {
                        this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Dark;
                        lightStyleTabBarItem.Enabled = true;
                        darkStyleTabBarItem.Enabled = false;
                        hybridStyleTabBarItem.Enabled = true;
                        customStyleTabBarItem.Enabled = true;
                        this.satelliteOverlay.IsVisible = false;
                    }
                    break;
                case "hybrid":
                    if (this.thinkGeoCloudVectorMapsOverlay.MapType != ThinkGeoCloudVectorMapsMapType.TransparentBackground)
                    {
                        this.thinkGeoCloudVectorMapsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.TransparentBackground;
                        lightStyleTabBarItem.Enabled = true;
                        darkStyleTabBarItem.Enabled = true;
                        hybridStyleTabBarItem.Enabled = false;
                        customStyleTabBarItem.Enabled = true;
                        this.satelliteOverlay.IsVisible = true;
                    }
                    break;
                case "sea blue (custom)":
                    if (this.thinkGeoCloudVectorMapsOverlay.MapType != ThinkGeoCloudVectorMapsMapType.CustomizedByStyleJson)
                    {
                        this.thinkGeoCloudVectorMapsOverlay.StyleJsonUri = new Uri("AppData/mutedblue.json", UriKind.Relative);
                        lightStyleTabBarItem.Enabled = true;
                        darkStyleTabBarItem.Enabled = true;
                        hybridStyleTabBarItem.Enabled = true;
                        customStyleTabBarItem.Enabled = false;
                        this.satelliteOverlay.IsVisible = false;
                    }
                    break;
            }

            this.thinkGeoCloudVectorMapsOverlay.Refresh();
        }
    }
}