using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class UseGoogleMapsOverlay : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new GoogleMapsZoomLevelSet();

            GoogleMapsOverlay googleMapsOverlay = new GoogleMapsOverlay();
            googleMapsOverlay.ClientId = "Your Client Id";
            googleMapsOverlay.PrivateKey = "Your Private Key";
            googleMapsOverlay.ClientId = "gme-valmontindustries";
            googleMapsOverlay.PrivateKey = "-J7YzyRJpepGq9HwRM3LGXyDM04=";
            googleMapsOverlay.TileType = TileType.MultiTile;
            mapView.Overlays.Add(googleMapsOverlay);

            ProjectionConverter proj4 = new ProjectionConverter();
            proj4.InternalProjection = new Projection(Projection.GetDecimalDegreesProjString());
            proj4.ExternalProjection = new Projection(Projection.GetGoogleMapProjString());
            proj4.Open();

            mapView.CurrentExtent = proj4.ConvertToExternalProjection(new RectangleShape(-139.2, 92.4, 120.9, -93.2)) as RectangleShape;

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}