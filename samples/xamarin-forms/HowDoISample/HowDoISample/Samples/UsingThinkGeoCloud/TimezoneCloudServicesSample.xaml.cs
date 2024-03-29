﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the TimezoneCloudClient to access the Timezone APIs available from the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimezoneCloudServicesSample : ContentPage
    {
        private TimeZoneCloudClient timeZoneCloudClient;

        public TimezoneCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
           
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a PopupOverlay to display time zone information based on locations input by the user
            var timezoneInfoPopupOverlay = new PopupOverlay();

            // Add the overlay to the map
            mapView.Overlays.Add("Timezone Info Popup Overlay", timezoneInfoPopupOverlay);

            // Add a new InMemoryFeatureLayer to hold the timezone shapes
            var timezonesFeatureLayer = new InMemoryFeatureLayer();

            // Add a style to use to draw the timezone polygons
            timezonesFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            timezonesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Add the layer to an overlay, and add it to the map
            var timezonesLayerOverlay = new LayerOverlay();
            timezonesLayerOverlay.Layers.Add("Timezone Feature Layer", timezonesFeatureLayer);
            mapView.Overlays.Add("Timezone Layer Overlay", timezonesLayerOverlay);

            // Initialize the TimezoneCloudClient with our ThinkGeo Cloud credentials
            timeZoneCloudClient = new TimeZoneCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            // Set the Map Extent
            mapView.CurrentExtent = new RectangleShape(-14269933.09, 6354969.40, -6966221.89, 2759371.58);

            // Get Timezone info for Frisco, TX
            //GetTimeZoneInfo(-10779572.80, 3915268.68);

            await mapView.RefreshAsync();
        }

        private async void mapView_MapTap(object sender, TouchMapViewEventArgs e)
        {
            //Run the timezone info query
            await GetTimeZoneInfo(e.PointInWorldCoordinate.X, e.PointInWorldCoordinate.Y);
        }

        /// <summary>
        ///     Use the TimezoneCloudClient to query for timezone information
        /// </summary>
        private async Task GetTimeZoneInfo(double lon, double lat)
        {
            CloudTimeZoneResult result;
            try
            {
                // Get timezone info based on the lon, lat, and input projection (Spherical Mercator in this case)
                result = await timeZoneCloudClient.GetTimeZoneByCoordinateAsync(lon, lat, 3857);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }
         

            // Get the timezone info popup overlay from the mapview
            var timezoneInfoPopupOverlay = (PopupOverlay) mapView.Overlays["Timezone Info Popup Overlay"];

            // Clear the existing info popups from the map
            timezoneInfoPopupOverlay.Popups.Clear();

            // Build a string description of the timezone
            var timezoneInfoString = new StringBuilder();
            timezoneInfoString.AppendLine($"Time Zone: {result.TimeZone}");
            timezoneInfoString.AppendLine($"Current Local Time: {result.CurrentLocalTime}");
            timezoneInfoString.AppendLine($"Daylight Savings Active: {result.DaylightSavingsActive}");

            // Display the timezone info on a popup on the map
            var popup = new Popup();
            popup.Text = timezoneInfoString.ToString();
            popup.Position = new PointShape(lon, lat);
            timezoneInfoPopupOverlay.Popups.Add(popup);

            // Clear the timezone feature layer of previous features
            var timezonesFeatureLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Timezone Feature Layer");
            timezonesFeatureLayer.Open();
            timezonesFeatureLayer.InternalFeatures.Clear();

            // Use a ProjectionConverter to convert the shape to Spherical Mercator
            var converter = new ProjectionConverter(3857, 4326);
            converter.Open();

            // Add the new timezone polygon to the map
            timezonesFeatureLayer.InternalFeatures.Add(
                new Feature(converter.ConvertToInternalProjection(result.Shape)));
            converter.Close();
            timezonesFeatureLayer.Close();

            // Refresh and redraw the map
            await mapView.RefreshAsync();
        }
    }
}