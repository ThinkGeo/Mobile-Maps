using Android.Content;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class ClipSample : BaseSample
    {
        public ClipSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            InMemoryFeatureLayer clipLayer = (InMemoryFeatureLayer)layerOverlay.Layers["ClippingLayer"];
            Feature clippingFeature = clipLayer.InternalFeatures.FirstOrDefault();

            if (clippingFeature != null)
            {
                sourceLayer.Open();
                Collection<Feature> features = sourceLayer.QueryTools.GetFeaturesIntersecting(clippingFeature.GetShape(), ReturningColumnsType.AllColumns);
                sourceLayer.InternalFeatures.Clear();
                clipLayer.InternalFeatures.Clear();
                foreach (Feature feature in features)
                {
                    sourceLayer.InternalFeatures.Add(feature.GetIntersection(clippingFeature));
                }
            }

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(160, 255, 248, 172)));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 1;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            foreach (Feature feature in GeometrySource.Skip(1))
            {
                sourceLayer.InternalFeatures.Add(feature);
            }

            InMemoryFeatureLayer clippingLayer = new InMemoryFeatureLayer();
            clippingLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            clippingLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            clippingLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.FromArgb(255, 255, 155, 13);
            clippingLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            clippingLayer.InternalFeatures.Add(GeometrySource.FirstOrDefault());

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("ClippingLayer", clippingLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}