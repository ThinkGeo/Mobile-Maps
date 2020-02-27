using Android.Content;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class DotDenstyStyleSample : BaseSample
    {
        public DotDenstyStyleSample(Context context)
            : base(context)
        { }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-14299615, 20037508, -7255178, -1015546);

            ShapeFileFeatureLayer usLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usStatesCensus2010.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(usLayer);
            MapView.Overlays.Add(layerOverlay);

            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new AreaStyle(new GeoPen(new GeoSolidBrush(GeoColor.SimpleColors.Black))));
            //TODO: check this
            double pointToValueRatio = 0.0000094778167166538189;
            PointStyle pointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Black, 7);
            DotDensityStyle dotDensityStyle = new DotDensityStyle("Population", pointToValueRatio, pointStyle);
            dotDensityStyle.CustomPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromHtml("#a57431"), 5);

            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(dotDensityStyle);
            usLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}