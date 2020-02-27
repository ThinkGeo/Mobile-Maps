using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class EnvelopViewController : DetailViewController
    {
        public EnvelopViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.InternalFeatures.Add(GeometrySource.First());

            InMemoryFeatureLayer boundingboxLayer = new InMemoryFeatureLayer();
            boundingboxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(160, 255, 248, 172)));
            boundingboxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            boundingboxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.FromArgb(255, 255, 155, 13);
            boundingboxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("BoundingboxLayer", boundingboxLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer boundingboxLayer = (InMemoryFeatureLayer)layerOverlay.Layers["BoundingboxLayer"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            Feature sourceFeature = sourceLayer.InternalFeatures.First();
            boundingboxLayer.InternalFeatures.Clear();
            boundingboxLayer.InternalFeatures.Add(new Feature(sourceFeature.GetBoundingBox()));

            layerOverlay.Refresh();
        }
    }
}