using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadAGpxFeatureLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ValueStyle pointStyle = new ValueStyle();
            pointStyle.ColumnName = "IsWayPoint";
            pointStyle.ValueItems.Add(new ValueItem("0", PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Red, 4)));
            pointStyle.ValueItems.Add(new ValueItem("1", PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Green, 8)));

            LineStyle roadstyle = LineStyle.CreateSimpleLineStyle(GeoColors.Black, 1, true);

            TextStyle labelStyle = TextStyle.CreateSimpleTextStyle("name", "Arial", 8, DrawingFontStyles.Bold, GeoColors.Black);
            labelStyle.TextPlacement = TextPlacement.Upper;
            labelStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelStyle.YOffsetInPixel = 8;

            GpxFeatureLayer gpxFeatureLayer = new GpxFeatureLayer(SampleHelper.GetDataPath(@"Gpx/afoxboro.gpx"));
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(roadstyle);
            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            gpxFeatureLayer.Open();

            GpxFeatureLayer gpxTextLayer = new GpxFeatureLayer(SampleHelper.GetDataPath(@"Gpx/afoxboro.gpx"));
            gpxTextLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(labelStyle);
            gpxTextLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(gpxFeatureLayer);
            layerOverlay.Layers.Add(gpxTextLayer);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            
            mapView.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);
            mapView.Overlays.Add(layerOverlay);
            mapView.CurrentExtent = gpxFeatureLayer.GetBoundingBox();
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}