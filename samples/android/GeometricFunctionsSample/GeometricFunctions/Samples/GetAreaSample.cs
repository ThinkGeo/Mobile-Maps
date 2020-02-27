using Android.Content;
using Android.Widget;
using System.Globalization;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace GeometricFunctions
{
    public class GetAreaSample : BaseSample
    {
        private Context context;

        public GetAreaSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        {
            this.context = context;
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

                Toast.MakeText(context, areaMessage, ToastLength.Long).Show();
                layerOverlay.Refresh();
            }
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.Open();
            sourceLayer.Columns.Add(new FeatureSourceColumn("Area"));

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;

            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Area";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColor.StandardColors.White), 1);
            textStyle.PointPlacement = PointPlacement.LowerCenter;
            textStyle.YOffsetInPixel = -8;

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            GeometrySource.First().ColumnValues.Clear();
            sourceLayer.InternalFeatures.Add(GeometrySource.First());

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}