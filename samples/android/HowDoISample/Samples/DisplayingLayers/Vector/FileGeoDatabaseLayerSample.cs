using System;
using System.Collections.ObjectModel;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using Xamarin.Essentials;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a file geodatabase layer.
    /// </summary>
    public class FileGeoDatabaseLayerSample : SampleFragment
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
            LayerOverlay fileGeoDatabaseOverlay = new LayerOverlay();
            mapView.Overlays.Add(fileGeoDatabaseOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            FileGeoDatabaseFeatureLayer fileGeoDatabaseFeatureLayer = new FileGeoDatabaseFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/FileGeoDatabase/zoning.gdb"));
            fileGeoDatabaseFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            fileGeoDatabaseFeatureLayer.ActiveLayer = "zoning";

            // Add the layer to the overlay we created earlier.
            fileGeoDatabaseOverlay.Layers.Add("Zoning", fileGeoDatabaseFeatureLayer);

            // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            fileGeoDatabaseFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.Black, new GeoSolidBrush(new GeoColor(50, GeoColors.Blue)));
            fileGeoDatabaseFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Open the layer and set the map view current extent to the bounding box of the layer.  
            fileGeoDatabaseFeatureLayer.Open();
            mapView.CurrentExtent = fileGeoDatabaseFeatureLayer.GetBoundingBox();

            //Refresh the map.
            mapView.Refresh();
        }
    }
}