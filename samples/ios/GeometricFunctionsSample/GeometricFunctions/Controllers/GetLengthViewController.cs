using System.Globalization;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace GeometricFunctions
{
    public class GetLengthViewController : DetailViewController
    {
        public GetLengthViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.Open();
            sourceLayer.Columns.Add(new FeatureSourceColumn("Length"));

            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Length";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColor.StandardColors.White), 1);

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true);
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(180, 255, 155, 13));
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

            Feature firstFeature = sourceLayer.InternalFeatures.FirstOrDefault();
            if (firstFeature != null)
            {
                Feature feature = firstFeature.CloneDeep();
                LineBaseShape lineShape = (LineBaseShape)feature.GetShape();
                double length = lineShape.GetLength(GeographyUnit.Meter, DistanceUnit.Mile);
                string lengthContent = string.Format(CultureInfo.InvariantCulture, "Length is {0:N2} miles.", length);
                feature.ColumnValues["Length"] = lengthContent;

                sourceLayer.InternalFeatures.Clear();
                sourceLayer.InternalFeatures.Add(feature);

                UIAlertView alertView = new UIAlertView("Length", lengthContent, null, "OK");
                alertView.Show();

                layerOverlay.Refresh();
            }
        }
    }
}