using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayWMSOverlaySample : ContentPage
    {
        public DisplayWMSOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            // Add a simple background overlay
            mapView.BackgroundColor = new Color(234, 232, 226);
            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-96.8538765269409, 33.1618647290098, -96.7987487018851, 33.1054126590461);

            // Create a WmsOverlay and add it to the map.
            WmsOverlay wmsOverlay = new WmsOverlay() { ServerUris = { new Uri("http://ows.mundialis.de/services/service")}};
            wmsOverlay.Parameters.Add("LAYERS", "OSM-WMS");
            wmsOverlay.Parameters.Add("STYLES", "default");
            mapView.Overlays.Add(wmsOverlay);

            mapView.Refresh();
        }
    }
}
