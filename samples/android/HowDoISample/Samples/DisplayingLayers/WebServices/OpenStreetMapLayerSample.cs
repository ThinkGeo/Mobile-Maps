using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a Open Street Maps layer.
    /// </summary>
    public class OpenStreetMapLayerSample : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the zoom level set on the map to make sure its compatable with the OSM zoom levels.
            mapView.ZoomLevelSet = new OpenStreetMapsZoomLevelSet();

            // Create a new overlay that will hold our new layer and add it to the map and set the tile size to match up with the OSM til size.
            var layerOverlay = new LayerOverlay();
            mapView.Overlays.Add(layerOverlay);
            layerOverlay.TileSizeMode = TileSizeMode.Default;

            // Create the new layer and add it to the overlay.  We set the user agent to specify the requests are coming from our samples.
            // You need to change this to your application so they can identify you for usage.
            var openStreetMapLayer = new ThinkGeo.Core.OpenStreetMapLayer("ThinkGeo Samples/12.0 (http://thinkgeo.com/; system@thinkgeo.com)");
            layerOverlay.Layers.Add(openStreetMapLayer);

            // Set the current extent to a local area.
            mapView.CurrentExtent = new RectangleShape(-10789388.4602951, 3923878.18083465, -10768258.7082788, 3906668.46719412);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}