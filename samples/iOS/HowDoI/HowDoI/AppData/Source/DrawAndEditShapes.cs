using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DrawAndEditShapes : BaseViewController
    {
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

        public DrawAndEditShapes()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-180.0, 83.0, 180.0, -90.0);

            ThinkGeoCloudRasterMapsOverlay layerOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret) { TileCache = null };
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            if (!SampleUIHelper.IsOnIPhone)
            {
                SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 140, (contentView) =>
                {
                    cursorButton = GetUIButton(GetButtonLeft(0), "Cursor", TrackButtonClick);
                    pointButton = GetUIButton(GetButtonLeft(1), "Point", TrackButtonClick);
                    lineButton = GetUIButton(GetButtonLeft(2), "Line", TrackButtonClick);
                    rectangleButton = GetUIButton(GetButtonLeft(3), "Rectangle", TrackButtonClick);
                    circleButton = GetUIButton(GetButtonLeft(4), "Circle", TrackButtonClick);
                    polygonButton = GetUIButton(GetButtonLeft(5), "Polygon", TrackButtonClick);
                    ellipseButton = GetUIButton(GetButtonLeft(6), "Ellipse", TrackButtonClick);
                    editButton = GetUIButton(GetButtonLeft(7), "Edit", TrackButtonClick);
                    clearButton = GetUIButton(GetButtonLeft(8), "Clear", TrackButtonClick);

                    contentView.AddSubviews(new UIView[] { cursorButton, pointButton, lineButton, rectangleButton, circleButton, polygonButton, ellipseButton, editButton, clearButton });
                });
            }
            else
            {
                SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 140 : 120, (contentView) =>
                {
                    cursorButton = GetUIButton(GetButtonLeft(0), "Cursor", TrackButtonClick);
                    drawButton = GetUIButton(GetButtonLeft(1), "Draw", TrackButtonClick);
                    editButton = GetUIButton(GetButtonLeft(2), "Edit", TrackButtonClick);
                    clearButton = GetUIButton(GetButtonLeft(3), "Clear", TrackButtonClick);

                    pointButton = GetUIButton(0, 0, "Point", TrackButtonClick);
                    lineButton = GetUIButton(GetButtonLeft(1), 0, "Line", TrackButtonClick);
                    rectangleButton = GetUIButton(GetButtonLeft(2), 0, "Rectangle", TrackButtonClick);
                    circleButton = GetUIButton(GetButtonLeft(3), 0, "Circle", TrackButtonClick);
                    polygonButton = GetUIButton(GetButtonLeft(4), 0, "Polygon", TrackButtonClick);
                    ellipseButton = GetUIButton(GetButtonLeft(5), 0, "Ellipse", TrackButtonClick);

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

                    contentView.AddSubviews(new UIView[] { cursorButton, drawButton, editButton, clearButton, drawButtonsView });
                });
            }
        }

        private static int GetButtonLeft(int i)
        {
            int size = 50;
            int left = size * i;
            return left;
        }

        private IEnumerable<UIButton> GetButtons()
        {
            yield return lineButton;
            yield return pointButton;
            yield return editButton;
            yield return clearButton;
            yield return circleButton;
            yield return cursorButton;
            yield return polygonButton;
            yield return ellipseButton;
            yield return rectangleButton;
        }

        private static UIButton GetUIButton(int leftLocation, string imageName, EventHandler handler)
        {
            CGSize buttonSize = new CGSize(44, 44);
            UIButton button = UIButton.FromType(UIButtonType.System);
            button.Frame = new CGRect(new Point(leftLocation, 0), buttonSize);
            button.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
            button.SetTitle(imageName, UIControlState.Application);
            button.TouchUpInside += handler;
            button.TintColor = UIColor.White;
            return button;
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
            foreach (var tempButton in GetButtons())
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
                case "Cursor":
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
                    MapView.TrackOverlay.TrackMode = TrackMode.None;
                    MapView.EditOverlay.ClearAllControlPoints();
                    MapView.Refresh();
                    break;

                case "Clear":
                    MapView.EditOverlay.ClearAllControlPoints();
                    MapView.EditOverlay.EditShapesLayer.Open();
                    MapView.EditOverlay.EditShapesLayer.Clear();
                    MapView.TrackOverlay.TrackShapeLayer.Open();
                    MapView.TrackOverlay.TrackShapeLayer.Clear();
                    MapView.Refresh();
                    break;

                case "Point":
                    MapView.TrackOverlay.TrackMode = TrackMode.Point;
                    break;

                case "Line":
                    MapView.TrackOverlay.TrackMode = TrackMode.Line;
                    break;

                case "Rectangle":
                    MapView.TrackOverlay.TrackMode = TrackMode.Rectangle;
                    break;

                case "Polygon":
                    MapView.TrackOverlay.TrackMode = TrackMode.Polygon;
                    break;

                case "Circle":
                    MapView.TrackOverlay.TrackMode = TrackMode.Circle;
                    break;

                case "Ellipse":
                    MapView.TrackOverlay.TrackMode = TrackMode.Ellipse;
                    break;

                case "Edit":
                    MapView.TrackOverlay.TrackMode = TrackMode.None;
                    foreach (Feature feature in MapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
                    {
                        MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
                    }
                    MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
                    MapView.EditOverlay.CalculateAllControlPoints();
                    MapView.Refresh();
                    break;

                case "Draw":
                    UIView.Animate(.3, () =>
                    {
                        drawButtonsView.Frame = new CGRect(drawButton.Frame.Left, 0, drawButtonsView.Frame.Width, 44);
                        ellipseButton.Hidden = true;
                    });
                    pointButton.Layer.BorderWidth = 1;
                    pointButton.Layer.BorderColor = UIColor.White.CGColor;
                    MapView.TrackOverlay.TrackMode = TrackMode.Point;
                    break;

                default:
                    MapView.TrackOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }
    }
}