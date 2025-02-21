using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadATabFeatureLayer : BaseViewController
    {
        public LoadATabFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;

            TabFeatureLayer tabFeatureLayer = new TabFeatureLayer("AppData/Tab/HoustonMuniBdySamp_Boundary.TAB");
            tabFeatureLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            tabFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // ThinkGeoCloudRasterMapsOverlay worldMapKitOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            // MapView.Overlays.Add(worldMapKitOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileHeight = 512;
            layerOverlay.TileWidth = 512;
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(tabFeatureLayer);

            tabFeatureLayer.Open();
            MapView.CurrentExtent = tabFeatureLayer.GetBoundingBox();
            tabFeatureLayer.Close();

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}