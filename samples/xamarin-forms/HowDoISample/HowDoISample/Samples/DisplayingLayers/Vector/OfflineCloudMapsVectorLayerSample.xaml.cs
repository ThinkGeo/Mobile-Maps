using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    /// Learn how to display ThinkGeo Vector Tiles offline from a local data source
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfflineCloudMapsVectorLayerSample : ContentPage
    {
        public OfflineCloudMapsVectorLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add the ThinkGeoMBTiles Layer to the map and load the data
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a new overlay that will hold our new layer
            LayerOverlay layerOverlay = new LayerOverlay();

            // Create the background world maps using vector tiles stored locally in our MBTiles file and also set the styling though a json file
            ThinkGeoMBTilesLayer mbTilesLayer = new ThinkGeoMBTilesLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Mbtiles/Frisco.mbtiles"), new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Json/thinkgeo-world-streets-dark.json"), UriKind.Relative));
            
            layerOverlay.Layers.Add(mbTilesLayer);

            // Add the overlay to the map
            mapView.Overlays.Add(layerOverlay);

            // Set the current extent of the map to an area in the data
            mapView.CurrentExtent = new RectangleShape(-10785086.173498387, 3913489.693302595, -10779919.030415015, 3910065.3144544438);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}
