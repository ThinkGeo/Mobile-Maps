using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display an OpenStreetMaps Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpenStreetMapLayerSample : ContentPage
    {
        public OpenStreetMapLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Add the OpenStreetMaps layer to the map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the zoom level set on the map to make sure its compatible with the OSM zoom levels.
            mapView.ZoomLevelSet = new OpenStreetMapsZoomLevelSet();

            // Create a new overlay that will hold our new layer and add it to the map and set the tile size to match up with the OSM til size.
            var layerOverlay = new LayerOverlay();
            mapView.Overlays.Add(layerOverlay);
            //layerOverlay.TileWidth = 256;
            //layerOverlay.TileHeight = 256;

            // Create the new layer and add it to the overlay.  We set the user agent to specify the requests are coming from our samples.
            // You need to change this to your application so they can identify you for usage.
            var openStreetMapLayer = new OpenStreetMapLayer("ThinkGeo Samples");
            layerOverlay.Layers.Add(openStreetMapLayer);

            // Set the current extent to a local area.
            mapView.CurrentExtent = new RectangleShape(-10789388, 3923878, -10768258, 3906668);

            // Refresh the map.
            await mapView.RefreshAsync();
        }
    }
}