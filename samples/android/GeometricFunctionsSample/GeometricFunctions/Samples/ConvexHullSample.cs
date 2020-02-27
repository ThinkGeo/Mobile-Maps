using System.Linq;
using Android.Content;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class ConvexHullSample : BaseSample
    {
        public ConvexHullSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            InMemoryFeatureLayer convexHullLayer = (InMemoryFeatureLayer)layerOverlay.Layers["ConvexHullLayer"];

            sourceLayer.Open();
            Collection<Feature> features = sourceLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns);

            convexHullLayer.Clear();
            MultipointShape multipointShape = new MultipointShape();
            foreach (Feature feature in features.Skip(1))
            {
                PointShape pointShape = (PointShape)feature.GetShape();
                multipointShape.Points.Add(pointShape);
            }

            RingShape convexHull = multipointShape.ConvexHull();
            PolygonShape polygonShape = new PolygonShape(convexHull);
            convexHullLayer.InternalFeatures.Add(new Feature(polygonShape));
            convexHullLayer.InternalFeatures.Add(features.FirstOrDefault().GetConvexHull());

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.PointType = PointType.Bitmap;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.Image = new GeoImage(SampleHelper.GetDataPath(@"fire.png"));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            foreach (Feature feature in GeometrySource)
            {
                sourceLayer.InternalFeatures.Add(feature);
            }

            InMemoryFeatureLayer convexHullLayer = new InMemoryFeatureLayer();
            convexHullLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            convexHullLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            convexHullLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            convexHullLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("ConvexHullLayer", convexHullLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}