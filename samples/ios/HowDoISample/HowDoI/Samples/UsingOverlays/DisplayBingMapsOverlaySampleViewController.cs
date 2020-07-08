using System;
using System.Drawing;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public partial class DisplayBingMapsOverlaySampleViewController : BaseController
    {
        UITextField txtApiKey;
        MapView mapView;

        public DisplayBingMapsOverlaySampleViewController() : base("DisplayBingMapsOverlaySampleViewController", null)
        {
            Title = "";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            mapView = new MapView(View.Frame);
            mapView.MapTools.ZoomMapTool.IsEnabled = false;
            // Set the Map Unit to Meter, the Shapefile’s unit of measure.
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = MaxExtents.BingMaps;
            View.Add(mapView);

            // Add the control panel to allow user add the api key for bing map
            AddControlPanel();
        }

        private void ApplyApiKeyForBingMap()
        {
            // Clear overlays
            mapView.Overlays.Clear();
            // Create BingMapsOverlay with the application id
            BingMapsOverlay bingMapsOverlay = new BingMapsOverlay(txtApiKey.Text);
            mapView.Overlays.Add("BingMapsOverlay", bingMapsOverlay);

            // Set a proper extent for the map. The extent is the geographical area you want it to display.
            mapView.CurrentExtent = new RectangleShape(-11917925, 6094804, -3300683, 370987);

            // We now need to call the Refresh() method of the Map view so that the Map can redraw based on the data that has been provided.
            mapView.Refresh();
        }

        private void AddControlPanel()
        {
            // Control panel view 
            var controlPanelView = new UIView();
            controlPanelView.ViewForBaselineLayout.Frame = new CoreGraphics.CGRect(View.Frame.Width - 200, 200, 200, 60);
            controlPanelView.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.8f);
            controlPanelView.Layer.BorderWidth = 1;
            controlPanelView.Layer.BorderColor = UIColor.Gray.CGColor;
            controlPanelView.Layer.CornerRadius = 4;
            this.View.AddSubview(controlPanelView);

            // Add label
            var lblApiKey = new UILabel();
            lblApiKey.Text = "API key:";
            lblApiKey.Font = UIFont.SystemFontOfSize(12);
            lblApiKey.BackgroundColor = UIColor.White;
            lblApiKey.Frame = new Rectangle(10, 10, 50, 20);
            controlPanelView.Add(lblApiKey);

            // Add apply button
            UIButton btnApply = new UIButton(UIButtonType.System);
            btnApply.SetTitle("Apply", UIControlState.Normal);
            btnApply.Font = UIFont.SystemFontOfSize(12);
            btnApply.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnApply.BackgroundColor = UIColor.FromRGBA(0, 0, 255, 180);
            btnApply.Layer.BorderWidth = 1;
            btnApply.Layer.BorderColor = UIColor.Gray.CGColor;
            btnApply.Layer.CornerRadius = 3;
            btnApply.Frame = new Rectangle((int)(controlPanelView.Frame.Width - 40), 35, 40, 20);
            btnApply.TouchUpInside += (sender, e) => {
                ApplyApiKeyForBingMap();
            };
            controlPanelView.Add(btnApply);

            // Control panel
            txtApiKey = new UITextField();
            txtApiKey.Frame = new Rectangle(60, 10, 150, 20);
            txtApiKey.BackgroundColor = UIColor.White;
            txtApiKey.Text = "AqdOeptF4BISqQZztbLziEv80WCPR943wA802vfb8mXH4aFono-DVuuaORxY9-7v";
            txtApiKey.Font = UIFont.SystemFontOfSize(12);
            controlPanelView.Add(txtApiKey);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

