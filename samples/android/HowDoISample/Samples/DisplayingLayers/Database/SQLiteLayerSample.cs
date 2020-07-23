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
    /// This sample shows how to display a SQLite layer.
    /// </summary>
    public class SQLiteLayerSample : SampleFragment
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
            LayerOverlay restuarantsOverlay = new LayerOverlay();
            mapView.Overlays.Add(restuarantsOverlay);
            // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
            var sqlitePath = Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/SQLite/frisco-restaurants.sqlite");
            SqliteFeatureLayer restaurantsLayer = new SqliteFeatureLayer($"Data Source={sqlitePath};", "restaurants", "id", "geometry");
            restaurantsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            restuarantsOverlay.Layers.Add("Frisco Restaurants", restaurantsLayer);

            // Create a new text style and set various settings to make it look good.
            var textStyle = new TextStyle("Name", new GeoFont("Arial", 12), GeoBrushes.Black);
            textStyle.MaskType = MaskType.RoundedCorners;
            textStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            textStyle.Mask = new AreaStyle(GeoBrushes.WhiteSmoke);
            textStyle.SuppressPartialLabels = true;
            textStyle.YOffsetInPixel = -5;

            // Set a point style and the above text style to zoom level 1 and then apply it to all zoom levels up to 20.
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Green, new GeoPen(GeoColors.White, 2));
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            restaurantsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map view current extent to a bounding box that shows just a few restaurants.  
            mapView.CurrentExtent = new RectangleShape(-10776971.1234695, 3915454.06613793, -10775965.157585, 3914668.53918197);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}