/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace DrawEditFeatures
{
    public partial class MainFormViewController : UIViewController
    {
        private MapView mapView;
        private UIButton editButton;
        private UIButton lineButton;
        private UIButton pointButton;
        private UIButton clearButton;
        private UIButton cursorButton;
        private UIButton circleButton;
        private UIButton polygonButton;
        private UIButton ellipseButton;
        private UIButton rectangleButton;
        private UIButton drawButton;
        private UIView drawButtonsView;
        private int buttonWidth = 50;

        private List<UIButton> uiButtons;

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            mapView = new MapView(View.Frame)
            {
                MapUnit = GeographyUnit.Meter,
                CurrentExtent = (new RectangleShape(-13358339, 11068716, -5565975, -11068716))
            };

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);

            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");
            mapView.Overlays.Add("WMK", thinkGeoCloudMapsOverlay);
            View.AddSubview(mapView);
            InitializeInstruction();
            uiButtons = new List<UIButton>() { lineButton, pointButton, editButton, clearButton, circleButton, cursorButton, polygonButton, ellipseButton, rectangleButton };

            mapView.Refresh();
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
            SampleUIHelper.InstructionContainer.Hidden = true;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            InitializeInstruction();
            SampleUIHelper.InstructionContainer.Hidden = false;
        }

        private void InitializeInstruction()
        {
            if (!SampleUIHelper.IsOnIPhone)
            {
                SampleUIHelper.InitializeInstruction(View, isIphone => 140, contentView =>
                {
                    cursorButton = GetUIButton(0, 0, "pan", TrackButtonClick);
                    pointButton = GetUIButton(1 * buttonWidth, 0, "point", TrackButtonClick);
                    lineButton = GetUIButton(2 * buttonWidth, 0, "polyline", TrackButtonClick);
                    rectangleButton = GetUIButton(3 * buttonWidth, 0, "rectangle", TrackButtonClick);
                    circleButton = GetUIButton(4 * buttonWidth, 0, "circle", TrackButtonClick);
                    polygonButton = GetUIButton(5 * buttonWidth, 0, "polygon", TrackButtonClick);
                    ellipseButton = GetUIButton(6 * buttonWidth, 0, "ellipse", TrackButtonClick);
                    editButton = GetUIButton(7 * buttonWidth, 0, "edit", TrackButtonClick);
                    clearButton = GetUIButton(8 * buttonWidth,  0, "recycle", TrackButtonClick);

                    contentView.AddSubviews(new UIView[] { cursorButton, pointButton, lineButton, rectangleButton, circleButton, polygonButton, ellipseButton, editButton, clearButton });
                });
            }
            else
            {
                SampleUIHelper.InitializeInstruction(View, isIphone => isIphone ? 140 : 120, contentView =>
                {
                    cursorButton = GetUIButton(0, 0, "pan", TrackButtonClick);
                    drawButton = GetUIButton(1 * buttonWidth,  0, "pen", TrackButtonClick);
                    editButton = GetUIButton(2 * buttonWidth, 0, "edit", TrackButtonClick);
                    clearButton = GetUIButton(3 * buttonWidth, 0, "recycle", TrackButtonClick);

                    pointButton = GetUIButton(0, 0, "point", TrackButtonClick);
                    lineButton = GetUIButton(1 * buttonWidth, 0, "polyline", TrackButtonClick);
                    rectangleButton = GetUIButton(2 * buttonWidth,  0, "rectangle", TrackButtonClick);
                    circleButton = GetUIButton(3 * buttonWidth, 0, "circle", TrackButtonClick);
                    polygonButton = GetUIButton(4 * buttonWidth, 0, "polygon", TrackButtonClick);
                    ellipseButton = GetUIButton(5 * buttonWidth, 0, "ellipse", TrackButtonClick);

                    drawButtonsView = new UIView(new CGRect(View.Frame.Right, 0, View.Frame.Width, 44));
                    drawButtonsView.Layer.BorderWidth = 0;
                    drawButtonsView.Layer.BorderColor = UIColor.Clear.CGColor;
                    drawButtonsView.BackgroundColor = UIColor.FromRGBA(126, 124, 129, 255);

                    drawButtonsView.Add(pointButton);
                    drawButtonsView.Add(lineButton);
                    drawButtonsView.Add(rectangleButton);
                    drawButtonsView.Add(circleButton);
                    drawButtonsView.Add(polygonButton);
                    drawButtonsView.Add(ellipseButton);

                    contentView.AddSubviews(cursorButton, drawButton, editButton, clearButton, drawButtonsView);
                });
            }
        }

        private static RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, CGRect frame, double resolution)
        {
            double left = currentLocationInMecator.X - resolution * frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }

        private static UIButton GetUIButton(int leftLocation, int topLocation, string imageName, EventHandler handler)
        {
            CGSize buttonSize = new CGSize(44, 44);
            UIButton button = UIButton.FromType(UIButtonType.System);
            button.Frame = new CGRect(new Point(leftLocation, topLocation), buttonSize);
            button.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
            button.SetTitle(imageName, UIControlState.Application);
            button.TouchUpInside += handler;
            button.TintColor = UIColor.White;
            return button;
        }


        private void TrackButtonClick(object sender, EventArgs e)
        {
            foreach (var tempButton in uiButtons)
            {
                tempButton.Layer.BorderWidth = 0;
                tempButton.Layer.BorderColor = UIColor.Clear.CGColor;
            }

            if (SampleUIHelper.IsOnIPhone)
            {
                drawButton.Layer.BorderWidth = 0;
                drawButton.Layer.BorderColor = UIColor.Clear.CGColor;
            }

            UIButton button = (UIButton)sender;
            button.Layer.BorderWidth = 1;
            button.Layer.BorderColor = UIColor.White.CGColor;
            switch (button.Title(UIControlState.Application))
            {
                case "pan":
                    if (drawButtonsView != null)
                    {
                        if (drawButtonsView.Frame.Left != View.Frame.Right && SampleUIHelper.IsOnIPhone)
                        {
                            UIView.Animate(.3, () =>
                            {
                                drawButtonsView.Frame = new CGRect(View.Frame.Right, 0, drawButtonsView.Frame.Width, 44);

                            });
                        }
                    }

                    foreach (var item in mapView.EditOverlay.EditShapesLayer.InternalFeatures)
                    {
                        if (!mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Contains(item))
                        {
                            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Add(item);
                        }
                    }

                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    mapView.EditOverlay.EditShapesLayer.InternalFeatures.Clear();
                    mapView.EditOverlay.ClearAllControlPoints();
                    mapView.EditOverlay.Refresh();
                    mapView.TrackOverlay.Refresh();
                    break;
                case "recycle":
                    mapView.EditOverlay.ClearAllControlPoints();
                    mapView.EditOverlay.EditShapesLayer.Open();
                    mapView.EditOverlay.EditShapesLayer.Clear();
                    mapView.TrackOverlay.TrackShapeLayer.Open();
                    mapView.TrackOverlay.TrackShapeLayer.Clear();
                    mapView.EditOverlay.Refresh();
                    mapView.TrackOverlay.Refresh();
                    break;
                case "point":
                    mapView.TrackOverlay.TrackMode = TrackMode.Point;
                    break;
                case "polyline":
                    mapView.TrackOverlay.TrackMode = TrackMode.Line;
                    break;
                case "rectangle":
                    mapView.TrackOverlay.TrackMode = TrackMode.Rectangle;
                    break;
                case "polygon":
                    mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
                    break;
                case "circle":
                    mapView.TrackOverlay.TrackMode = TrackMode.Circle;
                    break;
                case "ellipse":
                    mapView.TrackOverlay.TrackMode = TrackMode.Ellipse;
                    break;
                case "edit":
                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    foreach (Feature feature in mapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
                    {
                        mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
                    }
                    mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
                    mapView.EditOverlay.CalculateAllControlPoints();
                    mapView.EditOverlay.Refresh();
                    mapView.TrackOverlay.Refresh();
                    break;
                case "pen":
                    UIView.Animate(.3, () =>
                    {
                        drawButtonsView.Frame = new CGRect(drawButton.Frame.Left, 0, drawButtonsView.Frame.Width, 44);
                        ellipseButton.Hidden = true;
                    });
                    pointButton.Layer.BorderWidth = 1;
                    pointButton.Layer.BorderColor = UIColor.White.CGColor;
                    mapView.TrackOverlay.TrackMode = TrackMode.Point;
                    break;
                default:
                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }
    }
}