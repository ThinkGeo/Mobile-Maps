using System;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to get data from all features in a ShapeFile
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetAllFeatureDataSample : ContentPage
    {
        public GetAllFeatureDataSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco hotels data
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                 "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                 "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a feature layer to hold the Frisco hotels data
            var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Hotels.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            hotelsLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco hotel points
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            hotelsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Star, 24,
                GeoBrushes.MediumPurple, GeoPens.Purple);

            var highlightedHotelLayer = new InMemoryFeatureLayer();
            highlightedHotelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            highlightedHotelLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 30, GeoBrushes.BrightYellow, GeoPens.Black);

            // Add the feature layer to an overlay, and add the overlay to the map
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("Frisco Hotels", hotelsLayer);
            layerOverlay.Layers.Add("Highlighted Hotel", highlightedHotelLayer);
            mapView.Overlays.Add(layerOverlay);

            // Open the hotels layer so we can read the data from it
            hotelsLayer.Open();

            // Get all features from the hotels layer
            // ReturningColumnsType.AllColumns will return all attributes for the features
            var features = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

            // Create a collection of Hotel objects to use as the data source for our list box
            var hotels = new Collection<Hotel>();

            // Create a hotel object based on the data from each hotel feature, and add them to the collection
            foreach (var feature in features)
            {
                var name = feature.ColumnValues["NAME"];
                var address = feature.ColumnValues["ADDRESS"];
                var rooms = int.Parse(feature.ColumnValues["ROOMS"]);
                var location = (PointShape) feature.GetShape();

                hotels.Add(new Hotel(name, address, rooms, location));
            }

            // Set the hotel collection as the data source of the list box
            lsbHotels.ItemsSource = hotels;

            // Set the map extent to the extent of the hotel features
            mapView.CurrentExtent = hotelsLayer.GetBoundingBox();
            hotelsLayer.Close();

            // Refresh and redraw the map
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     When a hotel is selected in the UI, center the map on it
        /// </summary>
        private async void lsbHotels_SelectionChanged(object sender,
            SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            var highlightedHotelLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Highlighted Hotel");
            highlightedHotelLayer.Open();
            highlightedHotelLayer.InternalFeatures.Clear();

            // Get the selected location
            var hotel = lsbHotels.SelectedItem as Hotel;
            if (hotel != null)
            {
                highlightedHotelLayer.InternalFeatures.Add(new Feature(hotel.Location));

                // Center the map on the chosen location
                mapView.CurrentExtent = hotel.Location.GetBoundingBox();
                var standardZoomLevelSet = new ZoomLevelSet();
                await mapView.ZoomToScaleAsync(standardZoomLevelSet.ZoomLevel18.Scale);
                await mapView.RefreshAsync();
            }

            highlightedHotelLayer.Close();
        }

        /// <summary>
        ///     Create a custom 'Hotel' class to use as the data source for our list box
        /// </summary>
        public class Hotel
        {
            public Hotel(string name, string address, int rooms, PointShape location)
            {
                Name = name;
                Address = address;
                Rooms = rooms;
                Location = location;
            }

            public string Name { get; set; }
            public string Address { get; set; }
            public int Rooms { get; set; }
            public PointShape Location { get; set; }
        }
    }
}