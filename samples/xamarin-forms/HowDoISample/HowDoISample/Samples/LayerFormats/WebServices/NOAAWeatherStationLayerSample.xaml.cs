using System;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a NOAA Weather Station Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NOAAWeatherStationLayerSample : ContentPage
    {
        public NOAAWeatherStationLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the NOAA Weather Station layer to the map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create background world map with vector tile requested from ThinkGeo Cloud Service.
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            var weatherOverlay = new LayerOverlay();
            weatherOverlay.TileType = TileType.SingleTile;
            mapView.Overlays.Add("Weather", weatherOverlay);

            // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
            var noaaWeatherStationLayer = new NoaaWeatherStationFeatureLayer();
            noaaWeatherStationLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            // Add the new layer to the overlay we created earlier
            weatherOverlay.Layers.Add("Noaa Weather Stations", noaaWeatherStationLayer);
                        
            // Create the weather stations style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
            noaaWeatherStationLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherStationStyle());
            noaaWeatherStationLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the extent to a view of the US
            mapView.CurrentExtent =
                new RectangleShape(-14927495.374917, 8262593.0543992, -6686622.84891633, 1827556.23117885);

            // Refresh the map.
            await mapView.RefreshAsync();
        }
    }
}