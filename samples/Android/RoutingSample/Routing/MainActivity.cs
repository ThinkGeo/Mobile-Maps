/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;

namespace RoutingSample
{
    [Activity(Label = "RoutingSample", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MapView mapView;
        private LayerOverlay layerOverlay;
        private RoutingLayer routingLayer;
        private RoutingEngine routingEngine;
        private bool firstClick = true;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            try
            {
                FrameLayout mapContainerView = FindViewById<FrameLayout>(Resource.Id.MapContainerView);
                mapContainerView.RemoveAllViews();
                mapView = new MapView(Application.Context);
                mapView.SetBackgroundColor(Color.Argb(255, 244, 242, 238));

                InitalizeMap();

                mapContainerView.AddView(mapView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
            }
            catch (Exception ex)
            {
                Log.Debug("Sample Changed", ex.Message);
            }
        }

        private void InitalizeMap()
        {
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            mapView.SingleTap += MapView_SingleTap;

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay baseOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            mapView.Overlays.Add(baseOverlay);

            layerOverlay = new LayerOverlay();
            routingLayer = new RoutingLayer();
            layerOverlay.Layers.Add(routingLayer);
            mapView.Overlays.Add(layerOverlay);

            var routingSource = new RtgRoutingSource(DataManager.GetDataPath("DallasCounty-3857.rtg"));
            var featureSource = new ShapeFileFeatureSource(DataManager.GetDataPath("DallasCounty-3857.shp"));

            routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            routingEngine.SearchRadiusInMeters = 100;

            mapView.CurrentExtent = new RectangleShape(-10781100.2970769, 3875007.18710502, -10767407.8727504, 3854947.78546675);
        }

        private void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            routingLayer.Routes.Clear();
            if (firstClick)
            {
                routingLayer.StartPoint = e.WorldPoint;
                firstClick = false;
            }
            else
            {
                routingLayer.EndPoint = e.WorldPoint;
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
            var directionListView = FindViewById<ListView>(Resource.Id.DirectionListView);

            var directionData = new List<DirectionDataItem>();

            for (int i = 0; i < roads.Count; i++)
            {
                var direction = new DirectionDataItem()
                {
                    RoadName = features[i].ColumnValues["NAME"],
                    Direction = roads[i].DrivingDirection.ToString(),
                    Length = Math.Round(((LineBaseShape)features[i].GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Meter), 2)
                };
                directionData.Add(direction);
            }

            directionListView.Adapter = new DirectionItemAdapter(this, directionData);
        }
    }
}

