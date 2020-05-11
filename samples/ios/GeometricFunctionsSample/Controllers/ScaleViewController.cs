using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;

namespace GeometricFunctions
{
    public class ScaleViewController : DetailViewController
    {
        public ScaleViewController()
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
            Feature sourceFeature = GeometrySource.First();
            sourceLayer.InternalFeatures.Add(sourceFeature.Id, sourceFeature);

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
            Feature sourceFeature = sourceLayer.InternalFeatures.First();

            sourceLayer.Open();
            sourceLayer.EditTools.BeginTransaction();
            sourceLayer.EditTools.ScaleUp(sourceFeature.Id, 10);
            sourceLayer.EditTools.CommitTransaction();

            layerOverlay.Refresh();
        }
    }
}