/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class ClassBreakStyleViewController : DetailViewController
    {
        public ClassBreakStyleViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ClassBreakStyle classBreakStyle = GetClassBreakStyle();
            ShapeFileFeatureLayer usLayer = new ShapeFileFeatureLayer("AppData/usStatesCensus2010.shp");
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
            usLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.Layers.Add(usLayer);
            MapView.Overlays.Add(layerOverlay);
            MapView.ZoomTo(new PointShape(-10777397, 4821690), MapView.ZoomLevelSet.ZoomLevel05.Scale);

            ClassBreakChartView chartView = new ClassBreakChartView(new CGPoint(5, View.Frame.Height - 5), classBreakStyle);
            View.AddSubview(chartView);
        }

        private static ClassBreakStyle GetClassBreakStyle()
        {
            double[] classBreakValues = { 0, 814180.0, 1328361.0, 2059179.0, 2967297.0, 4339367.0, 5303925.0, 6392017.0, 8791894.0 };

            GeoColor fromColor = GeoColor.FromArgb(255, 116, 160, 255);
            GeoColor toColor = GeoColor.FromArgb(255, 220, 52, 56);
            Collection<GeoColor> familyColors = GeoColor.GetColorsInQualityFamily(fromColor, toColor, 10, ColorWheelDirection.CounterClockwise);

            ClassBreakStyle classBreakStyle = new ClassBreakStyle("Population", BreakValueInclusion.IncludeValue);
            GeoColor outlineColor = GeoColor.FromHtml("#f05133");
            for (int i = 0; i < classBreakValues.Length; i++)
            {
                GeoColor fillColor = new GeoColor(200, familyColors[i]);
                AreaStyle areaStyle = AreaStyles.CreateSimpleAreaStyle(fillColor, outlineColor, 1);
                classBreakStyle.ClassBreaks.Add(new ClassBreak(classBreakValues[i], areaStyle));
            }
            return classBreakStyle;
        }
    }
}