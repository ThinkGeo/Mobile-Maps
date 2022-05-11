using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a GeoTiff Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoTiffLayerSample : ContentPage
    {
        public GeoTiffLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Add the GeoTiff layer to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            // Create a new overlay that will hold our new layer and add it to the map.
            var layerOverlay = new LayerOverlay();
            mapView.Overlays.Add(layerOverlay);

            // Create the new layer and dd the layer to the overlay we created earlier.
            var geoTiffRasterLayer = new GeoTiffRasterLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/GeoTiff/World.tif"));
            layerOverlay.Layers.Add(geoTiffRasterLayer);

            // Set the map view current extent to a slightly zoomed in area of the image.
            mapView.CurrentExtent =
                new RectangleShape(-90.5399054799761, 68.8866552710533, 57.5181302343096, -43.7137911575181);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}