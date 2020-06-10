using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadAGdiPlusRasterLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;

            NativeImageRasterLayer radarImageLayer = new NativeImageRasterLayer(SampleHelper.GetDataPath(@"Gif/EWX_N0R_0.gif"));
            radarImageLayer.UpperThreshold = double.MaxValue;
            radarImageLayer.LowerThreshold = 0;
            layerOverlay.Layers.Add(radarImageLayer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            radarImageLayer.Open();
            androidMap.CurrentExtent = radarImageLayer.GetBoundingBox();
            androidMap.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}