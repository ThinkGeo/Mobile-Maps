using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadATinyGeoFeatureLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            TinyGeoFeatureLayer tinyGeoFeatureLayer = new TinyGeoFeatureLayer(SampleHelper.GetDataPath(@"Frisco/Frisco.tgeo"));
            tinyGeoFeatureLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel11.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel11.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level14;
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            tinyGeoFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            tinyGeoFeatureLayer.Open();
            var currentExtent = tinyGeoFeatureLayer.GetBoundingBox();
            tinyGeoFeatureLayer.Close();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.Layers.Add(tinyGeoFeatureLayer);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);
            mapView.Overlays.Add(layerOverlay);
            mapView.CurrentExtent = currentExtent;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}