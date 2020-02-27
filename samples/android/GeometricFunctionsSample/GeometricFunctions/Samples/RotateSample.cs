using Android.Content;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class RotateSample : BaseSample
    {
        public RotateSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            Feature sourceFeature = sourceLayer.InternalFeatures.FirstOrDefault();
            if (sourceFeature != null && sourceFeature.GetShape().CanRotate)
            {
                PointShape center = sourceFeature.GetBoundingBox().GetCenterPoint();
                BaseShape rotatedShape = BaseShape.Rotate(sourceFeature.GetShape(), center, 22.5f);
                sourceLayer.InternalFeatures.Clear();
                sourceLayer.InternalFeatures.Add(new Feature(rotatedShape));
                layerOverlay.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.InternalFeatures.Add(GeometrySource.FirstOrDefault());

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}