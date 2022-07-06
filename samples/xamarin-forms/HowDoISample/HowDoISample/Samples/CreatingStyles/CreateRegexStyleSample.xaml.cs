﻿using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a CloudMapsVector Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRegexStyleSample : ContentPage
    {
        public CreateRegexStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~",
                "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            var coyoteSightings = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco_Coyote_Sightings.shp"));

            // Project the layer's data to match the projection of the map
            coyoteSightings.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to a layer overlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("coyoteSightings", coyoteSightings);

            // Add the overlay to the map
            mapView.Overlays.Add(layerOverlay);

            var regexStyle = new RegexStyle();
            regexStyle.ColumnName = "Comments";

            var largeItem = new RegexItem("big|large|huge", new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red));
            regexStyle.RegexItems.Add(largeItem);

            var allSightingsStyle = new PointStyle(PointSymbolType.Circle, 5, GeoBrushes.Green);

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            coyoteSightings.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(allSightingsStyle);
            coyoteSightings.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(regexStyle);

            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map.
            coyoteSightings.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10781794.4716492, 3917077.66579861, -10775416.8466492, 3913528.63559028);
        }
    }
}