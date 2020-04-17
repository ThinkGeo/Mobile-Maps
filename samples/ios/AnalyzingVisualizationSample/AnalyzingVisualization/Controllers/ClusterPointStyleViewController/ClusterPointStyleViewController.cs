/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;

namespace AnalyzingVisualization
{
    public class ClusterPointStyleViewController : DetailViewController
    {
        public ClusterPointStyleViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            
            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");

            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer usEarthquakeLayer = new ShapeFileFeatureLayer("AppData/usEarthquake.shp");
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetClusterPointStyle());
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;

            layerOverlay.Layers.Add(usEarthquakeLayer);
            MapView.Overlays.Add(layerOverlay);
            MapView.ZoomTo(new PointShape(-12777397, 4821690), MapView.ZoomLevelSet.ZoomLevel05.Scale);
        }

        private static ClassBreakClusterPointStyle GetClusterPointStyle()
        {
            ClassBreakClusterPointStyle clusterPointStyle = new ClassBreakClusterPointStyle();
            clusterPointStyle.CellSize = 65;

            PointStyle pointStyle1 = new PointStyle(PointSymbolType.Circle, 8, new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 5));
            PointStyle pointStyle2 = new PointStyle(PointSymbolType.Circle, 15,  new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 8));
            PointStyle pointStyle3 = new PointStyle(PointSymbolType.Circle, 25, new GeoSolidBrush(GeoColor.FromArgb(250, 255, 183, 76)), new GeoPen(GeoColor.FromArgb(100, 255, 183, 76), 10));
            PointStyle pointStyle4 = new PointStyle(PointSymbolType.Circle, 35, new GeoSolidBrush(GeoColor.FromArgb(250, 243, 193, 26)), new GeoPen(GeoColor.FromArgb(100, 243, 193, 26), 15));
            PointStyle pointStyle5 = new PointStyle(PointSymbolType.Circle, 40, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 15));
            PointStyle pointStyle6 = new PointStyle(PointSymbolType.Circle, 50, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 20));

            clusterPointStyle.ClassBreakPoint.Add(1, pointStyle1);
            clusterPointStyle.ClassBreakPoint.Add(2, pointStyle2);
            clusterPointStyle.ClassBreakPoint.Add(50, pointStyle3);
            clusterPointStyle.ClassBreakPoint.Add(150, pointStyle4);
            clusterPointStyle.ClassBreakPoint.Add(350, pointStyle5);
            clusterPointStyle.ClassBreakPoint.Add(500, pointStyle6);

            clusterPointStyle.TextStyle = TextStyle.CreateSimpleTextStyle("FeatureCount", "Arail", 10, DrawingFontStyles.Regular, GeoColors.Black);
            clusterPointStyle.TextStyle.TextPlacement = TextPlacement.Center;
            return clusterPointStyle;
        }
    }
}