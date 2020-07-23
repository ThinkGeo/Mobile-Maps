using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a Oracle layer.
    /// </summary>
    public class OracleLayerSample : SampleFragment
    {
        // Controls
        private MapView mapView;

        /// <summary>
        /// Defines the Layout to use from the `Resources/layout` directory
        /// </summary>
        public override int Layout => Resource.Layout.__SampleTemplate;

        /// <summary>
        /// Creates the sample view from the Layout resource and exposes controls from the view that needs to be 
        /// referenced for the sample to run (mapView, buttons, etc.)
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Call the base OnCreateView method to inflate the Layout with basic functionality
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            // Bind the controls needed from the Layout to the class
            mapView = view.FindViewById<MapView>(Resource.Id.mapView);

            return view;
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay schoolOverlay = new LayerOverlay();
            mapView.Overlays.Add(schoolOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
            OracleFeatureLayer schoolLayer = new OracleFeatureLayer(@"OCI:ThinkGeoSampleUser/ThinkGeoSamplePassword@sampledatabases.thinkgeo.com/xe", "SYSTEM.SCHOOLS", "OGR_FID");
            schoolLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            schoolOverlay.Layers.Add("Coyote Sightings", schoolLayer);

            // Set a point style to zoom level 1 and then apply it to all zoom levels up to 20.
            schoolLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoColors.White, 2));
            schoolLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map view current extent to a bounding box that shows just a few sightings.  
            mapView.CurrentExtent = new RectangleShape(-10789388.4602951, 3923878.18083465, -10768258.7082788, 3906668.46719412);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}