using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadThinkGeoCloudVectorMapsLayer : BaseViewController
    {
        public LoadThinkGeoCloudVectorMapsLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var thinkGeoCloudVectorMapsLayer = new ThinkGeoVectorMapsAsyncLayer(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret, ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsLayer.BitmapTileCache = null;
            thinkGeoCloudVectorMapsLayer.VectorTileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add(thinkGeoCloudVectorMapsLayer);

            MapView.Overlays.Add(layerOverlay);
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}