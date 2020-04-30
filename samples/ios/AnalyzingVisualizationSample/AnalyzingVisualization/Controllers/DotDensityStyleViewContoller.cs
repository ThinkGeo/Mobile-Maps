/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using ThinkGeo.UI.iOS;
using ThinkGeo.Core;

namespace AnalyzingVisualization
{
    public class DotDensityStyleViewContoller : DetailViewController
    {
        public DotDensityStyleViewContoller()
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer usLayer = new ShapeFileFeatureLayer("AppData/usStatesCensus2010.shp");
            LayerOverlay layerOverlay = new LayerOverlay();
       //     layerOverlay.TileType = TileType.SingleTile;
           
            layerOverlay.Layers.Add(usLayer);
            MapView.Overlays.Add(layerOverlay);

            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(20, GeoColor.FromHtml("#00e6fe")), GeoColors.Gray));
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetDotDensityStyle());
            usLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            MapView.ZoomTo(new PointShape(-10777397, 4821690), MapView.ZoomLevelSet.ZoomLevel05.Scale);
        }

        private static DotDensityStyle GetDotDensityStyle()
        {
            double pointToValueRatio = 0.0000094778167166538189;
            PointStyle pointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Black, 7);
            DotDensityStyle dotDensityStyle = new DotDensityStyle("Population", pointToValueRatio, pointStyle);
            dotDensityStyle.CustomPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColor.FromHtml("#a57431"), 5);
            return dotDensityStyle;
        }
    }
}