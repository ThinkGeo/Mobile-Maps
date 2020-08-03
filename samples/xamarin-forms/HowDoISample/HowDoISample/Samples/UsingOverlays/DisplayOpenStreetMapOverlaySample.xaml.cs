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
    public partial class DisplayOpenStreetMapOverlaySample : ContentPage
    {
        public DisplayOpenStreetMapOverlaySample()
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
        }
        /// <summary>
        /// Create an OpenStretMaps overlay and add it to the map view.
        /// </summary>
        private void DisplayOsmMaps_Click(object sender, EventArgs e)
        {
            //OpenStreetMapOverlay osmMapsOverlay = new OpenStreetMapOverlay(osmUserAgent.Text);
            //mapView.Overlays.Add(osmMapsOverlay);
            //mapView.Refresh();
        }
    }
}