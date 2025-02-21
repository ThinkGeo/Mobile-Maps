using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadOpenStreetMapLayer : BaseViewController
    {
        public LoadOpenStreetMapLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var openStreetMapLayer = new OpenStreetMapAsyncLayer("Android Test");
            openStreetMapLayer.TileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 256;
            layerOverlay.TileHeight = 256;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add("OpenStreetMapLayer", openStreetMapLayer);

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
            MapView.ZoomLevelSet = new OpenStreetMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}