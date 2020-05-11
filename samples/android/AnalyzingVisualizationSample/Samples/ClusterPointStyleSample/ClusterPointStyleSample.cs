using Android.Content;
using Android.Util;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace AnalyzingVisualization
{
    public class ClusterPointStyleSample : BaseSample
    {
        public ClusterPointStyleSample(Context context)
            : this(context, null)
        { }

        public ClusterPointStyleSample(Context context, IAttributeSet attrs)
            : base(context)
        {
        }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-14299615, 20037508, -7255178, -1015546);

            ShapeFileFeatureLayer clusterPointStyleFeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usEarthquake.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(clusterPointStyleFeatureLayer);
            MapView.Overlays.Add(layerOverlay);

            clusterPointStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            clusterPointStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetClusterPointStyle());
            clusterPointStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        private static ClassBreakClusterPointStyle GetClusterPointStyle()
        {
            ClassBreakClusterPointStyle clusterPointStyle = new ClassBreakClusterPointStyle();
            clusterPointStyle.CellSize = 65;

            PointStyle pointStyle1 = new PointStyle(PointSymbolType.Circle, 8, new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 5));
            clusterPointStyle.ClassBreakPoint.Add(1, pointStyle1);

            PointStyle pointStyle2 = new PointStyle(PointSymbolType.Circle, 15, new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 8));
            clusterPointStyle.ClassBreakPoint.Add(2, pointStyle2);

            PointStyle pointStyle3 = new PointStyle(PointSymbolType.Circle, 25, new GeoSolidBrush(GeoColor.FromArgb(250, 255, 183, 76)), new GeoPen(GeoColor.FromArgb(100, 255, 183, 76), 10));
            clusterPointStyle.ClassBreakPoint.Add(50, pointStyle3);

            PointStyle pointStyle4 = new PointStyle(PointSymbolType.Circle, 35, new GeoSolidBrush(GeoColor.FromArgb(250, 243, 193, 26)), new GeoPen(GeoColor.FromArgb(100, 243, 193, 26), 15));
            clusterPointStyle.ClassBreakPoint.Add(150, pointStyle4);

            PointStyle pointStyle5 = new PointStyle(PointSymbolType.Circle, 40, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 15));
            clusterPointStyle.ClassBreakPoint.Add(350, pointStyle5);

            PointStyle pointStyle6 = new PointStyle(PointSymbolType.Circle, 50, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 20));
            clusterPointStyle.ClassBreakPoint.Add(500, pointStyle6);

            clusterPointStyle.TextStyle = TextStyle.CreateSimpleTextStyle("FeatureCount", "Arail", 10, DrawingFontStyles.Regular, GeoColors.Black);
            clusterPointStyle.TextStyle.TextPlacement = TextPlacement.Center;
            return clusterPointStyle;
        }
    }
}