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
    /// This sample shows how to display a shapefile layer.
    /// </summary>
    public class ShapefileLayerSample : SampleFragment
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
            LayerOverlay parksOverlay = new LayerOverlay();
            mapView.Overlays.Add(parksOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            ShapeFileFeatureLayer parksLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/Shapefile/Parks.shp"));
            parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            parksOverlay.Layers.Add("Frisco Parks", parksLayer);

            // Create a dashed pen that we will use below.
            var dashedPen = new GeoPen(GeoColors.Green, 5);
            dashedPen.DashPattern.Add(1);
            dashedPen.DashPattern.Add(1);

            // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(dashedPen, new GeoSolidBrush(GeoColors.Transparent));
            parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the current extent of the map to a few parks in the area
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015, 3910065.3144544438);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}