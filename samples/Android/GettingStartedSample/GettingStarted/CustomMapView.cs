/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.Content;
using ThinkGeo.MapSuite.Android;

namespace GettingStartedSample
{
    public class CustomMapView : MapView
    {
        private static CustomMapView current;
        private BaseMapType baseMapType;

        public static CustomMapView Current
        {
            get { return current ?? (current = new CustomMapView(Application.Context)); }
        }

        protected CustomMapView(Context context)
                : base(context)
        {
            baseMapType = BaseMapType.ThinkGeoCloudMapLight;

            LoadOverlays();
        }

        private void LoadOverlays()
        {
            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);
        }

        public BaseMapType BaseMapType
        {
            get { return baseMapType; }
            set { baseMapType = value; }
        }

        public void SwitchBaseMapTo(BaseMapType baseMapType)
        {
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = (ThinkGeoCloudRasterMapsOverlay)Overlays["ThinkGeoCloudMapsOverlay"];

            thinkGeoCloudMapsOverlay.IsVisible = baseMapType == BaseMapType.ThinkGeoCloudMapAerial || baseMapType == BaseMapType.ThinkGeoCloudMapLight || baseMapType == BaseMapType.ThinkGeoCloudMapBybrid;

            switch (baseMapType)
            {
                case BaseMapType.ThinkGeoCloudMapAerial:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                    break;
                case BaseMapType.ThinkGeoCloudMapBybrid:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                    break;
                case BaseMapType.ThinkGeoCloudMapLight:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                    break;
                default:
                    break;
            }

            BaseMapType = baseMapType;
        }
    }
}