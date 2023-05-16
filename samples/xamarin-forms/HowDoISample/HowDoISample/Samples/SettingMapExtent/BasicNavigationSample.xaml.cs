using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to programmatically zoom, pan, and rotate the map control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasicNavigationSample : ContentPage
    {
        public BasicNavigationSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Zoom in on the map
        ///     The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the
        ///     map, or pinching the map.
        /// </summary>
        private async void ZoomIn_Click(object sender, EventArgs e)
        {
            await mapView.ZoomInAsync();
        }

        /// <summary>
        ///     Zoom out on the map
        ///     The same effect can be achieved by using the ZoomPanBar bar on the upper left of the map, double tapping on the
        ///     map, or pinching the map.
        /// </summary>
        private async void ZoomOut_Click(object sender, EventArgs e)
        {
            await mapView.ZoomOutAsync();
        }

        /// <summary>
        ///     Pan the map in a direction using the PanDirection enum and set how far to pan based on percentage.
        ///     The same effect can be achieved by tap dragging anywhere on the map.
        /// </summary>
        private async void PanArrow_Click(object sender, EventArgs e)
        {
            var percentage = (int) panPercentage.Value;
            switch (((ImageButton) sender).AutomationId)
            {
                case "panNorth":
                    await mapView.PanAsync(PanDirection.Up, percentage);
                    break;
                case "panEast":
                    await mapView.PanAsync(PanDirection.Right, percentage);
                    break;
                case "panWest":
                    await mapView.PanAsync(PanDirection.Left, percentage);
                    break;
                case "panSouth":
                    await mapView.PanAsync(PanDirection.Down, percentage);
                    break;
            }

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Locks slider values to whole numbers
        /// </summary>
        private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / 1.0);

            ((Slider) sender).Value = newStep * 1.0;
        }
    }
}