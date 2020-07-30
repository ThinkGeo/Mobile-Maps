using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ESRIGridLayerSample : ContentPage
    {
        public ESRIGridLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// It is important to set the map unit first to either feet, meters or decimal degrees.
            //mapView.MapUnit = GeographyUnit.DecimalDegree;

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay layerOverlay = new LayerOverlay();
            //mapView.Overlays.Add(layerOverlay);

            //// Create the new layer and dd the layer to the overlay we created earlier.
            //EcwRasterLayer ecwRasterLayer = new EcwRasterLayer("../../../Data/Ecw/World.ecw");
            //layerOverlay.Layers.Add(ecwRasterLayer);

            //// Set the map view current extent to a slightly zoomed in area of the image.
            //mapView.CurrentExtent = new RectangleShape(-90.5399054799761, 68.8866552710533, 57.5181302343096, -43.7137911575181);

            //// Refresh the map.
            //mapView.Refresh();
        }
    }
}