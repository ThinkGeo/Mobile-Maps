using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadBingMapsLayer : BaseViewController
    {
        public LoadBingMapsLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            BingMapsLayer bingMapsLayer = new BingMapsLayer();
            bingMapsLayer.ApplicationId = "Your Application Id";
            bingMapsLayer.TileCache = null;
            bingMapsLayer.ProjectedTileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 256;
            layerOverlay.TileHeight = 256;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add(bingMapsLayer);

            MapView.Overlays.Add(layerOverlay);
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.ZoomLevelSet = new BingMapsZoomLevelSet(256);
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}