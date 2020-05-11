/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace DrawEditFeatures
{
    [Activity(Label = "Draw And Edit", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int contentHeight;
        private MapView androidMap;
        private ImageButton editButton;
        private ImageButton lineButton;
        private ImageButton pointButton;
        private ImageButton clearButton;
        private ImageButton cursorButton;
        private ImageButton circleButton;
        private ImageButton polygonButton;
        private ImageButton ellipseButton;
        private ImageButton rectangleButton;
        private ImageButton drawButton;
        private LinearLayout trackLinearLayout;
        private Vertex endVertex;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            androidMap = FindViewById<MapView>(Resource.Id.androidmap);
            androidMap.TrackOverlay.VertexAdded += (sender, e) => { endVertex = e.AddedVertex; };
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.CurrentExtent = new RectangleShape(-13358339, 11068716, -5565975, -11068716);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay baseOverlay = new ThinkGeoCloudRasterMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            androidMap.Overlays.Add(baseOverlay);

            cursorButton = GetButton(Resource.Drawable.Cursor, TrackButtonClick);
            drawButton = GetButton(Resource.Drawable.Draw, TrackButtonClick);
            pointButton = GetButton(Resource.Drawable.Point, TrackButtonClick);
            lineButton = GetButton(Resource.Drawable.Line, TrackButtonClick);
            rectangleButton = GetButton(Resource.Drawable.Rectangle, TrackButtonClick);
            circleButton = GetButton(Resource.Drawable.Circle, TrackButtonClick);
            polygonButton = GetButton(Resource.Drawable.Polygon, TrackButtonClick);
            ellipseButton = GetButton(Resource.Drawable.Ellipse, TrackButtonClick);
            editButton = GetButton(Resource.Drawable.Edit, TrackButtonClick);
            clearButton = GetButton(Resource.Drawable.Clear, TrackButtonClick);

            trackLinearLayout = new LinearLayout(this);
            trackLinearLayout.Orientation = Orientation.Horizontal;
            trackLinearLayout.Visibility = ViewStates.Gone;
            trackLinearLayout.AddView(pointButton);
            trackLinearLayout.AddView(lineButton);
            trackLinearLayout.AddView(rectangleButton);
            trackLinearLayout.AddView(polygonButton);
            trackLinearLayout.AddView(ellipseButton);

            LinearLayout toolsLinearLayout = new LinearLayout(this);
            toolsLinearLayout.AddView(cursorButton);
            toolsLinearLayout.AddView(drawButton);
            toolsLinearLayout.AddView(trackLinearLayout);
            toolsLinearLayout.AddView(editButton);
            toolsLinearLayout.AddView(clearButton);

            InitializeInstruction(toolsLinearLayout);
        }

        private ImageButton GetButton(int imageResId, EventHandler handler)
        {
            ImageButton button = new ImageButton(this);
            button.Id = imageResId;
            button.SetImageResource(imageResId);
            button.Click += handler;
            button.SetBackgroundResource(Resource.Drawable.buttonbackground);
            return button;
        }

        private IEnumerable<ImageButton> GetButtons()
        {
            yield return editButton;
            yield return lineButton;
            yield return pointButton;
            yield return clearButton;
            yield return cursorButton;
            yield return circleButton;
            yield return polygonButton;
            yield return ellipseButton;
            yield return rectangleButton;
            yield return drawButton;
        }

        private void TrackButtonClick(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;
            foreach (ImageButton tempButton in GetButtons())
            {
                tempButton.SetBackgroundResource(Resource.Drawable.buttonbackground);
            }
            button.SetBackgroundResource(Resource.Drawable.buttonselectedbackground);
           
            androidMap.TrackOverlay.LongPress(new InteractionArguments() { WorldX = endVertex.X, WorldY = endVertex.Y});
     
            switch (button.Id)
            {
                case Resource.Drawable.Cursor:
                    ClearAndSaveEditMode();
                    androidMap.TrackOverlay.TrackMode = TrackMode.None;
                    trackLinearLayout.Visibility = ViewStates.Gone;
                    drawButton.Visibility = ViewStates.Visible;
                    editButton.Visibility = ViewStates.Visible;
                    clearButton.Visibility = ViewStates.Visible;
                    androidMap.Refresh();
                    break;

                case Resource.Drawable.Clear:
                    ClearAndSaveEditMode();
                    androidMap.TrackOverlay.TrackShapeLayer.Open();
                    androidMap.TrackOverlay.TrackShapeLayer.Clear();
                    androidMap.TrackOverlay.TrackMode = TrackMode.None;
                    androidMap.Refresh();
                    break;

                case Resource.Drawable.Point:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Point;
                    break;

                case Resource.Drawable.Line:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Line;
                    break;

                case Resource.Drawable.Rectangle:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Rectangle;
                    break;

                case Resource.Drawable.Polygon:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Polygon;
                    break;

                case Resource.Drawable.Circle:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Circle;
                    break;

                case Resource.Drawable.Ellipse:
                    androidMap.TrackOverlay.TrackMode = TrackMode.Ellipse;
                    break;

                case Resource.Drawable.Edit:
                    androidMap.TrackOverlay.TrackMode = TrackMode.None;
                    foreach (Feature feature in androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures)
                    {
                        androidMap.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
                    }
                    androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
                    androidMap.EditOverlay.CalculateAllControlPoints();
                    androidMap.Refresh();
                    break;

                case Resource.Drawable.Draw:
                    ClearAndSaveEditMode();
                    androidMap.TrackOverlay.TrackMode = TrackMode.Point;
                    trackLinearLayout.Visibility = ViewStates.Visible;
                    drawButton.Visibility = ViewStates.Gone;
                    editButton.Visibility = ViewStates.Gone;
                    clearButton.Visibility = ViewStates.Gone;
                    pointButton.SetBackgroundResource(Resource.Drawable.buttonselectedbackground);
                    androidMap.Refresh();
                    break;

                default:
                    androidMap.TrackOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }

        public void ClearAndSaveEditMode()
        {
            foreach (var item in androidMap.EditOverlay.EditShapesLayer.InternalFeatures)
            {
                if (!androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Contains(item))
                {
                    androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Add(item);
                }
            }
            androidMap.EditOverlay.ClearAllControlPoints();
            androidMap.EditOverlay.EditShapesLayer.Open();
            androidMap.EditOverlay.EditShapesLayer.Clear();
        }

        public void InitializeInstruction(params View[] contentViews)
        {
            contentHeight = 0;
            ViewGroup containerView = FindViewById<RelativeLayout>(Resource.Id.MainLayout);

            LayoutInflater inflater = LayoutInflater.From(this);
            View instructionLayoutView = inflater.Inflate(Resource.Layout.Instruction, containerView);

            TextView instructionTextView = instructionLayoutView.FindViewById<TextView>(Resource.Id.instructionTextView);
            TextView descriptionTextView = instructionLayoutView.FindViewById<TextView>(Resource.Id.descriptionTextView);
            descriptionTextView.Text = "Draw and Edit Shapes.";

            LinearLayout instructionLayout = instructionLayoutView.FindViewById<LinearLayout>(Resource.Id.instructionLinearLayout);
            LinearLayout contentLayout = instructionLayoutView.FindViewById<LinearLayout>(Resource.Id.contentLinearLayout);

            RelativeLayout headerRelativeLayout = instructionLayoutView.FindViewById<RelativeLayout>(Resource.Id.headerRelativeLayout);

            if (contentViews != null)
            {
                foreach (View view in contentViews)
                {
                    contentLayout.AddView(view);
                }
            }

            headerRelativeLayout.Click += (sender, e) =>
            {
                contentHeight = contentHeight == 0 ? instructionLayout.Height - instructionTextView.Height : -contentHeight;
                instructionLayout.Layout(instructionLayout.Left, instructionLayout.Top + contentHeight, instructionLayout.Right, instructionLayout.Bottom);
            };
        }
    }
}

