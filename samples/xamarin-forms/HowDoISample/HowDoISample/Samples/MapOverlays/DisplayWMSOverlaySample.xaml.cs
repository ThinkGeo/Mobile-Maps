using System;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to render a Web Map Service using the WMSOverlay.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayWMSOverlaySample : ContentPage
    {
        public DisplayWMSOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with a background overlay and set the map's extent to Frisco, Tx.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to Decimal Degrees(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-96.8538765269, 33.1618647290, -96.7987487018, 33.1054126590);

            // Create a WmsOverlay and add it to the map.
            var wmsOverlay = new WmsOverlay(new Uri("http://ows.mundialis.de/services/service"));
            wmsOverlay.Parameters.Add("LAYERS", "OSM-WMS");
            wmsOverlay.Parameters.Add("STYLES", "default");
            mapView.Overlays.Add(wmsOverlay);
            mapView.CollectedMapArguments += MapView_CollectedMapArguments;

            await mapView.RefreshAsync();
        }

        private void MapView_CollectedMapArguments(object sender, CollectedMapArgumentsMapViewEventArgs e)
        {
            // The server doesn't support high-res images so here we set the scaleFactor to 1 to avoid everything being too small. 
            // Comment out the following line and see what the map looks like. 
            e.MapArguments.ScaleFactor = 1;
        }
    }
}