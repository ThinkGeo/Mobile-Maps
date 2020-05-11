using System.Collections.Generic;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;

namespace GeometricFunctions
{
    public class UnionViewController : DetailViewController
    {
        public UnionViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
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

            if (sourceLayer.InternalFeatures.Count > 1)
            {
                IEnumerable<AreaBaseShape> areaBaseShapes = sourceLayer.InternalFeatures.Select(f => f.GetShape()).OfType<AreaBaseShape>();
                AreaBaseShape unionedAreaBaseShape = AreaBaseShape.Union(areaBaseShapes);
                sourceLayer.InternalFeatures.Clear();
                sourceLayer.InternalFeatures.Add(new Feature(unionedAreaBaseShape));

                layerOverlay.Refresh();
            }
        }
    }
}