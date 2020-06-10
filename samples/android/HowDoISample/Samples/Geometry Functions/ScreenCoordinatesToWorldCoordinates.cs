using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class ScreenCoordinatesToWorldCoordinates : SampleFragment
    {
        private MapView androidMap;
        private TextView screenPositionLable;
        private TextView worldPositionLable;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add(layerOverlay);
            androidMap.SingleTap += AndroidMap_SingleTap; ;

            screenPositionLable = new TextView(this.Context);
            worldPositionLable = new TextView(this.Context);
            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { screenPositionLable, worldPositionLable });
        }

        private void AndroidMap_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            screenPositionLable.Text = string.Format("Screen Position:({0:N4},{1:N4})", e.ScreenX, e.ScreenY);
            worldPositionLable.Text = string.Format("World Position: ({0:N4},{1:N4})", e.WorldX, e.WorldY);
        }
    }
}