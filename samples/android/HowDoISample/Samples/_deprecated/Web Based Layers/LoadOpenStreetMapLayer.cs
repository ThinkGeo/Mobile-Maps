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

            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            mapView.Overlays.Add("LayerOverlay", layerOverlay);
            mapView.ZoomLevelSet = new OpenStreetMapsZoomLevelSet();
            mapView.Refresh();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo);
        }
    }
}