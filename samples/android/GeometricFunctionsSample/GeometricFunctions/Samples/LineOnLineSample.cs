using System.Linq;
using Android.Content;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class LineOnLineSample : BaseSample
    {
        public LineOnLineSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer lineOnLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["LineOnLineLayer"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            Feature firstFeature = sourceLayer.InternalFeatures.FirstOrDefault();
            if (firstFeature != null)
            {
                LineShape lineShape = (LineShape)firstFeature.GetShape();
                lineOnLineLayer.InternalFeatures.Clear();

                LineBaseShape resultLine = lineShape.GetLineOnALine(StartingPoint.FirstPoint, 80, 450, GeographyUnit.Meter, DistanceUnit.Meter);
                lineOnLineLayer.InternalFeatures.Add(new Feature(resultLine));
                layerOverlay.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 9.2F, GeoColors.DarkGray, 12.2F, true);
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(180, 255, 155, 13));
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Transparent, GeoColors.Black, 18);
            foreach (var feature in GeometrySource)
            {
                sourceLayer.InternalFeatures.Add(feature);
            }

            InMemoryFeatureLayer lineOnLineLayer = new InMemoryFeatureLayer();
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 9.2F, GeoColors.DarkGray, 12.2F, true);
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColors.Blue);
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Brush = new GeoSolidBrush(GeoColors.Red);
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("LineOnLineLayer", lineOnLineLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}