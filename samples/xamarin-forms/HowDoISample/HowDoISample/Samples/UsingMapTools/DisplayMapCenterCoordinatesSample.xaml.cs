using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render coordinate info based on the center position of the map.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapCenterCoordinatesSample : ContentPage
    {
        public DisplayMapCenterCoordinatesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Enable the CenterCoordinate map tool
            mapView.MapTools.CenterCoordinate.IsEnabled = true;
            coordinateType.SelectedItem = "(x), (y)";

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }


        /// <summary>
        ///     Toggle the visibility of the CenterCoordinate
        /// </summary>
        private void DisplayMouseCoordinates_Checked(object sender, EventArgs e)
        {
            mapView.MapTools.CenterCoordinate.IsEnabled = displayMouseCoordinates.IsChecked;
        }

        /// <summary>
        ///     Changes the display format of the CenterCoordinates based on ComboBox selection
        /// </summary>
        private void CoordinateType_SelectionChanged(object sender, EventArgs e)
        {
            switch ((string) coordinateType.SelectedItem)
            {
                case "(x), (y)":
                    // Set to X, Y format
                    mapView.MapTools.CenterCoordinate.DisplayProjection = new ProjectionConverter(3857, 3857);
                    mapView.MapTools.CenterCoordinate.DisplayTextFormat = "X: {0:N}, Y: {1:N}";
                    break;
                case "(lat), (lon)":
                    // Set to Lat, Lon format
                    mapView.MapTools.CenterCoordinate.DisplayProjection = new ProjectionConverter(3857, 4326);
                    mapView.MapTools.CenterCoordinate.DisplayTextFormat = "Lat: {0:N4}, Lon: {1:N4}";
                    break;
            }

            mapView.Refresh();
        }
    }
}