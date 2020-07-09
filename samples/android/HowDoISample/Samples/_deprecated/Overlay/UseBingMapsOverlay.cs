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


            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new BingMapsZoomLevelSet();
            mapView.Overlays.Add("BingMapOverlay", bingMapOverlay);
            mapView.CurrentExtent = bingMapOverlay.GetBoundingBox();

            CheckBingMapApplicationId();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        private void CheckBingMapApplicationId()
        {
            BingMapsOverlay bingMapOverlay = (BingMapsOverlay)mapView.Overlays["BingMapOverlay"];
            if (bingMapOverlay.ApplicationId.Equals("Your Application Id"))
            {
                mapView.Overlays.Clear();

                ImageView imageView = new ImageView(this.Context);
                imageView.SetImageResource(Resource.Drawable.Notice);
                imageView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

                RelativeLayout mainLayout = currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout);
                mainLayout.RemoveView(mapView);
                mainLayout.AddView(imageView);
            }
        }
    }
}