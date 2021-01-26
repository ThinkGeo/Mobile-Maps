using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    /// Learn how to display a CloudMapsVector Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayCompassSample : ContentPage
    {
        public DisplayCompassSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;            

            // Create the layer overlay with some additional settings and add to the map.
            ThinkGeoCloudVectorMapsLayer thinkGeoCloudVectorMapsLayer = new ThinkGeoCloudVectorMapsLayer("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            thinkGeoCloudVectorMapsLayer.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            thinkGeoCloudVectorMapsLayer.MapType = ThinkGeoCloudVectorMapsMapType.Light;
            var overlay = new LayerOverlay();
            overlay.Layers.Add(thinkGeoCloudVectorMapsLayer);
            mapView.Overlays.Add("Cloud Overlay", overlay);
            
            // Set the current extent to a neighborhood in Frisco Texas.
            mapView.CurrentExtent = new RectangleShape(-10781708.9749424, 3913502.90429046, -10777685.1114043, 3910360.79646662);

            // Refresh the map.
            mapView.Refresh();
        }
    }
}