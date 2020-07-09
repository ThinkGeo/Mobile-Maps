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
             
            mapView.Overlays.Add(layerOverlay);
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            mapView.Refresh();
        }
    }
}