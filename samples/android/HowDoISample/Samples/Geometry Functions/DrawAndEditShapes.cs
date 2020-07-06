using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class DrawAndEditShapes : SampleFragment
    {
        private MapView mapView;
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
        private LinearLayout drawLinearLayout;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ThinkGeoCloudRasterMapsOverlay layerOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);

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

            drawLinearLayout = new LinearLayout(this.Context);
            drawLinearLayout.Orientation = Orientation.Horizontal;
            drawLinearLayout.Visibility = ViewStates.Gone;
            drawLinearLayout.AddView(pointButton);
            drawLinearLayout.AddView(lineButton);
            drawLinearLayout.AddView(rectangleButton);
            drawLinearLayout.AddView(polygonButton);
            drawLinearLayout.AddView(ellipseButton);

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.AddView(cursorButton);
            linearLayout.AddView(drawButton);
            linearLayout.AddView(drawLinearLayout);
            linearLayout.AddView(editButton);
            linearLayout.AddView(clearButton);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private ImageButton GetButton(int imageResId, EventHandler handler)
        {
            ImageButton button = new ImageButton(this.Context);
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

            switch (button.Id)
            {
                case Resource.Drawable.Cursor:
                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    mapView.EditOverlay.ClearAllControlPoints();
                    drawLinearLayout.Visibility = ViewStates.Gone;
                    drawButton.Visibility = ViewStates.Visible;
                    editButton.Visibility = ViewStates.Visible;
                    clearButton.Visibility = ViewStates.Visible;
                    mapView.Refresh();
                    break;

                case Resource.Drawable.Clear:
                    mapView.EditOverlay.ClearAllControlPoints();
                    mapView.EditOverlay.EditShapesLayer.Open();
                    mapView.EditOverlay.EditShapesLayer.Clear();
                    mapView.TrackOverlay.TrackShapeLayer.Open();
                    mapView.TrackOverlay.TrackShapeLayer.Clear();
                    mapView.Refresh();
                    break;

                case Resource.Drawable.Point:
                    mapView.TrackOverlay.TrackMode = TrackMode.Point;
                    break;

                case Resource.Drawable.Line:
                    mapView.TrackOverlay.TrackMode = TrackMode.Line;
                    break;

                case Resource.Drawable.Rectangle:
                    mapView.TrackOverlay.TrackMode = TrackMode.Rectangle;
                    break;

                case Resource.Drawable.Polygon:
                    mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
                    break;

                case Resource.Drawable.Circle:
                    mapView.TrackOverlay.TrackMode = TrackMode.Circle;
                    break;

                case Resource.Drawable.Ellipse:
                    mapView.TrackOverlay.TrackMode = TrackMode.Ellipse;
                    break;

                case Resource.Drawable.Edit:
                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    foreach (Feature feature in mapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
                    {
                        mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
                    }
                    mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
                    mapView.EditOverlay.CalculateAllControlPoints();
                    mapView.Refresh();
                    break;

                case Resource.Drawable.Draw:
                    mapView.TrackOverlay.TrackMode = TrackMode.Point;
                    drawLinearLayout.Visibility = ViewStates.Visible;
                    drawButton.Visibility = ViewStates.Gone;
                    editButton.Visibility = ViewStates.Gone;
                    clearButton.Visibility = ViewStates.Gone;
                    pointButton.SetBackgroundResource(Resource.Drawable.buttonselectedbackground);
                    break;

                default:
                    mapView.TrackOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }
    }
}