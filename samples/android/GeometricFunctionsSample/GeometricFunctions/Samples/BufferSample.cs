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
    public class BufferSample : BaseSample
    {
        private int bufferSize;

        public BufferSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            InMemoryFeatureLayer bufferedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["BufferedLayer"];

            bufferSize += 15;
            Feature bufferSourceFeature = sourceLayer.InternalFeatures[0].CloneDeep();
            Feature bufferedFeature = bufferSourceFeature.Buffer(bufferSize, 8, BufferCapType.Butt, GeographyUnit.Meter, DistanceUnit.Meter);
            bufferedLayer.InternalFeatures.Clear();
            bufferedLayer.InternalFeatures.Add(bufferedFeature);

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            bufferSize = 0;
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer bufferedLayer = new InMemoryFeatureLayer();
            bufferedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(140, 255, 155, 13)));
            bufferedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.InternalFeatures.Add(GeometrySource.FirstOrDefault());
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("BufferedLayer", bufferedLayer);
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}