using Android.App;
using Android.OS;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadBingMapsLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            BingMapsLayer bingMapsLayer = new BingMapsLayer();
            // Please set your own information about those parameters below.
            bingMapsLayer.ApplicationId = "Your Application Id";
            bingMapsLayer.MapType = BingMapsMapType.AerialWithLabels;
            bingMapsLayer.TileCache = null;
            bingMapsLayer.ProjectedTileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("BingMapsLayer", bingMapsLayer);
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;

           androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            androidMap.ZoomLevelSet = new BingMapsZoomLevelSet();
            androidMap.Overlays.Add("LayerOverlay", layerOverlay);
            androidMap.Refresh();
        }
    }
}