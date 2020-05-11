using System.Globalization;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GeometricFunctions
{
    public class GetAreaViewController : DetailViewController
    {
        public GetAreaViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.Open();
            sourceLayer.Columns.Add(new FeatureSourceColumn("Area"));

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;

            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Area";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextBrush = new GeoSolidBrush(GeoColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColors.White), 1);
            textStyle.TextPlacement = TextPlacement.Lower;
            textStyle.YOffsetInPixel = -8;

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            sourceLayer.InternalFeatures.Add(GeometrySource.First());

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
            Feature feature = sourceLayer.InternalFeatures.FirstOrDefault();
            if (feature != null)
            {
                AreaBaseShape areaShape = (AreaBaseShape)feature.GetShape();
                double metersArea = areaShape.GetArea(GeographyUnit.Meter, AreaUnit.SquareMeters);
                double hectaresArea = areaShape.GetArea(GeographyUnit.Meter, AreaUnit.Hectares);

                string areaMessage = string.Format(CultureInfo.InvariantCulture, "{0:N2} Hectares, \r\n {1:N2} Acres", metersArea, hectaresArea);
                feature.ColumnValues["Area"] = areaMessage;
                UIAlertView alertView = new UIAlertView("Area", areaMessage, null, "OK");
                alertView.Show();

                layerOverlay.Refresh();
            }
        }
    }
}