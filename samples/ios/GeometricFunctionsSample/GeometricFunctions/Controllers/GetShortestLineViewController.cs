using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;

namespace GeometricFunctions
{
    public class GetShortestLineViewController : DetailViewController
    {
        public GetShortestLineViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            RectangleShape mapExtent = MapUtil.GetBoundingBoxOfItems(GeometrySource);
            mapExtent.ScaleUp(20);
            MapView.CurrentExtent = mapExtent;

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            sourceLayer.InternalFeatures.Add("AreaShape1", GeometrySource[0]);
            sourceLayer.InternalFeatures.Add("AreaShape2", GeometrySource[1]);

            InMemoryFeatureLayer shortestLineLayer = new InMemoryFeatureLayer();
            shortestLineLayer.Open();
            shortestLineLayer.Columns.Add(new FeatureSourceColumn("Distance"));

            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = GetTextStyle();
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width = 5;
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(180, 255, 155, 13);
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("ShortestLineLayer", shortestLineLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];
            InMemoryFeatureLayer shortestLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["ShortestLineLayer"];

            BaseShape areaShape1 = sourceLayer.InternalFeatures["AreaShape1"].GetShape();
            BaseShape areaShape2 = sourceLayer.InternalFeatures["AreaShape2"].GetShape();
            MultilineShape multiLineShape = areaShape1.GetShortestLineTo(areaShape2, GeographyUnit.Meter);
            Feature feature = new Feature(multiLineShape.GetWellKnownBinary());
            feature.ColumnValues["Distance"] = string.Format(CultureInfo.InvariantCulture, "Distance is {0:N2} miles.", multiLineShape.GetLength(GeographyUnit.Meter, DistanceUnit.Mile));
            shortestLineLayer.InternalFeatures.Clear();
            shortestLineLayer.InternalFeatures.Add(feature);

            layerOverlay.Refresh();
        }

        private static TextStyle GetTextStyle()
        {
            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Distance";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextBrush = new GeoSolidBrush(GeoColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColors.White), 1);
            textStyle.TextPlacement = TextPlacement.Lower;
            textStyle.YOffsetInPixel = -8;
            return textStyle;
        }
    }
}