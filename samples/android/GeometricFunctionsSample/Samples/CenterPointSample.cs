using Android.Content;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class CenterPointSample : BaseSample
    {
        private ProjectionConverter projection;

        public CenterPointSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            InMemoryFeatureLayer pointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["PointLayer"];
            InMemoryFeatureLayer sourceLayer = (InMemoryFeatureLayer)layerOverlay.Layers["SourceLayer"];

            Feature feature = sourceLayer.InternalFeatures.FirstOrDefault();
            pointLayer.Clear();
            Feature centerFeature = new Feature(feature.GetBoundingBox().GetCenterPoint());
            PointShape centerPoint = (PointShape)projection.ConvertToExternalProjection(centerFeature.CloneDeep()).GetShape();
            centerFeature.ColumnValues["Type"] = $"Center at Lon: {centerPoint.X:N4}, Lat: {centerPoint.Y:N4}";
            pointLayer.InternalFeatures.Add(centerFeature);
            Feature centroidFeature = new Feature(feature.GetShape().GetCenterPoint());
            PointShape centroidPoint = (PointShape)projection.ConvertToExternalProjection(centroidFeature.CloneDeep()).GetShape();
            centroidFeature.ColumnValues["Type"] = $"Centroid at Lon: {centroidPoint.X:N4}, Lat: {centroidPoint.Y:N4}";
            pointLayer.InternalFeatures.Add(centroidFeature);

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            RectangleShape mapExtent = (RectangleShape)MapUtil.GetBoundingBoxOfItems(GeometrySource).CloneDeep();
            mapExtent.ScaleUp(20);
            MapView.CurrentExtent = mapExtent;

            projection = new ProjectionConverter();
            projection.InternalProjection = new Projection(Projection.GetSphericalMercatorProjString());
            projection.ExternalProjection = new Projection(Projection.GetLatLongProjString());
            projection.Open();

            InMemoryFeatureLayer sourceLayer = new InMemoryFeatureLayer();
            sourceLayer.InternalFeatures.Add(GeometrySource.FirstOrDefault());
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(BrushColor));
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width = 3;
            sourceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            sourceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer pointLayer = new InMemoryFeatureLayer();
            pointLayer.Open();
            pointLayer.Columns.Add(new FeatureSourceColumn("Type"));
            pointLayer.Close();
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolSize = 12;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 255, 155, 13), 2);
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 255, 248, 172));
            TextStyle textStyle = new TextStyle();
            textStyle.TextColumnName = "Type";
            textStyle.Font = new GeoFont("Arial", 15);
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.TextBrush = new GeoSolidBrush(GeoColors.Black);
            textStyle.HaloPen = new GeoPen(new GeoSolidBrush(GeoColors.White), 1);
            textStyle.TextPlacement = TextPlacement.Lower;
            textStyle.YOffsetInPixel = -8;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            pointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", sourceLayer);
            layerOverlay.Layers.Add("PointLayer", pointLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }
    }
}