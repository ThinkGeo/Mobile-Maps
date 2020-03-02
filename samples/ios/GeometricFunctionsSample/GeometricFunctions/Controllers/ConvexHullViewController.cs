using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class ConvexHullViewController : DetailViewController
    {
        public ConvexHullViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.PointType = PointType.Bitmap;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.Image = new GeoImage(@"fire");
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
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("ConvexHullLayer", convexHullLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            InMemoryFeatureLayer convexHullLayer = (InMemoryFeatureLayer)layerOverlay.Layers["ConvexHullLayer"];

            sourceLayer.Open();
            Collection<Feature> features = sourceLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns);

            convexHullLayer.Clear();
            MultipointShape multipointShape = new MultipointShape();
            foreach (PointShape pointShape in features.Skip(1).Select(feature => (PointShape)feature.GetShape()))
            {
                multipointShape.Points.Add(pointShape);
            }

            RingShape convexHull = multipointShape.ConvexHull();
            PolygonShape polygonShape = new PolygonShape(convexHull);
            convexHullLayer.InternalFeatures.Add(new Feature(polygonShape));
            convexHullLayer.InternalFeatures.Add(features.FirstOrDefault().GetConvexHull());

            layerOverlay.Refresh();
        }
    }
}