using Android.App;
using Android.OS;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadThinkGeoCloudRasterMapsLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ThinkGeoCloudRasterMapsLayer thinkGeoCloudRasterMapsLayer = new ThinkGeoCloudRasterMapsLayer(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            thinkGeoCloudRasterMapsLayer.TileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add("ThinkGeoCloudRasterMapsLayer", thinkGeoCloudRasterMapsLayer);

            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            androidMap.Overlays.Add("LayerOverlay", layerOverlay);
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.Refresh();
        }
    }
}