using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadOpenStreetMapLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            OpenStreetMapLayer openStreetMapLayer = new OpenStreetMapLayer("Android Test");
            openStreetMapLayer.TileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.Default;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add("OpenStreetMapLayer", openStreetMapLayer);

            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            androidMap.Overlays.Add("LayerOverlay", layerOverlay);
            androidMap.ZoomLevelSet = new OpenStreetMapsZoomLevelSet();
            androidMap.Refresh();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo);
        }
    }
}