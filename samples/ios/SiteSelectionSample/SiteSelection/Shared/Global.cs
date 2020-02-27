using System.Collections.Generic;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Shapes;
using UIKit;

namespace MapSuiteSiteSelection
{
    public static class Global
    {
        public static readonly string HighlightAreaLayerKey = "HighlightAreaLayer";
        public static readonly string HighlightMarkerLayerKey = "HighlightMarkerLayer";
        public static readonly string HighlightCenterMarkerLayerKey = "HighlightCenterMarkerLayer";

        public static readonly string SchoolsLayerKey = "SchoolsLayer";
        public static readonly string RestaurantsLayerKey = "RestaurantsLayer";
        public static readonly string PublicFacilitiesLayerKey = "PublicFacilitiesLayer";
        public static readonly string MedicalFacilitiesLayerKey = "MedicalFacilitiesLayer";
        public static readonly string HotelsLayerKey = "HotelsLayer";

        public static readonly string ThinkGeoCloudMapsOverlayKey = "ThinkGeoCloudMapsOverlay";
        public static readonly string HighLightOverlayKey = "HighlightOverlay";

        public static readonly string AllFeatureKey = "All";

        private static UIStoryboard storyboard;
        private static Dictionary<string, UIViewController> controllers;

        public static FilterConfiguration FilterConfiguration { get; private set; }

        private static UIStoryboard Storyboard
        {
            get
            {
                string storyboardName = "MainForm";
                return storyboard ?? (storyboard = UIStoryboard.FromName(storyboardName, null));
            }
        }

        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudMapsOverlay
        {
            get { return (ThinkGeoCloudRasterMapsOverlay)MapView.Overlays[ThinkGeoCloudMapsOverlayKey]; }
        }

        public static LayerOverlay HighLightOverlay
        {
            get { return (LayerOverlay)MapView.Overlays[HighLightOverlayKey]; }
        }

        public static MapView MapView { get; set; }

        public static BaseMapType BaseMapType { get; set; }

        public static string BaseMapTypeString { get; set; }

        static Global()
        {
            FilterConfiguration = new FilterConfiguration();
            controllers = new Dictionary<string, UIViewController>();
        }

        public static Proj4Projection GetWgs84ToMercatorProjection()
        {
            Proj4Projection wgs84ToMercatorProjection = new Proj4Projection();
            wgs84ToMercatorProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            wgs84ToMercatorProjection.ExternalProjectionParametersString = Proj4Projection.GetBingMapParametersString();
            return wgs84ToMercatorProjection;
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
    }
}