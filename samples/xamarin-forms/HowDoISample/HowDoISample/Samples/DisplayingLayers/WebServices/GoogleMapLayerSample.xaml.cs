using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThinkGeo.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoogleMapLayerSample : ContentPage
    {
        // Launcher.OpenAsync is provided by Xamarin.Essentials.
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public GoogleMapLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void btnActivate_Click(object sender, EventArgs e)
        {
            //// It is important to set the map unit first to either feet, meters or decimal degrees.
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Sets the map zoom level set to the Google maps zoom level set.
            //mapView.ZoomLevelSet = new GoogleMapsZoomLevelSet();

            //// Clear the current overlay
            //mapView.Overlays.Clear();

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay worldOverlay = new LayerOverlay();
            //mapView.Overlays.Add("WorldOverlay", worldOverlay);

            //// Create the new layer.
            //GoogleMapsLayer worldLayer = new GoogleMapsLayer();

            //// Add the layer to the overlay we created earlier.
            //worldOverlay.Layers.Add("WorldLayer", worldLayer);

            //// Set the client ID and Private key from the text box on the sample.  
            //worldLayer.ClientId = txtClientId.Text;
            //worldLayer.PrivateKey = txtPrivateKey.Text;

            //// Set the current extent to the whole world.
            //mapView.CurrentExtent = new RectangleShape(-10000000, 10000000, 10000000, -10000000);

            //// Refresh the map.
            //mapView.Refresh();
        }

        private void BtnActivate_Clicked(object sender, EventArgs e)
        {

        }
    }
}