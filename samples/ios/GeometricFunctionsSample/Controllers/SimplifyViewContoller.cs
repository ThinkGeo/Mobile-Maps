using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;

namespace GeometricFunctions
{
    public class SimplifyViewController : DetailViewController
    {
        private static readonly double simplifyTolerence = 30;

        public SimplifyViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            Feature feature = GeometrySource.FirstOrDefault();
            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.InternalFeatures.Add(feature);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

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
    }
}