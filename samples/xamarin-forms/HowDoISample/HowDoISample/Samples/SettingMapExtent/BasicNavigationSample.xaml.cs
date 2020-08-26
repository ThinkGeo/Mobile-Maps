using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasicNavigationSample : ContentPage
    {
        public BasicNavigationSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();


            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);
            
            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            await Task.Delay(5000);
            controlsExpander.IsExpanded = true;
        }

        protected override void on

        /// <summary>
        /// Zoom in on the map
        /// The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the map, or pinching the map.
        /// </summary>
        private void ZoomIn_Click(object sender, EventArgs e)
        {
            //mapView.ZoomIn();
        }

        /// <summary>
        /// Zoom out on the map
        /// The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the map, or pinching the map.
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            //mapView.ZoomOut();
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
        /// Rotate the map at an angle using the value of the rotateAngle Slider. Since this is just setting a property, you must refresh the map in order for the rotation to show.
        /// The same effect can be achieved by holding down the ALT key and left click dragging anywhere on the map.
        /// </summary>
        private void Rotate_Click(object sender, EventArgs e)
        {
            //mapView.RotatedAngle = (float)rotateAngle.Value;
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
