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

            // Create a new ThinkGeo vector tile overlay using the apiKey, and add the overlay to MapView.
            var thinkGeoVectorTileOverlay = new ThinkGeoVectorTileOverlay(SampleHelper.ThinkGeoApiKey);
            mapView.Overlays.Add(thinkGeoVectorTileOverlay);

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