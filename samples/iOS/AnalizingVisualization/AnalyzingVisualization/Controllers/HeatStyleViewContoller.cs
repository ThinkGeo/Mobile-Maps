/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace AnalyzingVisualization
{
    public class HeatStyleViewContoller : DetailViewController
    {
        public HeatStyleViewContoller()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);
            MapView.CurrentExtent = new RectangleShape(-13814438, 6265892, -12272095, 3342085);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer usEarthquakeLayer = new ShapeFileFeatureLayer("AppData/usEarthquake_Simplified.shp");
            MapView.Overlays.Add(new LayerOverlay(new Layer[] { usEarthquakeLayer })
            {
                TileWidth = 512,
                TileHeight = 512
            });

            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 150, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer));
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            MapView.Refresh();
        }
    }
}