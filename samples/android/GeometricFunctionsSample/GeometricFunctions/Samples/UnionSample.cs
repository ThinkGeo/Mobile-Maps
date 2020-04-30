using System.Collections.Generic;
using System.Linq;
using Android.Content;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class UnionSample : BaseSample
    {
        public UnionSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            if (sourceLayer.InternalFeatures.Count > 1)
            {
                IEnumerable<AreaBaseShape> areaBaseShapes = sourceLayer.InternalFeatures.Select(f => f.GetShape()).OfType<AreaBaseShape>();

                AreaBaseShape unionedAreaBaseShape = AreaBaseShape.Union(areaBaseShapes);
                sourceLayer.InternalFeatures.Clear();
                sourceLayer.InternalFeatures.Add(new Feature(unionedAreaBaseShape));

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
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            foreach (Feature feature in GeometrySource)
            {
                sourceLayer.InternalFeatures.Add(feature);
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}