using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to add, edit, or remove markers on the map using the MarkerOverlay.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsingMarkersSample : ContentPage
    {
        public UsingMarkersSample()
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
            mapView.CurrentExtent =
                new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            AddSimpleMarkers();

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Add a SimpleMarkerOverlay to the map
        /// </summary>
        private void AddSimpleMarkers()
        {
            var simpleMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);
        }

        /// <summary>
        ///     Adds a marker to the simpleMarkerOverlay where the map tap event occurred.
        /// </summary>
        private async void MapView_OnMapTouch(object sender, TouchMapViewEventArgs e)
        {
            var simpleMarkerOverlay = (SimpleMarkerOverlay) mapView.Overlays["simpleMarkerOverlay"];

            // Create a marker at the position the mouse was tapped
            var marker = new Marker
            {
                Position = e.PointInWorldCoordinate,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/AQUA.png"),
                YOffset = -17
            };

            // Add the marker to the simpleMarkerOverlay and refresh the map
            simpleMarkerOverlay.Markers.Add(marker);
            await simpleMarkerOverlay.RefreshAsync();
        }
    }
}