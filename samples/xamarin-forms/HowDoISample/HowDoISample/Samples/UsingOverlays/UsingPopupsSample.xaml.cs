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
    public partial class UsingPopupsSample : ContentPage
    {
        public UsingPopupsSample()
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
            //  mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            //AddHotelPopups();
        }
        /// <summary>
        /// Adds hotel popups to the map
        /// </summary>
        private void AddHotelPopups()
        {
            //// Create a PopupOverlay
            //var popupOverlay = new PopupOverlay();

            //// Create a layer in order to query the data
            //var hotelsLayer = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Hotels.shp");

            //// Project the data to match the map's projection
            //hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Open the layer so that we can begin querying
            //hotelsLayer.Open();

            //// Query all the hotel features
            //var hotelFeatures = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

            //// Add each hotel feature to the popupOverlay
            //foreach (var feature in hotelFeatures)
            //{
            //    var popup = new Popup(feature.GetShape().GetCenterPoint())
            //    {
            //        Content = feature.ColumnValues["NAME"]
            //    };
            //    popupOverlay.Popups.Add(popup);
            //}

            //// Close the hotel layer
            //hotelsLayer.Close();

            //// Add the popupOverlay to the map and refresh
            //mapView.Overlays.Add(popupOverlay);
            //mapView.Refresh();
        }
    }
}