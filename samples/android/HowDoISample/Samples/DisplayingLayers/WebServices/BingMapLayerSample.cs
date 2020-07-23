using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a Bing Maps layer.
    /// </summary>
    public class BingMapLayerSample : SampleFragment
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

            // Set the map zoom level set to the bing map zoom level set so all the zoom levels line up.
            mapView.ZoomLevelSet = new BingMapsZoomLevelSet(256);

            // Create the layer overlay with some additional settings and add to the map.
            LayerOverlay layerOverlay = new LayerOverlay() { TileSizeMode = TileSizeMode.Default };
            layerOverlay.TileSizeMode = TileSizeMode.Default;
            layerOverlay.MaxExtent = MaxExtents.BingMaps;
            mapView.Overlays.Add("Bing Map", layerOverlay);

            // Create the bing map layer and add it to the map.
            BingMapsLayer bingMapsLayer = new BingMapsLayer("YOUR_BING_MAPS_KEY", BingMapsMapType.Road);
            layerOverlay.Layers.Add(bingMapsLayer);

            // Set the current extent to the whole world.
            mapView.CurrentExtent = new RectangleShape(-10000000, 10000000, 10000000, -10000000);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}