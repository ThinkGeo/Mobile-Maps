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
    public class ScaleSample : BaseSample
    {
        public ScaleSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            Feature sourceFeature = sourceLayer.InternalFeatures.First();

            sourceLayer.Open();
            sourceLayer.EditTools.BeginTransaction();
            sourceLayer.EditTools.ScaleUp(sourceFeature.Id, 10);
            sourceLayer.EditTools.CommitTransaction();

            layerOverlay.Refresh();
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
            Feature sourceFeature = GeometrySource.First();
            sourceLayer.InternalFeatures.Add(sourceFeature.Id, sourceFeature);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}