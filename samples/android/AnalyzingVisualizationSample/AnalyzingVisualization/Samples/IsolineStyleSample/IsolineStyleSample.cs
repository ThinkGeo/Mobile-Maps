using Android.Content;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace AnalyzingVisualization
{
    public class IsolineStyleSample : BaseSample
    {
        public IsolineStyleSample(Context context)
            : base(context)
        { }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-15116491.8671313, 8720801.79162702, -11021545.2583953, 2603975.29482756);

            EarthquakeIsoLineFeatureLayer usEarthquakeIsoLayer = new EarthquakeIsoLineFeatureLayer(new ShapeFileFeatureSource(SampleHelper.GetDataPath("usEarthquake_Simplified.shp")));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.Layers.Add(usEarthquakeIsoLayer);
            MapView.Overlays.Add(layerOverlay);
        }
    }
}