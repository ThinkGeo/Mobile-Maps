﻿using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     TODO: This sample is a Work In Progress and is disabled in the app!
    ///     Learn how to snap a shape to a nearby shape for precise placement.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnapToShapeSample : ContentPage
    {
        public SnapToShapeSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     WIP
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

            var friscoParks = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));
            var snapLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project friscoParks layer to Spherical Mercator to match the map projection
            friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoParks layer
            friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the splitLayer
            snapLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            snapLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoParks to a LayerOverlay
            layerOverlay.Layers.Add("friscoParks", friscoParks);

            // Add splitLayer to the layerOverlay
            layerOverlay.Layers.Add("snapLayer", snapLayer);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            // Add Toyota Stadium feature to stadiumLayer
            var stadium = new Feature(new PointShape(-10779651.500992451, 3915933.0023557912));
            snapLayer.InternalFeatures.Add(stadium);

            await mapView.RefreshAsync();
        }

        private void SnapToShape_Click(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var friscoParks = (ShapeFileFeatureLayer) layerOverlay.Layers["friscoParks"];
            var snapLayer = (InMemoryFeatureLayer) layerOverlay.Layers["snapLayer"];

            // WIP
        }
    }
}