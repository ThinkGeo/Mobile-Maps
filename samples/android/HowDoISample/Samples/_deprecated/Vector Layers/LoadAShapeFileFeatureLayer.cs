using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadAShapeFileFeatureLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer txwatFeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXwat.shp"));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 153, 179, 204));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("LandName", "Arial", 9, DrawingFontStyles.Italic, GeoColors.Navy);
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXlkaA40.shp"));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlka40FeatureLayer.DrawingMarginInPixel = 200;

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXlkaA20.shp"));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 6, GeoColors.LightGray, 9, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColors.LightGray, 12, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 12, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(txwatFeatureLayer);
            layerOverlay.Layers.Add(txlka40FeatureLayer);
            layerOverlay.Layers.Add(txlkaA20FeatureLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);
            mapView.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}