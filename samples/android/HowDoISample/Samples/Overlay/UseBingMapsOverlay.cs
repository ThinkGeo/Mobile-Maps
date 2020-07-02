using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class UseBingMapsOverlay : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            BingMapsOverlay bingMapOverlay = new BingMapsOverlay();
            // Please set your own information about those parameters below.
            bingMapOverlay.ApplicationId = "Your Application Id";
            bingMapOverlay.ApplicationId = "Amg9BxyuF81NEyxKm2ESMaoL03MTvaYBV3KOfxpHeXDsEt38DVwK4-SPFPg6qcBp";
            bingMapOverlay.MapType = BingMapsMapType.AerialWithLabels;


            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.ZoomLevelSet = new BingMapsZoomLevelSet();
            androidMap.Overlays.Add("BingMapOverlay", bingMapOverlay);
            androidMap.CurrentExtent = bingMapOverlay.GetBoundingBox();

            CheckBingMapApplicationId();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        private void CheckBingMapApplicationId()
        {
            BingMapsOverlay bingMapOverlay = (BingMapsOverlay)androidMap.Overlays["BingMapOverlay"];
            if (bingMapOverlay.ApplicationId.Equals("Your Application Id"))
            {
                androidMap.Overlays.Clear();

                ImageView imageView = new ImageView(this.Context);
                imageView.SetImageResource(Resource.Drawable.Notice);
                imageView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

                RelativeLayout mainLayout = currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout);
                mainLayout.RemoveView(androidMap);
                mainLayout.AddView(imageView);
            }
        }
    }
}