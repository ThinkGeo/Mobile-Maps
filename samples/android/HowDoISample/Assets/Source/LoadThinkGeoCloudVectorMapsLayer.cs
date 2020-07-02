using Android.App;
using Android.OS;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadThinkGeoCloudVectorMapsLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ThinkGeoCloudVectorMapsLayer thinkGeoCloudVectorMapsLayer = new ThinkGeoCloudVectorMapsLayer(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret, ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsLayer.BitmapTileCache = null;
            thinkGeoCloudVectorMapsLayer.VectorTileCache = null;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.Layers.Add(thinkGeoCloudVectorMapsLayer);
             
            androidMap.Overlays.Add(layerOverlay);
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.Refresh();
        }
    }
}