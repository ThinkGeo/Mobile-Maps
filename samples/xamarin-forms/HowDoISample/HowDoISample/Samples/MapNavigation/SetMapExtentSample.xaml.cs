﻿using System;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to set the map extent using a variety of different methods.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetMapExtentSample : ContentPage
    {
        private ShapeFileFeatureLayer friscoCityBoundary;

        public SetMapExtentSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map and a shapefile with simple data to work
        ///     with
        /// </summary>
        protected override void OnAppearing()
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

            // Load the Frisco data to a layer
            friscoCityBoundary = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Subdivisions.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            friscoCityBoundary.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style the data so that we can see it on the map
            friscoCityBoundary.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(16, GeoColors.Blue), GeoColors.DimGray, 2);
            friscoCityBoundary.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add Frisco data to a LayerOverlay and add it to the map
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(friscoCityBoundary);
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }

        /// <summary>
        ///     Zoom to a scale programmatically. Note that the scales are bound by a ZoomLevelSet.
        /// </summary>
        private async void ZoomToScale_Click(object sender, EventArgs e)
        {
            await mapView.ZoomToScaleAsync(10000);
        }

        /// <summary>
        ///     Set the map extent to fix a layer's bounding box
        /// </summary>
        private async void LayerBoundingBox_Click(object sender, EventArgs e)
        {
            mapView.CurrentExtent = friscoCityBoundary.GetBoundingBox();
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Set the map extent to fix a feature's bounding box
        /// </summary>
        private async void FeatureBoundingBox_Click(object sender, EventArgs e)
        {
            var feature = friscoCityBoundary.FeatureSource.GetFeatureById("1", ReturningColumnsType.NoColumns);
            mapView.CurrentExtent = feature.GetBoundingBox();
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Zoom to a lat/lon at a desired scale by converting the lat/lon to match the map's projection
        /// </summary>
        private async void ZoomToLatLon_Click(object sender, EventArgs e)
        {
            // Create a PointShape from the lat-lon
            var latlon = new PointShape(-96.82, 33.15);

            // Convert the lat-lon point to Spherical Mercator
            var projectionConverter = new ProjectionConverter(4326, 3857);
            projectionConverter.Open();
            var poingInSphericalMercator = (PointShape) projectionConverter.ConvertToExternalProjection(latlon);
            projectionConverter.Close();

            // Zoom to the converted lat-lon at the desired scale
            await mapView.ZoomToAsync(poingInSphericalMercator);
        }
    }
}