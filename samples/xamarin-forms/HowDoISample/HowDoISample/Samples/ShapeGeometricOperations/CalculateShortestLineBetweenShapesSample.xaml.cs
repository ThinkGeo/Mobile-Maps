﻿using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to calculate the shortest line between two shapes
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateShortestLineBetweenShapesSample : ContentPage
    {
        public CalculateShortestLineBetweenShapesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the friscoParks, stadiumLayer, and
        ///     shortestLineLayer layers into a grouped LayerOverlay and display it on the map.
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
            var stadiumLayer = new InMemoryFeatureLayer();
            var shortestLineLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project friscoParks layer to Spherical Mercator to match the map projection
            friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoParks layer
            friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
            friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style stadiumLayer
            stadiumLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 16, GeoColors.White, 4);
            stadiumLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style shortestLineLayer
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoParks layer to a LayerOverlay
            layerOverlay.Layers.Add("friscoParks", friscoParks);

            // Add stadiumLayer layer to a LayerOverlay
            layerOverlay.Layers.Add("stadiumLayer", stadiumLayer);

            // Add shortestLineLayer to the layerOverlay
            layerOverlay.Layers.Add("shortestLineLayer", shortestLineLayer);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            // Add Toyota Stadium feature to stadiumLayer
            var stadium = new Feature(new PointShape(-10779651.500992451, 3915933.0023557912));
            stadiumLayer.InternalFeatures.Add(stadium);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Calculates the shortest line from the selected park to the stadium and displays it's length and shows the line on
        ///     the map
        /// </summary>
        private async void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var friscoParks = (ShapeFileFeatureLayer) layerOverlay.Layers["friscoParks"];
            var stadiumLayer = (InMemoryFeatureLayer) layerOverlay.Layers["stadiumLayer"];
            var shortestLineLayer = (InMemoryFeatureLayer) layerOverlay.Layers["shortestLineLayer"];

            // Query the friscoParks layer to get the first feature closest to the map tap event
            var park = friscoParks.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Get the stadium feature from the stadiumLayer
            var stadium = stadiumLayer.InternalFeatures[0];

            // Get the shortest line from the selected park to the stadium
            var shortestLine = park.GetShape().GetShortestLineTo(stadium, GeographyUnit.Meter);

            // Show the shortestLine on the map
            shortestLineLayer.InternalFeatures.Clear();
            shortestLineLayer.InternalFeatures.Add(new Feature(shortestLine));
            await layerOverlay.RefreshAsync();

            // Get the area of the first feature
            var length = shortestLine.GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

            // Display the shortestLine's length in the distanceResult TextBox
            distanceResult.Text = $"{length:f3} km";
        }
    }
}