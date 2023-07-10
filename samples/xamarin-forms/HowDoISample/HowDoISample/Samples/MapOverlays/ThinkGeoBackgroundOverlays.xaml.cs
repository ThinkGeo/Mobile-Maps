using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to improve performance by locally caching map tiles
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThinkGeoBackgroundOverlaysSample : ContentPage
    {
        private ThinkGeoCloudVectorMapsOverlay thinkGeoVectorsOverlay;
        private ThinkGeoCloudRasterMapsOverlay thinkGeoRasterOverlay;

        public ThinkGeoBackgroundOverlaysSample()
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
            thinkGeoVectorsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(thinkGeoVectorsOverlay);

            thinkGeoRasterOverlay = new ThinkGeoCloudRasterMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudRasterMapsMapType.Light_V2_X2);
            thinkGeoRasterOverlay.IsVisible = false;
            thinkGeoRasterOverlay.TileCache = new FileRasterTileCache(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoCloudRasterMaps");
            mapView.Overlays.Add(thinkGeoRasterOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-16061259, 13162974, -5494605, -5338454);
            mapView.RotationEnabled = true;

            await mapView.RefreshAsync();
        }


        private async void Radial_Checked(object sender, CheckedChangedEventArgs e)
        {
            if (thinkGeoVectorsOverlay == null || thinkGeoRasterOverlay == null)
                return;
            if (!e.Value)
                return;

            var radioButton = (RadioButton)sender;
            switch (radioButton.StyleId)
            {
                case "RasterLightV1":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light_V1_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "RasterLightV2":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "RasterDarkV1":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark_V1_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "RasterDarkV2":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark_V2_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "Aerial":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "Hybrid":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid_V2_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "TransparentBg":
                    thinkGeoVectorsOverlay.IsVisible = false;
                    thinkGeoRasterOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.TransparentBackground_V2_X2;
                    await thinkGeoRasterOverlay.RefreshAsync();
                    break;
                case "VectorLight":
                    thinkGeoVectorsOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.IsVisible = false;
                    thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Light;
                    thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
                    await thinkGeoVectorsOverlay.RefreshAsync();
                    break;
                case "VectorDark":
                    thinkGeoVectorsOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.IsVisible = false;
                    thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Dark;
                    thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoDarkBackground");
                    await thinkGeoVectorsOverlay.RefreshAsync();
                    break;
                case "VectorCustom":
                    thinkGeoVectorsOverlay.IsVisible = true;
                    thinkGeoRasterOverlay.IsVisible = false;
                    thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.CustomizedByStyleJson;

                    var jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Json/thinkgeo-world-streets-cobalt.json");
                    thinkGeoVectorsOverlay.StyleJsonUri = new Uri(jsonPath, UriKind.Relative);
                    thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoCustomCobalt");

                    await thinkGeoVectorsOverlay.RefreshAsync();
                    break;
            }
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
        }
    }
}