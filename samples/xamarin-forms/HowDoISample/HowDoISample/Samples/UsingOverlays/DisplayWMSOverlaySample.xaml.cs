using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
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
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add a simple background overlay
            //mapView.BackgroundOverlay.BackgroundBrush = GeoBrushes.AliceBlue;

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            //// Create a WmsOverlay and add it to the map.
            //WmsOverlay wmsOverlay = new WmsOverlay();
            //wmsOverlay.ServerUri = new Uri("http://ows.mundialis.de/services/service");
            //wmsOverlay.Parameters.Add("VERSION", "1.3.0");
            //wmsOverlay.Parameters.Add("LAYERS", "OSM-WMS");
            //wmsOverlay.Parameters.Add("STYLES", "default");
            //wmsOverlay.Parameters.Add("CRS", "EPSG:3857");  // Make sure to match the WMS CRS to the Map's projection
            //mapView.Overlays.Add(wmsOverlay);
        }
    }
}