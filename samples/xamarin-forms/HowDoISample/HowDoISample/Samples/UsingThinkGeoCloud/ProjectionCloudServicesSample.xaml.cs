﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the ProjectionCloudClient to access the Projection APIs available from the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectionCloudServicesSample : ContentPage
    {
        private ProjectionCloudClient projectionCloudClient;

        public ProjectionCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer for the reprojected features
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

            // Create a new feature layer to display the shapes we will be reprojecting
            var reprojectedFeaturesLayer = new InMemoryFeatureLayer();

            // Add a point, line, and polygon style to the layer. These styles control how the shapes will be drawn
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple);
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 6, false);
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Apply these styles on all zoom levels. This ensures our shapes will be visible on all zoom levels
            reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to an overlay
            var reprojectedFeaturesOverlay = new LayerOverlay();
            reprojectedFeaturesOverlay.Layers.Add("Reprojected Features Layer", reprojectedFeaturesLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("Reprojected Features Overlay", reprojectedFeaturesOverlay);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10779897.2471527, 3915795.035256, -10779253.4730711, 3914680.94844247);

            // Initialize the ProjectionCloudClient with our ThinkGeo Cloud credentials
            projectionCloudClient = new ProjectionCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            await mapView.RefreshAsync();
        }


        /// <summary>
        ///     Draw reprojected features on the map
        /// </summary>
        private async Task ClearMapAndAddFeatures(Collection<Feature> features)
        {
            // Get the layer we prepared from the MapView
            var reprojectedFeatureLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Reprojected Features Layer");

            // Clear old features from the feature layer and add the newly reprojected features
            reprojectedFeatureLayer.InternalFeatures.Clear();

            foreach (var sphericalMercatorFeature in features)
                reprojectedFeatureLayer.InternalFeatures.Add(sphericalMercatorFeature);

            // Set the map extent to zoom into the feature and refresh the map
            reprojectedFeatureLayer.Open();
            mapView.CurrentExtent = reprojectedFeatureLayer.GetBoundingBox();

            var standardZoomLevelSet = new ZoomLevelSet();
            await mapView.ZoomToScaleAsync(standardZoomLevelSet.ZoomLevel18.Scale);
        }

        /// <summary>
        ///     Use the ProjectionCloudClient to reproject multiple different features
        /// </summary>
        private async void ReprojectMultipleFeatures_Click(object sender, EventArgs e)
        {
            // Create features based on the WKT in the textbox in the UI
            var decimalDegreeFeatures = new Collection<Feature>();
            var wktStrings = new Collection<string>();
            wktStrings.Add("POINT(-96.834516 33.150083)");
            wktStrings.Add("LINESTRING(-96.83559 33.149,-96.835866046134 33.1508413556856,-96.835793626491 33.1508974965687,-96.8336008970734 33.1511063402186,-96.83356 33.15109,-96.83328 33.14922)");
            wktStrings.Add("POLYGON((-96.83582 33.1508,-96.83578 33.15046,-96.83353 33.15068,-96.83358 33.15102,-96.83582 33.1508))");

            foreach (var wktString in wktStrings)
                try
                {
                    var wktFeature = new Feature(wktString);
                    decimalDegreeFeatures.Add(wktFeature);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }

            // Use the ProjectionCloudClient to convert between Decimal Degrees (4326) and Spherical Mercator (3857)
            var sphericalMercatorFeatures = await projectionCloudClient.ProjectAsync(decimalDegreeFeatures, 4326, 3857);
            
            // Add the reprojected features to the map
            await ClearMapAndAddFeatures(sphericalMercatorFeatures);
        }
    }
}