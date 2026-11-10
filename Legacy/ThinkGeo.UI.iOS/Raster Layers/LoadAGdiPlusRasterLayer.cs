using System.IO;
using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadAGdiPlusRasterLayer : BaseViewController
    {
        public LoadAGdiPlusRasterLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            string targetDirectory = "AppData/Gif";
            SkiaRasterLayer radarImageLayer = new SkiaRasterLayer(Path.Combine(targetDirectory, "EWX_N0R_0.gif"));
            radarImageLayer.UpperScale = double.MaxValue;
            radarImageLayer.LowerScale = 0;
            layerOverlay.Layers.Add(radarImageLayer);
            layerOverlay.TileType = TileType.SingleTile;

            radarImageLayer.Open();
            MapView.CurrentExtent = radarImageLayer.GetBoundingBox();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}