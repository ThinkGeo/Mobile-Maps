﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OverlapsSample : ContentPage
    {
        public OverlapsSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create a feature layer to hold the Frisco zoning data
            ShapeFileFeatureLayer zoningLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Zoning.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            ProjectionConverter projectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco zoning polygons
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);

            // Set the map extent to Frisco, TX
           // mapView.CurrentExtent = new RectangleShape(-10781137.28, 3917162.59, -10774579.34, 3911241.35);

            // Create a layer to hold the feature we will perform the spatial query against
            InMemoryFeatureLayer queryFeatureLayer = new InMemoryFeatureLayer();
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(75, GeoColors.LightRed), GeoColors.LightRed);
            queryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a layer to hold features found by the spatial query
            InMemoryFeatureLayer highlightedFeaturesLayer = new InMemoryFeatureLayer();
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.MidnightBlue), GeoColors.MidnightBlue);
            highlightedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add each feature layer to it's own overlay
            // We do this so we can control and refresh/redraw each layer individually
            LayerOverlay zoningOverlay = new LayerOverlay();
            zoningOverlay.Layers.Add("Frisco Zoning", zoningLayer);
            mapView.Overlays.Add("Frisco Zoning Overlay", zoningOverlay);

            LayerOverlay queryFeaturesOverlay = new LayerOverlay();
            queryFeaturesOverlay.Layers.Add("Query Feature", queryFeatureLayer);
            mapView.Overlays.Add("Query Features Overlay", queryFeaturesOverlay);

            LayerOverlay highlightedFeaturesOverlay = new LayerOverlay();
            highlightedFeaturesOverlay.Layers.Add("Highlighted Features", highlightedFeaturesLayer);
            mapView.Overlays.Add("Highlighted Features Overlay", highlightedFeaturesOverlay);

            // Add an event to handle new shapes that are drawn on the map
            mapView.TrackOverlay.TrackEnded += OnPolygonDrawn;

            // Add a sample shape to the map for the initial query
            PolygonShape sampleShape = new PolygonShape("POLYGON((-10779549.4792414 3915352.92061116,-10777495.2341177 3915859.31592073,-10776214.913901 3914827.41589883,-10776081.1491022 3913384.66699796,-10777906.0831424 3912553.41431997,-10779702.3532971 3914110.81876263,-10779549.4792414 3915352.92061116))");
            GetFeaturesOverlaps(sampleShape);

            // Set the map extent to the sample shapes
            mapView.CurrentExtent = sampleShape.GetBoundingBox();
            //mapView.ZoomOut();
            mapView.Refresh();
        }

        /// <summary>
        /// Perform the 'Overlaps' spatial query using the layer's QueryTools
        /// </summary>
        private Collection<Feature> PerformSpatialQuery(BaseShape shape, FeatureLayer layer)
        {
            // Perform the spatial query on features in the specified layer
            layer.Open();
            var features = layer.QueryTools.GetFeaturesOverlapping(shape, ReturningColumnsType.AllColumns);
            layer.Close();

            return features;
        }

        /// <summary>
        /// Highlight the features that were found by the spatial query
        /// </summary>
        private void HighlightQueriedFeatures(IEnumerable<Feature> features)
        {
            // Find the layers we will be modifying in the MapView dictionary
            LayerOverlay highlightedFeaturesOverlay = (LayerOverlay)mapView.Overlays["Highlighted Features Overlay"];
            InMemoryFeatureLayer highlightedFeaturesLayer = (InMemoryFeatureLayer)highlightedFeaturesOverlay.Layers["Highlighted Features"];

            // Clear the currently highlighted features
            highlightedFeaturesLayer.Open();
            highlightedFeaturesLayer.InternalFeatures.Clear();

            // Add new features to the layer
            foreach (var feature in features)
            {
                highlightedFeaturesLayer.InternalFeatures.Add(feature);
            }
            highlightedFeaturesLayer.Close();

            // Refresh the overlay so the layer is redrawn
            highlightedFeaturesOverlay.Refresh();

            // Update the number of matching features found in the UI
            txtNumberOfFeaturesFound.Text = string.Format("Number of features overlapping the drawn shape: {0}", features.Count());
        }

        /// <summary>
        /// Perform the spatial query and draw the shapes on the map
        /// </summary>
        private void GetFeaturesOverlaps(PolygonShape polygon)
        {
            // Find the layers we will be modifying in the MapView
            LayerOverlay queryFeaturesOverlay = (LayerOverlay)mapView.Overlays["Query Features Overlay"];
            InMemoryFeatureLayer queryFeatureLayer = (InMemoryFeatureLayer)queryFeaturesOverlay.Layers["Query Feature"];
            ShapeFileFeatureLayer zoningLayer = (ShapeFileFeatureLayer)mapView.FindFeatureLayer("Frisco Zoning");

            // Clear the query shape layer and add the newly drawn shape
            queryFeatureLayer.InternalFeatures.Clear();
            queryFeatureLayer.InternalFeatures.Add(new Feature(polygon));
            queryFeaturesOverlay.Refresh();

            // Perform the spatial query using the drawn shape and highlight features that were found
            var queriedFeatures = PerformSpatialQuery(polygon, zoningLayer);
            HighlightQueriedFeatures(queriedFeatures);

            // Disable map drawing and clear the drawn shape
            mapView.TrackOverlay.TrackMode = TrackMode.None;
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        /// <summary>
        /// Performs the spatial query when a new polygon is drawn
        /// </summary>
        private void OnPolygonDrawn(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            GetFeaturesOverlaps((PolygonShape)e.TrackShape);
        }

        /// <summary>
        /// Set the map to 'Polygon Drawing Mode' when the user clicks on the map without panning
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            if (!(mapView.TrackOverlay.TrackMode == TrackMode.Polygon))
            {
                // Set the drawing mode to 'Polygon'
                mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
            }
        }
    }
}