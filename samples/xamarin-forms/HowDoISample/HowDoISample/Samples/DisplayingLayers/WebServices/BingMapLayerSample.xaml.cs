using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BingMapLayerSample : ContentPage
    {
        public BingMapLayerSample()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the map zoom level set to the bing map zoom level set so all the zoom levels line up.
            mapView.ZoomLevelSet = new BingMapsZoomLevelSet(256);

            // Set the current extent to the whole world.
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015, 3910065.3144544438);
        }

        /// <summary>
        /// ...
        /// </summary>
        private void btnActivate_Click(object sender, EventArgs e)
        {
            // Create the layer overlay with some additional settings and add to the map.
            LayerOverlay layerOverlay = new LayerOverlay();
            mapView.Overlays.Add("Bing Map", layerOverlay);

            // Create the bing map layer and add it to the map.
            BingMapsLayer bingMapsLayer = new BingMapsLayer(txtApplicationID.Text, BingMapsMapType.Road, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache/bing_maps"));
            layerOverlay.Layers.Add(bingMapsLayer);

            // Refresh the map.
            mapView.Refresh();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.bingmapsportal.com/");
        }
    }
}
