using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DisplayASimpleMap : BaseViewController
    {
        public DisplayASimpleMap()
            : base()
        { }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            
            // Perform any additional setup after loading the view, typically from a nib.
            // Creat a new MapView, which is the canvas of the map, and add it to the View.
            MapView mapView = new MapView(View.Frame);
            View.AddSubview(mapView);

            // Set the Map Unit to Meter and set the map's current extent to North America.
            mapView.MapUnit = GeographyUnit.Meter;
            //mapView.CurrentExtent = new RectangleShape(-13939426, 6701997, -7812401, 2626987);
            mapView.CurrentExtent = MaxExtents.ThinkGeoMaps;

            // Create a new ThinkGeoCloud Overlay using Client ID / Client Secret, and add it the overlay to MapView.
            string clientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string secret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(clientKey, secret);
            mapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            mapView.RotationEnabled = true;
            //mapView.IsInertiallyPanEnabled = true;


            await mapView.RefreshAsync();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}