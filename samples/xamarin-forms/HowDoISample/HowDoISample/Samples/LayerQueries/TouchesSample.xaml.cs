﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find which features in a layer are touching a shape
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TouchesSample : ContentPage
    {
        public TouchesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco zoning data
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

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a feature layer to hold and display the zoning data
            var zoningLayer = new InMemoryFeatureLayer();

            // Add a style to use to draw the Frisco zoning polygons
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Import the features from the Frisco zoning data shapefile
            var zoningDataFeatureSource = new ShapeFileFeatureSource(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Zoning.shp"));

            // Create a ProjectionConverter to convert the shapefile data from North Central Texas (2276) to Spherical Mercator (3857)
            var projectionConverter = new ProjectionConverter(3857, 2276);

            // For this sample, we have to reproject the features before adding them to the feature layer
            // This is because the topological equality query often does not work when used on a feature layer with a ProjectionConverter, due to rounding issues between projections
            zoningDataFeatureSource.Open();
            projectionConverter.Open();
            foreach (var zoningFeature in zoningDataFeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns))
            {
                var reprojectedFeature = projectionConverter.ConvertToInternalProjection(zoningFeature);
                zoningLayer.InternalFeatures.Add(reprojectedFeature);
            }

            zoningDataFeatureSource.Close();
            projectionConverter.Close();

            // Set the map extent to Frisco, TX
            //mapView.CurrentExtent = new RectangleShape(-10781137.28, 3917162.59, -10774579.34, 3911241.35);

            // Create a layer to hold the feature we will perform the spatial query against
            var queryFeatureLayer = new InMemoryFeatureLayer();
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a layer to hold features found by the spatial query
            var highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            var layerOverlay = new LayerOverlay();

            layerOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            layerOverlay.Layers.Add("Query Feature", queryFeatureLayer);
            layerOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);

            mapView.Overlays.Add("Layer Overlay", layerOverlay);

            // Create a sample shape using vertices from an existing feature, to ensure that it is touching other features
            zoningLayer.Open();
            var firstFeatureShape = (MultipolygonShape) zoningLayer.FeatureSource
                .GetAllFeatures(ReturningColumnsType.NoColumns).First().GetShape();

            // Get vertices from an existing feature
            var vertices = firstFeatureShape.Polygons.First().OuterRing.Vertices;

            // Create a new feature using a subset of those vertices
            var sampleShape = new MultipolygonShape(new Collection<PolygonShape>
                {new PolygonShape(new RingShape(new Collection<Vertex> {vertices[0], vertices[1], vertices[2]}))});
            queryFeatureLayer.InternalFeatures.Add(new Feature(sampleShape));
            zoningLayer.Close();
            await GetFeaturesTouching(sampleShape);

            // Set the map extent to the sample shape
            mapView.CurrentExtent =
                new RectangleShape(-10778499.3056056, 3920951.91647677, -10774534.1347853, 3917536.13679426);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Perform the 'Touches' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            // Perform the spatial query on features in the specified layer
            layer.Open();
            var features = layer.QueryTools.GetFeaturesTouching(shape, ReturningColumnsType.NoColumns);
            layer.Close();

            return features;
        }

        /// <summary>
        ///     Highlight the features that were found by the spatial query
        /// </summary>
        private async Task HighlightQueriedFeatures(IEnumerable<Feature> features)
        {
            // Find the layers we will be modifying in the MapView dictionary
            var layerOverlay = (LayerOverlay) mapView.Overlays["Layer Overlay"];
            var highlightedFeaturesLayer = (InMemoryFeatureLayer) layerOverlay.Layers["Highlighted Features"];

            // Clear the currently highlighted features
            highlightedFeaturesLayer.Open();
            highlightedFeaturesLayer.InternalFeatures.Clear();

            // Add new features to the layer
            foreach (var feature in features) highlightedFeaturesLayer.InternalFeatures.Add(feature);
            highlightedFeaturesLayer.Close();

            // Refresh the overlay so the layer is redrawn
            await layerOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Perform the spatial query and draw the shapes on the map
        /// </summary>
        private async Task GetFeaturesTouching(BaseShape shape)
        {
            // Find the layers we will be modifying in the MapView
            var layerOverlay = (LayerOverlay) mapView.Overlays["Layer Overlay"];
            var queryFeatureLayer = (InMemoryFeatureLayer) layerOverlay.Layers["Query Feature"];
            var zoningLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query shape layer and add the newly drawn shape
            queryFeatureLayer.InternalFeatures.Clear();
            queryFeatureLayer.InternalFeatures.Add(new Feature(shape));
            await layerOverlay.RefreshAsync();

            // Perform the spatial query using the drawn shape and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(shape, zoningLayer);
            await HighlightQueriedFeatures(queriedFeatures);

            // Disable map drawing and clear the drawn shape
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }
    }
}