using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class UseGoogleMapsOverlay : BaseViewController
    {
        public UseGoogleMapsOverlay()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new SphericalMercatorZoomLevelSet();

            GoogleMapsOverlay googleMapsOverlay = new GoogleMapsOverlay();
            googleMapsOverlay.ClientId = "Your Client Id";
            googleMapsOverlay.PrivateKey = "Your Private Key";
            googleMapsOverlay.ClientId = "gme-valmontindustries";
            googleMapsOverlay.PrivateKey = "-J7YzyRJpepGq9HwRM3LGXyDM04=";
            MapView.Overlays.Add(googleMapsOverlay);

            ProjectionConverter proj4 = new ProjectionConverter();
            proj4.ExternalProjection = new Projection(Projection.GetDecimalDegreesProjString());
            proj4.InternalProjection = new Projection(Projection.GetGoogleMapProjString());
            proj4.Open();

            MapView.CurrentExtent = proj4.ConvertToExternalProjection(new RectangleShape(-139.2, 92.4, 120.9, -93.2));

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}