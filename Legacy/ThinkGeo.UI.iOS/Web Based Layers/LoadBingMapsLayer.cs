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

            var bingMapsLayer = new BingMapsAsyncLayer();
            bingMapsLayer.ApplicationId = "Your Application Id";
            bingMapsLayer.ApplicationId = "Amg9BxyuF81NEyxKm2ESMaoL03MTvaYBV3KOfxpHeXDsEt38DVwK4-SPFPg6qcBp";
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
            MapView.ZoomLevelSet = new BingMapsZoomLevelSet();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}