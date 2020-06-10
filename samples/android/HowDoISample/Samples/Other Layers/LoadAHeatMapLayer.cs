using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadAHeatMapLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer layer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/cities_e.shp"));
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 75, DistanceUnit.Kilometer));
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(layer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = (new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625));
            androidMap.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}