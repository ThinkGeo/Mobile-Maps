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
            NativeImageRasterLayer radarImageLayer = new NativeImageRasterLayer(Path.Combine(targetDirectory, "EWX_N0R_0.gif"));
            radarImageLayer.UpperThreshold = double.MaxValue;
            radarImageLayer.LowerThreshold = 0;
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