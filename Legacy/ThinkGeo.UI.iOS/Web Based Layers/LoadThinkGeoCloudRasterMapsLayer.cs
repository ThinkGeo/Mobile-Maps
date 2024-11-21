using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadThinkGeoCloudRasterMapsLayer : BaseViewController
    {
        public LoadThinkGeoCloudRasterMapsLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var thinkGeoCloudRasterMapsLayer = new ThinkGeoVectorMapsAsyncLayer(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            thinkGeoCloudRasterMapsLayer.BitmapTileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add("ThinkGeoCloudRasterMapsLayer", thinkGeoCloudRasterMapsLayer);

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}