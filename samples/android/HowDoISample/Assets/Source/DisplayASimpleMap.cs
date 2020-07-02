using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class DisplayASimpleMap : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.CurrentExtent = new RectangleShape(-2980558, 4655441, 2274028, -4520025);

            ThinkGeoCloudRasterMapsOverlay worldMapKitDesktopOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            androidMap.Overlays.Add(worldMapKitDesktopOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo);
        }
    }
}