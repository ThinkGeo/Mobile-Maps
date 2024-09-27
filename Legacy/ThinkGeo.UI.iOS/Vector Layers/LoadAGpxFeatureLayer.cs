using System.IO;
using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadAGpxFeatureLayer : BaseViewController
    {
        public LoadAGpxFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;

            string targetDirectory = "AppData/Gpx";
            GpxFeatureLayer gpxFeatureLayer = new GpxFeatureLayer(Path.Combine(targetDirectory, "afoxboro.gpx"));
            ValueStyle pointStyle = new ValueStyle();
            pointStyle.ColumnName = "IsWayPoint";
            pointStyle.ValueItems.Add(new ValueItem("0", PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Red, 4)));
            pointStyle.ValueItems.Add(new ValueItem("1", PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Green, 8)));
            LineStyle roadstyle = LineStyle.CreateSimpleLineStyle(GeoColors.Black, 1, true);
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(roadstyle);
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            GpxFeatureLayer gpxTextLayer = new GpxFeatureLayer(Path.Combine(targetDirectory, "afoxboro.gpx"));
            TextStyle labelStyle = TextStyle.CreateSimpleTextStyle("name", "Arial", 8, DrawingFontStyles.Bold, GeoColors.Black);
            labelStyle.TextPlacement = TextPlacement.Upper;
            labelStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelStyle.YOffsetInPixel = 8;
            gpxTextLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(labelStyle);
            gpxTextLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ThinkGeoCloudRasterMapsOverlay worldMapKitOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            MapView.Overlays.Add(worldMapKitOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(gpxFeatureLayer);
            layerOverlay.Layers.Add(gpxTextLayer);

            gpxFeatureLayer.Open();
            MapView.CurrentExtent = gpxFeatureLayer.GetBoundingBox();

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}