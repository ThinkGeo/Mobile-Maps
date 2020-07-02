using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public partial class UseGoogleMapsOverlayViewController : BaseController
    {
        string googleMapClientId = "gme-valmontindustries";
        string googleMapPrivateKey = "-J7YzyRJpepGq9HwRM3LGXyDM04=";
        public UseGoogleMapsOverlayViewController() : base("UseGoogleMapsOverlayViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            MapView mapView = new MapView(View.Frame);
            View.Add(mapView);

            //var margins = View.LayoutMarginsGuide;
            //mapView.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor);
            //mapView.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor);
            //mapView.TopAnchor.ConstraintEqualTo(margins.TopAnchor);
            //mapView.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor);

            // Set the Map Unit to Meter, the Shapefile’s unit of measure.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create BingMapsOverlay with the application id
            GoogleMapsOverlay googleMapsOverlay = new GoogleMapsOverlay(googleMapClientId, googleMapPrivateKey);
            mapView.Overlays.Add("googleMapsOverlay", googleMapsOverlay);

            // Set a proper extent for the map. The extent is the geographical area you want it to display.
            mapView.CurrentExtent = new RectangleShape(-11917925, 6094804, -3300683, 370987);

            // We now need to call the Refresh() method of the Map view so that the Map can redraw based on the data that has been provided.
            mapView.Refresh();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

