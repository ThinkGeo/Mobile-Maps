﻿using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to union shapes into a single shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnionShapesSample : ContentPage
    {
        public UnionShapesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the dividedCityLimits and unionLayer layers into a
        ///     grouped LayerOverlay and display them on the map.
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

            var dividedCityLimits = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/FriscoCityLimitsDivided.shp"));
            var unionLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Project dividedCityLimits layer to Spherical Mercator to match the map projection
            dividedCityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style dividedCityLimits layer
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(128, GeoColors.LightOrange), GeoColors.DimGray, 2);
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("NAME",
                "Segoe UI", 12, DrawingFontStyles.Bold, GeoColors.Black, GeoColors.White, 2);
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style unionLayer
            unionLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray, 2);
            unionLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add dividedCityLimits to a LayerOverlay
            layerOverlay.Layers.Add("dividedCityLimits", dividedCityLimits);

            // Add unionLayer to the layerOverlay
            layerOverlay.Layers.Add("unionLayer", unionLayer);

            // Set the map extent to the dividedCityLimits layer bounding box
            dividedCityLimits.Open();
            mapView.CurrentExtent = dividedCityLimits.GetBoundingBox();
            dividedCityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Unions all the features in the dividedCityLimits layer and displays the results on the map
        /// </summary>
        private async void UnionShapes_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var dividedCityLimits = (ShapeFileFeatureLayer) layerOverlay.Layers["dividedCityLimits"];
            var unionLayer = (InMemoryFeatureLayer) layerOverlay.Layers["unionLayer"];

            // Query the dividedCityLimits layer to get all the features
            dividedCityLimits.Open();
            var features = dividedCityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            dividedCityLimits.Close();

            // Union all the features into a single Multipolygon Shape
            var union = AreaBaseShape.Union(features);

            // Add the union shape into unionLayer to display the result.
            // If this were to be a permanent change to the dividedCityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            unionLayer.InternalFeatures.Clear();
            unionLayer.InternalFeatures.Add(new Feature(union));

            // Hide the dividedCityLimits layer
            dividedCityLimits.IsVisible = false;

            // Redraw the layerOverlay to see the unioned features on the map
            await layerOverlay.RefreshAsync();
        }
    }
}