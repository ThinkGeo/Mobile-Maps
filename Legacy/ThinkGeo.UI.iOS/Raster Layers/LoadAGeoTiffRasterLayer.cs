using System.IO;
using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadAGeoTiffRasterLayer : BaseViewController
    {
        public LoadAGeoTiffRasterLayer()
            : base()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            string targetDirectory = "AppData/Tiff";
            GeoTiffRasterLayer geoTiffRasterLayer = new GeoTiffRasterLayer(Path.Combine(targetDirectory, "World.tif"));
            geoTiffRasterLayer.UpperThreshold = double.MaxValue;
            geoTiffRasterLayer.LowerThreshold = 0;
            geoTiffRasterLayer.IsGrayscale = false;
            layerOverlay.Layers.Add(geoTiffRasterLayer);

            geoTiffRasterLayer.Open();
            MapView.CurrentExtent = geoTiffRasterLayer.GetBoundingBox();
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}