using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadATabFeatureLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ProjectionConverter projectionConverter = new ProjectionConverter(4326, 3857);

            TabFeatureLayer tabFeatureLayer = new TabFeatureLayer(SampleHelper.GetDataPath(@"Tab/HoustonMuniBdySamp_Boundary.tab"));
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColors.Green), GeoColors.Green);
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            tabFeatureLayer.FeatureSource.ProjectionConverter = projectionConverter;
            tabFeatureLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.Layers.Add(tabFeatureLayer);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-10743975, 3601560, -10503638, 3364279);
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);
            androidMap.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}