using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace RoutingSample_iOS
{
    public partial class ViewController : UIViewController
    {
        private MapView mapView;
        private LayerOverlay layerOverlay;
        private RoutingLayer routingLayer;
        private RoutingEngine routingEngine;
        private bool firstClick = true;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mapView = new MapView(View.Frame);
            View.Add(mapView);
            ComposeTableControl();

            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = ThinkGeoCloudMapsOverlay.GetZoomLevelSet();
            mapView.MapSingleTap += MapViewMapSingleTap;

            ThinkGeoCloudMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudMapsOverlay();
            mapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            layerOverlay = new LayerOverlay();
            routingLayer = new RoutingLayer();
            layerOverlay.Layers.Add(routingLayer);
            mapView.Overlays.Add(layerOverlay);

            string shapeFilePath = Path.Combine("AppData", "DallasCounty-3857.shp");
            string rtgPath = Path.Combine("AppData", "DallasCounty-3857.rtg");
            var routingSource = new RtgRoutingSource(rtgPath);
            var featureSource = new ShapeFileFeatureSource(shapeFilePath);

            routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            routingEngine.SearchRadiusInMeters = 200;

            mapView.CurrentExtent = new RectangleShape(-10781100.2970769, 3875007.18710502, -10767407.8727504, 3854947.78546675);
            mapView.Refresh();
        }

        private void ComposeTableControl()
        {
            var tableHeaderView = new UIView(new CGRect(View.Frame.X, View.Frame.Height - 110, View.Frame.Width, 30));
            tableHeaderView.BackgroundColor = UIColor.FromRGB(0, 0, 0);

            var cellWidth = tableHeaderView.Frame.Width / 3;

            var roadNameLabel = new UILabel() { Text = "RoadName", TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(10), Frame = new CGRect(View.Frame.X + 5, 8, cellWidth, 20), TextAlignment = UITextAlignment.Center };
            roadNameLabel.SizeToFit();
            var directionLabel = new UILabel() { Text = "Direction", TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(10), Frame = new CGRect(View.Frame.X + 5 + cellWidth, 8, cellWidth, 20), TextAlignment = UITextAlignment.Center };
            directionLabel.SizeToFit();
            var lengthLabel = new UILabel() { Text = "Length(Meter)", TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(10), Frame = new CGRect(View.Frame.X + 5 + 2 * cellWidth, 8, cellWidth, 20), TextAlignment = UITextAlignment.Center };
            lengthLabel.SizeToFit();

            tableHeaderView.AddSubview(roadNameLabel);
            tableHeaderView.AddSubview(directionLabel);
            tableHeaderView.AddSubview(lengthLabel);

            var tableView = new UITableView(new CGRect(View.Frame.X, View.Frame.Height - 80, View.Frame.Width, View.Frame.Height));

            tableView.BackgroundColor = UIColor.FromRGB(233, 233, 233);
            View.Add(tableHeaderView);

            View.Add(tableView);
        }

        private void MapViewMapSingleTap(object sender, UIGestureRecognizer e)
        {
            CGPoint location = e.LocationInView(View);
            PointShape position = ExtentHelper.ToWorldCoordinate(mapView.CurrentExtent, (float)location.X, (float)location.Y, (float)View.Frame.Width, (float)View.Frame.Height);

            routingLayer.Routes.Clear();
            if (firstClick)
            {
                routingLayer.StartPoint = position;
                firstClick = false;
            }
            else
            {
                routingLayer.EndPoint = position;
                firstClick = true;
            }

            routingLayer.Routes.Clear();
            if (routingLayer.StartPoint != null && routingLayer.EndPoint != null)
            {
                RoutingResult routingResult = routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.EndPoint);
                routingLayer.Routes.Add(routingResult.Route);
                ShowTurnByTurnDirections(routingResult.RouteSegments, routingResult.Features);
            }

            layerOverlay.Refresh();
        }

        private void ShowTurnByTurnDirections(Collection<RouteSegment> roads, Collection<Feature> features)
        {
            var tableView = View.Subviews[4] as UITableView;

            var directions = new List<DirectionDataItem>();
            for (int i = 0; i < roads.Count; i++)
            {
                var direction = new DirectionDataItem()
                {
                    RoadName = features[i].ColumnValues["NAME"],
                    Direction = roads[i].DrivingDirection.ToString(),
                    Length = Math.Round(((LineBaseShape)features[i].GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Meter), 2)
                };

                directions.Add(direction);
            }
            tableView.DataSource = new DirectionTableViewDataSource(directions);
            tableView.ReloadData();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}