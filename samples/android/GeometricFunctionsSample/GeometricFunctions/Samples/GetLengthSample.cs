using Android.Content;
using Android.Widget;
using System.Globalization;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class GetLengthSample : BaseSample
    {
        private Context context;

        public GetLengthSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        {
            this.context = context;
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

                Toast.MakeText(context, lengthContent, ToastLength.Long).Show();

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
            sourceLayer.Columns.Add(new FeatureSourceColumn("Length"));
            sourceLayer.Close();

            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Length";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextBrush = new GeoSolidBrush(GeoColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColors.White), 1);

            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 9.2F, GeoColors.DarkGray, 12.2F, true);
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(180, 255, 155, 13));
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            sourceLayer.InternalFeatures.Add(GeometrySource.First());

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}