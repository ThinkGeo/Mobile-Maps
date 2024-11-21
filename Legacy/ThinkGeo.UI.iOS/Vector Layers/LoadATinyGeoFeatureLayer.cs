using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadATinyGeoFeatureLayer : BaseViewController
    {
        public LoadATinyGeoFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;

            // ThinkGeoCloudRasterMapsOverlay worldMapKitOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            // MapView.Overlays.Add(worldMapKitOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            MapView.Overlays.Add(layerOverlay);
            View.AddSubview(MapView);

            TinyGeoFeatureLayer tinyGeoFeatureLayer = new TinyGeoFeatureLayer("AppData/Frisco/Frisco.tgeo");
            tinyGeoFeatureLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel11.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel11.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level14;
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            tinyGeoFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layerOverlay.Layers.Add(tinyGeoFeatureLayer);

            tinyGeoFeatureLayer.Open();
            MapView.CurrentExtent = tinyGeoFeatureLayer.GetBoundingBox();
            tinyGeoFeatureLayer.Close();

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}