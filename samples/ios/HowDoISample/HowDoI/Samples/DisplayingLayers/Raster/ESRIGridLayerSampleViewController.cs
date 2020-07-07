using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public partial class ESRIGridLayerSampleViewController : BaseController
    {
        public ESRIGridLayerSampleViewController() : base("ESRIGridLayerSampleViewController", null)
        {
            Title = "";
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
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            // Add a ThinkGeoCloudMapsOverlay.
            mapView.Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);

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

