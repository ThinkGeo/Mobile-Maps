using System;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render coordinate info based on the center position of the map.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapToolsSample : ContentPage
    {
        public DisplayMapToolsSample()
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

            // Enable the CenterCoordinate map tool
            DisplayCoordinate = true;
            mapView.MapTools.CenterCoordinate.DisplayProjection = new ProjectionConverter(3857, 4326);
            mapView.MapTools.CenterCoordinate.DisplayTextFormat = "Lat: {0:N4}, Lon: {1:N4}";

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            await mapView.RefreshAsync();
        }

        public bool DisplayCoordinate
        {
            get => mapView.MapTools.CenterCoordinate.IsEnabled;
            set
            {
                if (mapView.MapTools.CenterCoordinate.IsEnabled == value) return;
                mapView.MapTools.CenterCoordinate.IsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool DisplayZoomBar
        {
            get => mapView.MapTools.ZoomMapTool.IsEnabled;
            set
            {
                if (mapView.MapTools.ZoomMapTool.IsEnabled == value) return;
                mapView.MapTools.ZoomMapTool.IsEnabled = value;
                OnPropertyChanged();
            }
        }

    }
}