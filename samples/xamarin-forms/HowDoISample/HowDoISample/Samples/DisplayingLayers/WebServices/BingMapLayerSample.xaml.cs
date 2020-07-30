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

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BingMapLayerSample : ContentPage
    {
        // Launcher.OpenAsync is provided by Xamarin.Essentials.
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public BingMapLayerSample()
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

            //// Set the map zoom level set to the bing map zoom level set so all the zoom levels line up.
            //mapView.ZoomLevelSet = new BingMapsZoomLevelSet(256);

            //// Create the layer overlay with some additional settings and add to the map.
            //LayerOverlay layerOverlay = new LayerOverlay() { TileHeight = 256, TileWidth = 256 };
            //layerOverlay.TileSizeMode = TileSizeMode.Small;
            //layerOverlay.MaxExtent = MaxExtents.BingMaps;
            //mapView.Overlays.Add("Bing Map", layerOverlay);

            //// Create the bing map layer and add it to the map.
            //BingMapsLayer bingMapsLayer = new BingMapsLayer(txtApplicationID.Text, BingMapsMapType.Road, "C:\\temp");
            //layerOverlay.Layers.Add(bingMapsLayer);

            //// Set the current extent to the whole world.
            //mapView.CurrentExtent = new RectangleShape(-10000000, 10000000, 10000000, -10000000);

            //// Refresh the map.
            //mapView.Refresh();
        }
    }
}