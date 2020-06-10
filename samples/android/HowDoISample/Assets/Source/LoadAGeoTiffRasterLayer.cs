using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadAGeoTiffRasterLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            GeoTiffRasterLayer geoTiffRasterLayer = new GeoTiffRasterLayer(SampleHelper.GetDataPath(@"Tiff/World.tif"));
            geoTiffRasterLayer.UpperThreshold = double.MaxValue;
            geoTiffRasterLayer.IsGrayscale = false;
            geoTiffRasterLayer.LowerThreshold = 0;
            geoTiffRasterLayer.Open();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(geoTiffRasterLayer);

            
            androidMap.CurrentExtent = geoTiffRasterLayer.GetBoundingBox();
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}