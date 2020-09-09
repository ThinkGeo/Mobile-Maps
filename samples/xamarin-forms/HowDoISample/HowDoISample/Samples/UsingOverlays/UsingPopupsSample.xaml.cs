using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    /// Learn to add, edit, or remove popups on the map using the PopupOverlay.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsingPopupsSample : ContentPage
    {
        public UsingPopupsSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;            

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            AddHotelPopups();

            mapView.Refresh();
        }
        /// <summary>
        /// Adds hotel popups to the map
        /// </summary>
        private void AddHotelPopups()
        {
            // Create a PopupOverlay
            var popupOverlay = new PopupOverlay();

            // Create a layer in order to query the data
            var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Hotels.shp"));

            // Project the data to match the map's projection
            hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Open the layer so that we can begin querying
            hotelsLayer.Open();

            // Query all the hotel features
            var hotelFeatures = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

            // Add each hotel feature to the popupOverlay
            foreach (var feature in hotelFeatures)
            {
                var popup = new Popup()
                {
                    Position = feature.GetShape().GetCenterPoint(),
                    Content = feature.ColumnValues["NAME"]
                };
                popupOverlay.Popups.Add(popup);
            }

            // Close the hotel layer
            hotelsLayer.Close();

            // Add the popupOverlay to the map and refresh
            mapView.Overlays.Add(popupOverlay);
            mapView.Refresh();
        }
    }
}
