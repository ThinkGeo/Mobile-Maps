using Android.Content;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class SimplifySample : BaseSample
    {
        private static readonly double simplifyTolerence = 30;

        public SimplifySample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            AreaBaseShape sourceShape = (AreaBaseShape)sourceLayer.InternalFeatures.FirstOrDefault().GetShape();
            SimplificationType simplificationType = SimplificationType.DouglasPeucker;

            MultipolygonShape multipolygonShape = sourceShape.Simplify(GeographyUnit.Meter, simplifyTolerence, DistanceUnit.Meter, simplificationType);
            sourceLayer.InternalFeatures.Clear();
            sourceLayer.InternalFeatures.Add(new Feature(multipolygonShape));

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            Feature feature = GeometrySource.FirstOrDefault();
            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.InternalFeatures.Add(feature);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}