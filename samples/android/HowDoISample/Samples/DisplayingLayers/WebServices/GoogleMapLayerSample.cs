using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a Google Maps layer.
    /// </summary>
    public class GoogleMapLayerSample : SampleFragment
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

            // Sets the map zoom level set to the Google maps zoom level set.
            mapView.ZoomLevelSet = new GoogleMapsZoomLevelSet();

            // Clear the current overlay
            mapView.Overlays.Clear();

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay worldOverlay = new LayerOverlay();
            mapView.Overlays.Add("WorldOverlay", worldOverlay);

            // Create the new layer.
            GoogleMapsLayer worldLayer = new GoogleMapsLayer();

            // Add the layer to the overlay we created earlier.
            worldOverlay.Layers.Add("WorldLayer", worldLayer);

            // Set the client ID and Private key from the text box on the sample.  
            worldLayer.ClientId = "YOUR_GOOGLE_MAPS_CLIENT_ID";
            worldLayer.PrivateKey = "YOUR_GOOGLE_MAPS_PRIVATE_KEY";

            // Set the current extent to the whole world.
            mapView.CurrentExtent = new RectangleShape(-10000000, 10000000, 10000000, -10000000);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}