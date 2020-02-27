/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;

namespace GeometricFunctions
{
    public class SplitViewController : DetailViewController
    {
        public SplitViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            MapView.Refresh();
        }
    }
}