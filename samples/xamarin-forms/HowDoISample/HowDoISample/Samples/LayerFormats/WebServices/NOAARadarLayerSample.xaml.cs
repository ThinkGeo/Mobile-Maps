using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NOAARadarLayerSample : ContentPage
    {
        public NOAARadarLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     TODO: Update sample once API has been ported
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;
        }
    }
}