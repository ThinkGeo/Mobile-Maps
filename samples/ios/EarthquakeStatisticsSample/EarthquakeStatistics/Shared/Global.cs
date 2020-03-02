using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public static class Global
    {
        private static UIStoryboard storyboard;
        private static Collection<Feature> queriedFeatures;
        private static Dictionary<string, UIViewController> controllers;

        public static readonly string ThinkGeoCloudMapsOverlayKey = "ThinkGeoCloudMapsOverlay";
        public static readonly string HighLightOverlayKey = "HighlightOverlay";

        private static UIStoryboard Storyboard
        {
            get
            {
                string storyboardName = "MainForm";
                return storyboard ?? (storyboard = UIStoryboard.FromName(storyboardName, null));
            }
        }

        public static MapView MapView { get; set; }

        public static string BingMapKey { get; set; }

        public static QueryConfiguration QueryConfiguration { get; set; }

        public static BaseMapType BaseMapType { get; set; }

        public static string BaseMapTypeString { get; set; }

        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static Collection<Feature> QueriedFeatures
        {
            get { return queriedFeatures ?? (queriedFeatures = new Collection<Feature>()); }
        }

        public static ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudMapsOverlay
        {
            get { return (ThinkGeoCloudRasterMapsOverlay)MapView.Overlays[ThinkGeoCloudMapsOverlayKey]; }
        }

        public static LayerOverlay HighLightOverlay
        {
            get { return (LayerOverlay)MapView.Overlays[HighLightOverlayKey]; }
        }

        public static Proj4Projection GetWgs84ToMercatorProjection()
        {
            Proj4Projection wgs84ToMercatorProjection = new Proj4Projection
            {
                InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString(),
                ExternalProjectionParametersString = Proj4Projection.GetBingMapParametersString()
            };
            return wgs84ToMercatorProjection;
        }

        static Global()
        {
            QueryConfiguration = new QueryConfiguration();
            controllers = new Dictionary<string, UIViewController>();
        }

        public static void FilterSelectedEarthquakeFeatures()
        {
            InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)HighLightOverlay.Layers["SelectMarkerLayer"];

            selectMarkerLayer.InternalFeatures.Clear();

            foreach (var feature in QueriedFeatures)
            {
                double.TryParse(feature.ColumnValues["MAGNITUDE"], out double magnitude);
                double.TryParse(feature.ColumnValues["DEPTH_KM"], out double depth);
                double.TryParse(feature.ColumnValues["YEAR"], out double year);

                if ((magnitude >= QueryConfiguration.LowerMagnitude && magnitude <= QueryConfiguration.UpperMagnitude || magnitude == -9999)
                       && (depth <= QueryConfiguration.UpperDepth && depth >= QueryConfiguration.LowerDepth || depth == -9999)
                       && (year >= QueryConfiguration.LowerYear && year <= QueryConfiguration.UpperYear) || year == -9999)
                {
                    selectMarkerLayer.InternalFeatures.Add(feature);
                }
            }
        }

        public static UIViewController FindViewController(string viewControllerName)
        {
            if (!controllers.ContainsKey(viewControllerName))
            {
                UIViewController controller = Storyboard.InstantiateViewController(viewControllerName);
                controllers.Add(viewControllerName, controller);
            }

            return controllers[viewControllerName];
        }

        public static void AnimatedShow(this UIView view)
        {
            //if (Math.Abs(view.Transform.y0) < 0.001f)
            //{
            //    nfloat y = -view.Frame.Height;
            //    UIView.Animate(0.3, () =>
            //    {
            //        view.Transform = CGAffineTransform.MakeTranslation(0, y);
            //        view.Hidden = false;
            //    });
            //}

            UIView.Animate(0.3, () =>
            {
                view.Transform = CGAffineTransform.MakeTranslation(0, 0);
                view.Hidden = false;
            }, () =>
            {
                view.Hidden = false;
            });
        }

        public static void AnimatedHide(this UIView view)
        {
            //if (Math.Abs(view.Transform.y0) > 0.001f)
            //{
            //    UIView.Animate(0.3, () =>
            //    {
            //        view.Transform = CGAffineTransform.MakeTranslation(0, 0);
            //    }, () =>
            //    {
            //        view.Hidden = true;
            //    });
            //}

            UIView.Animate(0.3, () =>
            {
                view.Transform = CGAffineTransform.MakeTranslation(0, view.Frame.Height);
                view.Hidden = true;
            });
        }
    }
}