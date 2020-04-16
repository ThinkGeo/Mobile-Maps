/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreAnimation;
using CoreGraphics;
using CoreLocation;
using Foundation;
using System;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GettingStartedSample
{
    public partial class MainFormViewController : UIViewController
    {
        private MapView mapView;
        private InstructionView instructionView;
        CLLocationManager locationManager;

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // InitializeMap
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
            //thinkGeoCloudMapsOverlay.TileHeight = 256;
            //thinkGeoCloudMapsOverlay.TileWidth = 256;
            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");
            mapView.Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);

            // Init Location marker layer.
            MarkerOverlay locationOverlay = new MarkerOverlay();
            mapView.Overlays.Add("LocationOverlay", locationOverlay);

            mapView.Refresh();

            // Initialize Instruction Panel
            instructionView = new InstructionView(View, RefreshInstructionView, Instruction_ExpandStateChanged);
            Add(instructionView);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            instructionView.DidRotate();
            instructionView.Hidden = false;
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(mapView.CurrentExtent.Width / mapView.Frame.Width, mapView.CurrentExtent.Height / mapView.Frame.Height);
            mapView.Frame = View.Bounds;
            mapView.CurrentExtent = GetExtentRetainScale(mapView.CurrentExtent.GetCenterPoint(), mapView.Frame, resolution);
            mapView.Refresh();
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            instructionView.Hidden = true;
        }

        private void Instruction_ExpandStateChanged(CGRect instructionFrame, bool animate)
        {
            if (animate)
            {
                UIView.BeginAnimations("MapToolsPositionChanged");
                UIView.SetAnimationDuration(0.2);
            }

            CALayer attributionLayer = mapView.EventView.Layer.Sublayers.FirstOrDefault(l => l.Name.Equals("AttributionLayer"));
            attributionLayer.Frame = new CGRect(0, -instructionFrame.Height, attributionLayer.Frame.Width, attributionLayer.Frame.Height);

            if (animate) UIView.CommitAnimations();
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

        private void LocateButton_TouchUpInside(object buttonEventSender, EventArgs buttonEventE)
        {
            locationManager = new CLLocationManager();
            locationManager.RequestWhenInUseAuthorization();
            locationManager.DesiredAccuracy = 1;

            locationManager.LocationsUpdated += delegate (object sender, CLLocationsUpdatedEventArgs e)
            {
                CLLocation gpsLocation = e.Locations.FirstOrDefault();
                if (gpsLocation == null) return;

                ProjectionConverter bingToWgs84Projection = new ProjectionConverter(3857, 4326);
                bingToWgs84Projection.Open();

                PointShape currentLocationInWgs84 = new PointShape(gpsLocation.Coordinate.Longitude, gpsLocation.Coordinate.Latitude);
                PointShape currentLocationInMercator = (PointShape)bingToWgs84Projection.ConvertToInternalProjection(currentLocationInWgs84);

                MarkerOverlay locationOverlay = (MarkerOverlay)mapView.Overlays["LocationOverlay"];
                locationOverlay.Markers.Clear();

                // Update the GPS marker position.
                GpsMarker locationMarker = new GpsMarker();
                locationMarker.HaloRadiusInMeter = (float)gpsLocation.HorizontalAccuracy;
                locationMarker.Position = currentLocationInMercator;
                locationOverlay.Markers.Add(locationMarker);

                if (locationMarker != null)
                {
                    mapView.ZoomTo(locationMarker.Position, 18023);
                }
                locationManager.StopUpdatingLocation();
            };

            locationManager.StartUpdatingLocation();
        }

        private void InformationButton_TouchUpInside(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl("https://thinkgeo.com/gis-ui-mobile"));
        }

        private void RefreshInstructionView(UIView contentView)
        {
            UIButton locateButton;
            UIButton switchThemeButton;
            UIButton informationButton;

            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            // Check the device orientation, refresh the controls layout for instruction view.
            if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
            {
                locateButton = InitializeButton(contentView.Bounds.Width - 235, "location", LocateButton_TouchUpInside);
                switchThemeButton = InitializeButton(contentView.Bounds.Width - 195, "switch-theme", switchThemeButton_TouchUpInside);
                informationButton = InitializeButton(contentView.Bounds.Width - 35, "information", InformationButton_TouchUpInside);
            }
            else
            {
                locateButton = InitializeButton(0, "location", LocateButton_TouchUpInside);
                switchThemeButton = InitializeButton(40, "switch-theme", switchThemeButton_TouchUpInside);
                informationButton = InitializeButton(contentView.Bounds.Width - 35, "information", InformationButton_TouchUpInside);
            }

            contentView.AddSubview(locateButton);
            contentView.AddSubview(switchThemeButton);
            contentView.AddSubview(informationButton);
        }

        /// <summary>
        /// Gets the current extend for MapView by frame and GPS location.
        /// </summary>
        /// <param name="currentLocationInMercator">The current location in mercator.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="resolution">The resolution.</param>
        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMercator, CGRect frame, double resolution)
        {
            double left = currentLocationInMercator.X - resolution * frame.Width * .5;
            double right = currentLocationInMercator.X + resolution * frame.Width * .5;
            double top = currentLocationInMercator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMercator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private static UIButton InitializeButton(nfloat x, string imageName, EventHandler touchEventHandler)
        {
            UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
            button.Frame = new CGRect(x, 0, 35, 35);
            button.TintColor = UIColor.White;
            button.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
            button.TouchUpInside += touchEventHandler;
            return button;
        }
    }
}