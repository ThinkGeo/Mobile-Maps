using System.Globalization;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class CenterPointViewController : DetailViewController
    {
        private Proj4Projection projection;

        public CenterPointViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            RectangleShape mapExtent = (RectangleShape)ExtentHelper.GetBoundingBoxOfItems(GeometrySource).CloneDeep();
            mapExtent.ScaleUp(20);
            MapView.CurrentExtent = mapExtent;

            projection = new Proj4Projection();
            projection.InternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            projection.ExternalProjectionParametersString = Proj4Projection.GetLatLongParametersString();
            projection.Open();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.InternalFeatures.Add(GeometrySource.FirstOrDefault());
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer pointLayer = new InMemoryFeatureLayer();
            pointLayer.Open();
            pointLayer.Columns.Add(new FeatureSourceColumn("Type"));
            pointLayer.Close();
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolSize = 12;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolPen = new GeoPen(GeoColor.FromArgb(255, 255, 155, 13), 2);
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 255, 248, 172));
            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Type";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColor.StandardColors.White), 1);
            textStyle.PointPlacement = PointPlacement.LowerCenter;
            textStyle.YOffsetInPixel = -8;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            pointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("PointLayer", pointLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer pointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["PointLayer"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            pointLayer.Clear();

            Feature feature = sourceLayer.InternalFeatures.FirstOrDefault();
            Feature centerFeature = new Feature(feature.GetBoundingBox().GetCenterPoint());
            PointShape centerPoint = (PointShape)projection.ConvertToExternalProjection(centerFeature.CloneDeep()).GetShape();
            centerFeature.ColumnValues["Type"] = string.Format(CultureInfo.InvariantCulture, "Center at Lon: {0:N4}, Lat: {1:N4}", centerPoint.X, centerPoint.Y);
            pointLayer.InternalFeatures.Add(centerFeature);

            Feature centroidFeature = new Feature(feature.GetShape().GetCenterPoint());
            PointShape centroidPoint = (PointShape)projection.ConvertToExternalProjection(centroidFeature.CloneDeep()).GetShape();
            centroidFeature.ColumnValues["Type"] = string.Format(CultureInfo.InvariantCulture, "Centroid at Lon: {0:N4}, Lat: {1:N4}", centroidPoint.X, centroidPoint.Y);
            pointLayer.InternalFeatures.Add(centroidFeature);

            layerOverlay.Refresh();
        }
    }
}