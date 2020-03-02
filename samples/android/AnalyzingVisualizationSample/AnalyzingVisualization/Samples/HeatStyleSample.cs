using Android.Content;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace AnalyzingVisualization
{
    public class HeatStyleSample : BaseSample
    {
        public HeatStyleSample(Context context)
            : base(context)
        { }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-13814438, 6265892, -12272095, 3342085);

            ShapeFileFeatureLayer usEarthquakeLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usEarthquake_Simplified.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(usEarthquakeLayer);
            MapView.Overlays.Add(layerOverlay);

            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 150, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer));
            usEarthquakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}