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
    public class IsolineStyleViewContoller : DetailViewController
    {
        public IsolineStyleViewContoller()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);
            MapView.CurrentExtent = new RectangleShape(-15116491.8671313, 8720801.79162702, -11021545.2583953, 2603975.29482756);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureSource usEarthquakeIsoSource = new ShapeFileFeatureSource("AppData/usEarthquake_Simplified.shp");
            EarthquakeIsoLineFeatureLayer usEarthquakeIsoLayer = new EarthquakeIsoLineFeatureLayer(usEarthquakeIsoSource);
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(usEarthquakeIsoLayer);
            layerOverlay.TileHeight = 512;
            layerOverlay.TileWidth = 512;

            MapView.Overlays.Add(layerOverlay);
            MapView.Refresh();
        }
    }
}