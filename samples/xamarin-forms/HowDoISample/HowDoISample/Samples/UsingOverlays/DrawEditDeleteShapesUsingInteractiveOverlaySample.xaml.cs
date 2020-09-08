using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawEditDeleteShapesUsingInteractiveOverlaySample : ContentPage
    {
        public DrawEditDeleteShapesUsingInteractiveOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;            

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            LayerOverlay layerOverlay;
            InMemoryFeatureLayer featureLayer;

            // Create the layer that will store the drawn shapes
            featureLayer = new InMemoryFeatureLayer();

            // Add styles for the layer
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Blue, GeoColors.Black);
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to a LayerOverlay
            layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("featureLayer", featureLayer);

            // Add the LayerOverlay to the map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            // Update instructions
            instructions.Text = "Navigation Mode - The default map state. Allows you to pan and zoom the map with mouse controls.";

            mapView.Refresh();
        }


        /// <summary>
        /// Update the layer whenever the user switches modes
        /// </summary>
        private void UpdateLayerFeatures(InMemoryFeatureLayer featureLayer, LayerOverlay layerOverlay)
        {
            // If the user switched away from a Drawing Mode, add all the newly drawn shapes in the TrackOverlay into the the featureLayer
            foreach (Feature feature in mapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
            {
                featureLayer.InternalFeatures.Add(feature.Id, feature);
            }

            // Clear out all the TrackOverlay's features
            mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

            // If the user switched away from Edit Mode, add all the shapes that were in the EditOverlay back into the the featureLayer
            foreach (Feature feature in mapView.EditOverlay.EditShapesLayer.InternalFeatures)
            {
                featureLayer.InternalFeatures.Add(feature.Id, feature);
            }

            // Clear out all the EditOverlay's features
            mapView.EditOverlay.EditShapesLayer.InternalFeatures.Clear();

            // Refresh the overlays to show latest results
            mapView.TrackOverlay.Refresh();
            mapView.EditOverlay.Refresh();
            layerOverlay.Refresh();

            // In case the user was in Delete Mode, remove the event handler to avoid deleting features unintentionally
            mapView.MapSingleTap -= MapView_SingleTap;
        }

        /// <summary>
        /// Set the mode to normal navigation. This is the default.
        /// </summary>
        private void NavMode_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to None, so that the user will no longer draw shapes and will be able to naviage the map normally
            mapView.TrackOverlay.TrackMode = TrackMode.None;

            // Update instructions
            instructions.Text = "Navigation Mode - The default map state. Allows you to pan and zoom the map with touch controls.";
        }

        /// <summary>
        /// Set the mode to draw points on the map
        /// </summary>
        private void DrawPoint_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to Point, which draws a new point on the map on mouse tap
            mapView.TrackOverlay.TrackMode = TrackMode.Point;

            // Update instructions
            instructions.Text = "Draw Point Mode - Creates a Point Shape where at the location of each single tap event on the map.";
        }

        /// <summary>
        /// Set the mode to draw lines on the map
        /// </summary>
        private void DrawLine_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to Line, which draws a new line on the map on mouse tap. Double taps to finish drawing the line.
            mapView.TrackOverlay.TrackMode = TrackMode.Line;

            // Update instructions
            instructions.Text = "Draw Line Mode - Begin creating a Line Shape by tapping on the map. Each subsequent tap adds another vertex to the line. Long tap to finish creating the Shape.";
        }

        /// <summary>
        /// Set the mode to draw polygons on the map
        /// </summary>
        private void DrawPolygon_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to Polygon, which draws a new polygon on the map on touch. Double taps to finish drawing the polygon.
            mapView.TrackOverlay.TrackMode = TrackMode.Polygon;

            // Update instructions
            instructions.Text = "Draw Polygon Mode - Begin creating a Polygon Shape by tapping on the map. Each subsequent tap adds another vertex to the polygon. Long tap to finish creating the Shape.";
        }

        /// <summary>
        /// Set the mode to edit drawn shapes
        /// </summary>
        private void EditShape_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to None, so that the user will no longer draw shapes
            mapView.TrackOverlay.TrackMode = TrackMode.None;

            // Put all features in the featureLayer into the EditOverlay
            foreach (Feature feature in featureLayer.InternalFeatures)
            {
                mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
            }

            // Clear all the features in the featureLayer so that the editing features don't overlap with the original shapes
            // In UpdateLayerFeatures, we will add them all back to the featureLayer once the user switches modes
            featureLayer.InternalFeatures.Clear();

            // This method draws all the handles and manipulation points on the map to edit. Essentially putting them all in edit mode.
            mapView.EditOverlay.CalculateAllControlPoints();

            // Refresh the map so that the features properly show that they are in edit mode
            mapView.EditOverlay.Refresh();
            layerOverlay.Refresh();

            // Update instructions
            instructions.Text = "Edit Shapes Mode - Allows the user to modify Shapes. Translate, rotate, or scale a shape using the anchor controls around the shape. Line and Polygon Shapes can also be modified: move a vertex by taping and dragging on an existing vertex, add a vertex by tapping on a line segment, and remove a vertex by double tapping on an existing vertex.";
        }

        /// <summary>
        /// Set the mode to delete features ont the map
        /// </summary>
        private void DeleteShape_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Update the layer's features from any previous mode
            UpdateLayerFeatures(featureLayer, layerOverlay);

            // Set TrackMode to None, so that the user will no longer draw shapes
            mapView.TrackOverlay.TrackMode = TrackMode.None;

            // Add the event handler that will delete features on map tap
            mapView.MapSingleTap += MapView_SingleTap;

            // Update instructions
            instructions.Text = "Delete Shape Mode - Deletes a shape by tapping on the shape.";
        }

        /// <summary>
        /// Event handler that finds the nearest feature and removes it from the layer
        /// </summary>
        private void MapView_SingleTap(object sender, TouchMapViewEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

            // Query the layer for the closest feature within 100 meters
            Collection<Feature> closestFeatures = featureLayer.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1, new Collection<string>(), 100, DistanceUnit.Meter);

            // If a feature was found, remove it from the layer
            if (closestFeatures.Count > 0)
            {
                featureLayer.InternalFeatures.Remove(closestFeatures[0]);

                // Refresh the layerOverlay to show the results
                layerOverlay.Refresh();
            }
        }
    }
}
