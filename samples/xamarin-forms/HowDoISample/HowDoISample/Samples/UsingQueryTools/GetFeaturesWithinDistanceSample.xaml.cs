﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use layer query tools to find features in a layer within a given distance of a point
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetFeaturesWithinDistanceSample : ContentPage
    {
        public GetFeaturesWithinDistanceSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco zoning data
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");

            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a feature layer to hold the Frisco zoning data
            var zoningLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Create a layer to hold features found by the spatial query
            var highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            layerOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);


            // Add a MarkerOverlay to the map to display the selected point for the query
            var queryFeatureMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("Query Feature Marker Overlay", queryFeatureMarkerOverlay);

            mapView.Overlays.Add("Layer Overlay", layerOverlay);

            // Add a sample point to the map for the initial query
            var sampleShape = new PointShape(-10779425.2690712, 3914970.73561765);
            GetFeaturesWithinDistance(sampleShape);

            // Set the map extent to the initial area
            mapView.CurrentExtent =
                new RectangleShape(-10781338.5834248, 3916678.62545891, -10777511.9547176, 3913262.84577639);

            mapView.Refresh();
        }


        /// <summary>
        ///     Perform the 'Get Features Within Distance' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            var features = new Collection<Feature>();

            // Perform the spatial query on features in the specified layer
            layer.Open();
            features = layer.QueryTools.GetFeaturesWithinDistanceOf(shape, GeographyUnit.Meter, DistanceUnit.Meter,
                (int) searchRadius.Value, ReturningColumnsType.NoColumns);
            layer.Close();

            return features;
        }

        /// <summary>
        ///     Highlight the features that were found by the spatial query
        /// </summary>
        private void HighlightQueriedFeatures(IEnumerable<Feature> features)
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
            layerOverlay.Refresh();

            // Update the number of matching features found in the UI
            txtNumberOfFeaturesFound.Text =
                $"Number of features within distance of the drawn shape: {features.Count()}";
        }

        private void GetFeaturesWithinDistance(PointShape point)
        {
            // Find the layers we will be modifying in the MapView
            var queryFeatureMarkerOverlay = (SimpleMarkerOverlay) mapView.Overlays["Query Feature Marker Overlay"];
            var zoningLayer = (ShapeFileFeatureLayer) mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query point marker overlaylayer and add a marker on the newly drawn point
            queryFeatureMarkerOverlay.Markers.Clear();
            queryFeatureMarkerOverlay.Markers.Add(CreateNewMarker(point));
            queryFeatureMarkerOverlay.Refresh();

            // Perform the spatial query using the drawn point and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(point, zoningLayer);
            HighlightQueriedFeatures(queriedFeatures);

            // Disable map drawing and clear the drawn point
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        ///     Perform the spatial query when a new point is drawn
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            GetFeaturesWithinDistance(e.PointInWorldCoordinate);
        }

        /// <summary>
        ///     Create a new map marker using preloaded image assets
        /// </summary>
        private Marker CreateNewMarker(PointShape point)
        {
            return new Marker
            {
                Position = point,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/AQUA.png"),
                YOffset = -17
            };
        }
    }
}