using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public partial class UseBingMapsOverlayViewController : BaseController
    {
        string bingMapApplicationId = "AqdOeptF4BISqQZztbLziEv80WCPR943wA802vfb8mXH4aFono-DVuuaORxY9-7v";
        public UseBingMapsOverlayViewController() : base("UseBingMapsOverlayViewController", null)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            MapView mapView = new MapView(View.Frame);
            mapView.MapTools.ZoomMapTool.IsEnabled = false;
            View.Add(mapView);
            
            // Set the Map Unit to Meter, the Shapefile’s unit of measure.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create BingMapsOverlay with the application id
            BingMapsOverlay bingMapsOverlay = new BingMapsOverlay(bingMapApplicationId);
            mapView.Overlays.Add("BingMapsOverlay", bingMapsOverlay);

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

