using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class LineOnLineViewController : DetailViewController
    {
        public LineOnLineViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true); 
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(180, 255, 155, 13));
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColor.StandardColors.Transparent, GeoColor.StandardColors.Black, 18);
            foreach (var feature in GeometrySource)
            {
                sourceLayer.InternalFeatures.Add(feature);
            }

            InMemoryFeatureLayer lineOnLineLayer = new InMemoryFeatureLayer();
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true); 
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.StandardColors.Blue);
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Brush = new GeoSolidBrush(GeoColor.StandardColors.Red);
            lineOnLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("LineOnLineLayer", lineOnLineLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer lineOnLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["LineOnLineLayer"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            Feature firstFeature = sourceLayer.InternalFeatures.FirstOrDefault();
            if (firstFeature != null)
            {
                LineShape lineShape = (LineShape)firstFeature.GetShape();
                lineOnLineLayer.InternalFeatures.Clear();

                LineBaseShape resultLine = lineShape.GetLineOnALine(StartingPoint.FirstPoint, 80, 450, GeographyUnit.Meter, DistanceUnit.Meter);
                lineOnLineLayer.InternalFeatures.Add(new Feature(resultLine));
                layerOverlay.Refresh();
            }
        }
    }
}