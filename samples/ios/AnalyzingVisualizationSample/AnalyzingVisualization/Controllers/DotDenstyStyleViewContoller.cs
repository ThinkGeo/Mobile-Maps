/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class DotDenstyStyleViewContoller : DetailViewController
    {
        public DotDenstyStyleViewContoller()
        { }

        protected override void InitializeMap()
        {
            base.InitializeMap();
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer usLayer = new ShapeFileFeatureLayer("AppData/usStatesCensus2010.shp");
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.Layers.Add(usLayer);
            MapView.Overlays.Add(layerOverlay);

            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(20, GeoColor.FromHtml("#00e6fe")), GeoColor.StandardColors.Gray));
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetDotDensityStyle());
            usLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            MapView.ZoomTo(new PointShape(-10777397, 4821690), MapView.ZoomLevelSet.ZoomLevel05.Scale);
        }

        private static DotDensityStyle GetDotDensityStyle()
        {
            double pointToValueRatio = 0.0000094778167166538189;
            PointStyle pointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Black, 7);
            DotDensityStyle dotDensityStyle = new DotDensityStyle("Population", pointToValueRatio, pointStyle);
            dotDensityStyle.CustomPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromHtml("#a57431"), 5);
            return dotDensityStyle;
        }
    }
}