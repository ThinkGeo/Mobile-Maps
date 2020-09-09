using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using System.IO;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    /// Learn how to programmatically zoom, pan, and rotate the map control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasicNavigationSample : ContentPage
    {
        public BasicNavigationSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;            

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);
            
            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }

        /// <summary>
        /// Zoom in on the map
        /// The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the map, or pinching the map.
        /// </summary>
        private void ZoomIn_Click(object sender, EventArgs e)
        {
            mapView.ZoomIn();
        }

        /// <summary>
        /// Zoom out on the map
        /// The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the map, or pinching the map.
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            mapView.ZoomOut();
        }

        /// <summary>
        /// Pan the map in a direction using the PanDirection enum and set how far to pan based on percentage.
        /// The same effect can be achieved by tap dragging anywhere on the map.
        /// </summary>
        private void PanArrow_Click(object sender, EventArgs e)
        {
            var percentage = (int)panPercentage.Value;
            switch (((ImageButton)sender).AutomationId)
            {
                case "panNorth":
                    mapView.Pan(PanDirection.Up, percentage);
                    break;
                case "panEast":
                    mapView.Pan(PanDirection.Right, percentage);
                    break;
                case "panWest":
                    mapView.Pan(PanDirection.Left, percentage);
                    break;
                case "panSouth":
                    mapView.Pan(PanDirection.Down, percentage);
                    break;
            }
            mapView.Refresh();
        }

        /// <summary>
        /// Locks slider values to whole numbers
        /// </summary>
        private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / 1.0);

            ((Slider)sender).Value = newStep * 1.0;
        }
    }
}
