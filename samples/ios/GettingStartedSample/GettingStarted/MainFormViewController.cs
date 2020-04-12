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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GettingStartedSample
{
    public partial class MainFormViewController : UIViewController
    {
        private static readonly Dictionary<string, double> zoomLevelOptions;

        private MapView mapView;
        private InstructionView instructionView;
        private CLLocationManager locationManager;
        private ProjectionConverter bingToWgs84Projection;
        private long previousLocationUpdatedTicks;
        private TableViewSource zoomScaleSource;
        private TableViewSource mapTypeSource;

        static MainFormViewController()
        {
            zoomLevelOptions = new Dictionary<string, double>
            {
                ["Zoom to zoomlevel 2"] = 295295895,
                ["Zoom to zoomlevel 5"] = 36911986,
                ["Zoom to zoomlevel 10"] = 1153499,
                ["Zoom to zoomlevel 16"] = 18023,
                ["Zoom to zoomlevel 18"] = 4505
            };
        }

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            IOSCapabilityHelper.SetViewLayout(View);
            InitializeMap();
            InitializeTools();
            InitializeZoomToScaleDialog();
            InitializeGpsLocationManager();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            instructionView.DidRotate();
            instructionView.Hidden = false;
            mapView.MapTools["ScaleTool"].IsEnabled = true;
            mapView.MapTools.CenterCoordinate.IsEnabled = true;
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
            mapView.MapTools["ScaleTool"].IsEnabled = false;
            mapView.MapTools.CenterCoordinate.IsEnabled = false;
        }

        private void InitializeMap()
        {
            bingToWgs84Projection = new ProjectionConverter(Projection.GetBingMapProjString(), Projection.GetWgs84ProjString());
            bingToWgs84Projection.Open();

            mapView = new MapView(View.Frame)
            {
                MapUnit = GeographyUnit.Meter,
                ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512),
                BackgroundColor = UIColor.FromRGB(244, 242, 238),
                CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962)
            };
            View.AddSubview(mapView);

            // Touch events
            mapView.MapSingleTap += MapView_MapSingleTap;
            mapView.MapLongPress += MapView_MapLongPress;

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string clientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string secret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(clientKey, secret);
            mapView.Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);

            // Location marker layer.
            MarkerOverlay locationOverlay = new MarkerOverlay();
            mapView.Overlays.Add("LocationOverlay", locationOverlay);

            // Single tap popup overlay.
            PopupOverlay popupOverlay = new PopupOverlay();
            popupOverlay.OverlayView.UserInteractionEnabled = true;
            mapView.Overlays.Add("PopupOverlay", popupOverlay);

            mapView.MapTools.CenterCoordinate.IsEnabled = true;
            mapView.MapTools.CenterCoordinate.DisplayProjection = bingToWgs84Projection;
            mapView.MapTools.CenterCoordinate.DisplayTextFormat = "Longitude:{0:N4}, Latitude:{1:N4}";

            ScaleZoomLevelMapTool scaleTool = new ScaleZoomLevelMapTool
            {
                IsEnabled = true
            };
            mapView.MapTools.Add("ScaleTool", scaleTool);
            mapView.Refresh();
        }

        private void InitializeTools()
        {
            instructionView = new InstructionView(View, RefreshInstructionView, Instruction_ExpandStateChanged);
            Add(instructionView);
        }

        private void InitializeZoomToScaleDialog()
        {
            ModalDailogView.Layer.BorderColor = UIColor.Gray.CGColor;
            ModalDailogView.Layer.BorderWidth = 3;
            ModalDailogView.Layer.ShadowColor = UIColor.Gray.CGColor;
            ModalDailogView.Layer.ShadowRadius = 2;
            ModalDailogView.Layer.ShadowOpacity = .6f;
            ModalDailogView.Layer.ShadowOffset = new SizeF(1, 1);
            ModalDailogView.Hidden = true;
            View.BringSubviewToFront(ModalDailogView);

            //dialogBackground.Hidden = true;
            //View.BringSubviewToFront(dialogBackground);

            zoomScaleSource = new TableViewSource();
            SectionModel zoomScaleSection = new SectionModel();
            foreach (var zoomLevelOption in zoomLevelOptions)
            {
                zoomScaleSection.Rows.Add(new CellModel(zoomLevelOption.Key));
            }

            zoomScaleSource.Sections.Add(zoomScaleSection);
            zoomScaleSource.RowClick += ZoomScaleSourceRowClick;

            mapTypeSource = new TableViewSource();
            SectionModel mapTypeSection = new SectionModel();
            mapTypeSection.Rows.Add(new CellModel("Light"));
            mapTypeSection.Rows.Add(new CellModel("Dark"));

            mapTypeSource.Sections.Add(mapTypeSection);
            mapTypeSource.RowClick += MapTypeSourceRowClick;
        }

        private void MapTypeSourceRowClick(object sender, TableViewRowClickEventArgs e)
        {
            string baseMapTypeString = ((TableViewSource)e.TableView.Source).Sections[0].Rows[e.IndexPath.Row].Name;
            ThinkGeoCloudVectorMapsMapType wmkMapType = (ThinkGeoCloudVectorMapsMapType)Enum.Parse(typeof(ThinkGeoCloudVectorMapsMapType), baseMapTypeString);
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = (ThinkGeoCloudVectorMapsOverlay)mapView.Overlays["ThinkGeoCloudMapsOverlay"];
            if (wmkMapType != thinkGeoCloudMapsOverlay.MapType)
            {
                thinkGeoCloudMapsOverlay.MapType = wmkMapType;
                thinkGeoCloudMapsOverlay.Refresh();
            }

            SetModalDialogHidden(true);
        }

        private void InitializeGpsLocationManager()
        {
            locationManager = new CLLocationManager();
            IOSCapabilityHelper.SetGpsLocationManagerCapability(locationManager);
            locationManager.DesiredAccuracy = 1;
            locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
            locationManager.StartUpdatingLocation();
        }

        /// <summary>
        /// Handles the LocationsUpdated event of the LocationManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CLLocationsUpdatedEventArgs"/> instance containing the event data.</param>
        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            long currentTicks = DateTime.Now.Ticks;
            // Check the location update time, if is less that 5 seconds, don't need update the GPS marker.
            if (TimeSpan.FromTicks(currentTicks - previousLocationUpdatedTicks).TotalSeconds > 5 && !mapView.ExtentOverlay.IsBusy)
            {
                previousLocationUpdatedTicks = currentTicks;
            }
            else
            {
                return;
            }

            CLLocation gpsLocation = e.Locations.FirstOrDefault();
            if (gpsLocation == null) return;

            PointShape currentLocationInWgs84 = new PointShape(gpsLocation.Coordinate.Longitude, gpsLocation.Coordinate.Latitude);
            PointShape currentLocationInMercator = (PointShape)bingToWgs84Projection.ConvertToInternalProjection(currentLocationInWgs84);

            MarkerOverlay locationOverlay = (MarkerOverlay)mapView.Overlays["LocationOverlay"];
            GpsMarker locationMarker = locationOverlay.Markers.OfType<GpsMarker>().FirstOrDefault();

            // Create a location marker if not exists.
            if (locationMarker == null)
            {
                locationMarker = new GpsMarker();
                locationOverlay.Markers.Add(locationMarker);
            }

            // Update the GPS marker position.
            locationMarker.HaloRadiusInMeter = (float)gpsLocation.HorizontalAccuracy;
            locationMarker.Position = currentLocationInMercator;
            locationOverlay.Refresh();
        }

        private void ZoomScaleSourceRowClick(object sender, TableViewRowClickEventArgs e)
        {
            CellModel row = ((TableViewSource)e.TableView.Source).Sections[e.IndexPath.Section].Rows[e.IndexPath.Row];
            double scale = double.Parse(zoomLevelOptions[row.Name].ToString(CultureInfo.InvariantCulture));
            mapView.ZoomToScale(scale);

            SetModalDialogHidden(true);
        }

        private void Instruction_ExpandStateChanged(CGRect instructionFrame, bool animate)
        {
            if (animate)
            {
                UIView.BeginAnimations("MapToolsPositionChanged");
                UIView.SetAnimationDuration(0.2);
            }
            MapTool scaleTool = mapView.MapTools["ScaleTool"];
            MapTool coordinateTool = mapView.MapTools.CenterCoordinate;

            coordinateTool.Center = new CGPoint(View.Frame.Width - scaleTool.Frame.Width / 2 - 5, instructionFrame.Top - 25);
            scaleTool.Center = new CGPoint(View.Frame.Width - coordinateTool.Frame.Width / 2 - 5, instructionFrame.Top - 40);

            CALayer attributionLayer = mapView.EventView.Layer.Sublayers.FirstOrDefault(l => l.Name.Equals("AttributionLayer"));
            attributionLayer.Frame = new CGRect(0, -instructionFrame.Height, attributionLayer.Frame.Width, attributionLayer.Frame.Height);

            if (animate) UIView.CommitAnimations();
        }

        //partial void dialogBackground_TouchUpInside(UIButton sender)
        //{
        //    SetModalDialogHidden(true);
        //}

        partial void ModalDailogCloseButton_TouchUpInside(UIButton sender)
        {
            SetModalDialogHidden(true);
        }

        private void MapView_MapSingleTap(object sender, UIGestureRecognizer e)
        {
            PointF location = (PointF)e.LocationInView(mapView);
            PointShape worldPoint = MapUtil.ToWorldCoordinate(mapView.CurrentExtent, location.X, location.Y, (float)mapView.Frame.Width, (float)mapView.Frame.Height);

            PointShape wgs84Point = (PointShape)bingToWgs84Projection.ConvertToExternalProjection(worldPoint);
            string displayText = string.Format("Longitude : {0:N4}\r\nLatitude : {1:N4}", wgs84Point.X, wgs84Point.Y);
            PopupOverlay popupOverlay = (PopupOverlay)mapView.Overlays["PopupOverlay"];
            CurrentLocationPopup popup = (CurrentLocationPopup)popupOverlay.Popups.FirstOrDefault() ??
                new CurrentLocationPopup(worldPoint, displayText);

            popup.Position = worldPoint;
            popup.Hidden = false;
            popup.Content = displayText;
            popup.UserInteractionEnabled = true;

            popupOverlay.Popups.Clear();
            popupOverlay.Popups.Add(popup);
            popupOverlay.Refresh();
        }

        private void MapView_MapLongPress(object sender, UIGestureRecognizer e)
        {
            ModalOptionsTableView.Source = zoomScaleSource;
            ModalOptionsTableView.ReloadData();
            SetModalDialogHidden(false);
        }
        private void MapTypeButton_TouchUpInside(object sender, EventArgs e)
        {
            ModalOptionsTableView.Source = mapTypeSource;
            ModalOptionsTableView.ReloadData();
            SetModalDialogHidden(false);
        }

        private void LocateButton_TouchUpInside(object sender, EventArgs e)
        {
            MarkerOverlay locaterOverlay = (MarkerOverlay)mapView.Overlays["LocationOverlay"];
            Marker locateMarker = locaterOverlay.Markers.FirstOrDefault();
            if (locateMarker != null)
            {
                mapView.ZoomTo(locateMarker.Position, 18023);
            }
        }

        private void FullExtentButton_TouchUpInside(object sender, EventArgs e)
        {
            Overlay worldOverlay = mapView.Overlays["ThinkGeoCloudMapsOverlay"];
            mapView.CurrentExtent = worldOverlay.GetBoundingBox();
            mapView.Refresh();
        }

        private void PreviousExtentButton_TouchUpInside(object sender, EventArgs e)
        {
            mapView.ZoomToPreviousExtent();
        }

        private void NextExtentButton_TouchUpInside(object sender, EventArgs e)
        {
            mapView.ZoomToNextExtent();
        }

        private void InformationButton_TouchUpInside(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl("https://thinkgeo.com/gis-ui-mobile"));
        }

        private void RefreshInstructionView(UIView contentView)
        {
            UIButton locateButton;
            UIButton fullExtentButton;
            UIButton previousExtentButton;
            UIButton nextExtentButton;
            UIButton mapTypeButton;
            UIButton informationButton;

            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            // Check the device orientation, refresh the controls layout for instruction view.
            if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
            {
                locateButton = InitializeButton(contentView.Bounds.Width - 235, "location", LocateButton_TouchUpInside);
                fullExtentButton = InitializeButton(contentView.Bounds.Width - 195, "globe", FullExtentButton_TouchUpInside);
                previousExtentButton = InitializeButton(contentView.Bounds.Width - 155, "previous", PreviousExtentButton_TouchUpInside);
                nextExtentButton = InitializeButton(contentView.Bounds.Width - 115, "next", NextExtentButton_TouchUpInside);
                mapTypeButton = InitializeButton(contentView.Bounds.Width - 75, "map-type", MapTypeButton_TouchUpInside);
                informationButton = InitializeButton(contentView.Bounds.Width - 35, "information", InformationButton_TouchUpInside);
            }
            else
            {
                locateButton = InitializeButton(0, "location", LocateButton_TouchUpInside);
                fullExtentButton = InitializeButton(40, "globe", FullExtentButton_TouchUpInside);
                previousExtentButton = InitializeButton(80, "previous", PreviousExtentButton_TouchUpInside);
                nextExtentButton = InitializeButton(120, "next", NextExtentButton_TouchUpInside);
                mapTypeButton = InitializeButton(160, "map-type", MapTypeButton_TouchUpInside);
                informationButton = InitializeButton(contentView.Bounds.Width - 35, "information", InformationButton_TouchUpInside);
            }

            contentView.AddSubview(locateButton);
            contentView.AddSubview(fullExtentButton);
            contentView.AddSubview(previousExtentButton);
            contentView.AddSubview(nextExtentButton);
            contentView.AddSubview(mapTypeButton);
            contentView.AddSubview(informationButton);
        }

        private void SetModalDialogHidden(bool hidden)
        {
            UIView.Animate(0.2, () =>
            {
                //dialogBackground.Hidden = hidden;
                ModalDailogView.Hidden = hidden;
            });
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