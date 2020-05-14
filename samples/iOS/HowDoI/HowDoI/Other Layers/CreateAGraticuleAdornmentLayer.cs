using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class CreateAGraticuleAdornmentLayer : BaseViewController
    {
        public CreateAGraticuleAdornmentLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            GraticuleFeatureLayer graticuleFeatureLayer = new GraticuleFeatureLayer();
            graticuleFeatureLayer.WrappingMode = WrappingMode.WrapDateline;
            graticuleFeatureLayer.GraticuleLineStyle.OuterPen.Color = GeoColor.FromArgb(125, GeoColors.Navy);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.Layers.Add(graticuleFeatureLayer);

            MapView.Overlays.Add(layerOverlay);
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80, null);
        }
    }
}