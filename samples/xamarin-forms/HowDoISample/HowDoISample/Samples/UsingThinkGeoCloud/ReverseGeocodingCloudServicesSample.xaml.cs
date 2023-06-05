using System;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the ReverseGeocodingCloudClient to access the ReverseGeocoding APIs available from the ThinkGeo
    ///     Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReverseGeocodingCloudServicesSample : ContentPage
    {
        private ReverseGeocodingCloudClient reverseGeocodingCloudClient;

        public ReverseGeocodingCloudServicesSample()
        {
            InitializeComponent();
        }


        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay, as well as several feature layers to display the reverse
        ///     geocoding search area and locations
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

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a new feature layer to display the search radius of the reverse geocode and create a style for it
            var searchRadiusFeatureLayer = new InMemoryFeatureLayer();
            searchRadiusFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
                new GeoPen(new GeoColor(100, GeoColors.Blue)), new GeoSolidBrush(new GeoColor(10, GeoColors.Blue)));
            searchRadiusFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Cross, 20, GeoBrushes.Red);
            searchRadiusFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create a new feature layer to display selected locations returned from the reverse geocode and create styles for it
            var selectedResultItemFeatureLayer = new InMemoryFeatureLayer();
            // Add a point, line, and polygon style to the layer. These styles control how the shapes will be drawn
            selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple);
            selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 6, false);
            selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);
            selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create an overlay and add the feature layers to it
            var searchFeaturesOverlay = new LayerOverlay();
            searchFeaturesOverlay.Layers.Add("Search Radius", searchRadiusFeatureLayer);
            searchFeaturesOverlay.Layers.Add("Result Feature Geometry", selectedResultItemFeatureLayer);

            // Create a popup overlay to display the best match
            var bestMatchPopupOverlay = new PopupOverlay();

            // Add the overlays to the map
            mapView.Overlays.Add("Search Features Overlay", searchFeaturesOverlay);
            mapView.Overlays.Add("Best Match Popup Overlay", bestMatchPopupOverlay);

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent =
                new RectangleShape(-10780022.026198, 3915814.54657467, -10779119.113802, 3914667.51342533);

            // Initialize the ReverseGeocodingCloudClient with our ThinkGeo Cloud credentials
            reverseGeocodingCloudClient = new ReverseGeocodingCloudClient(
                "FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            await PerformReverseGeocodeAsync(-10779570, 3915241);


        }

        /// <summary>
        ///     Perform the reverse geocode when the user taps on the map
        /// </summary>
        private async void mapView_MapSingleTap(object sender, TouchMapViewEventArgs e)
        {
            // Run the reverse geocode
            await PerformReverseGeocodeAsync(e.PointInWorldCoordinate.X, e.PointInWorldCoordinate.Y);
        }

        /// <summary>
        ///     Perform the reverse geocode using the ReverseGeocodingCloudClient and update the UI
        /// </summary>
        private async Task PerformReverseGeocodeAsync(double x, double y)
        {
            // Perform some simple validation on the input text boxes
            var options = new CloudReverseGeocodingOptions();

            // Set up the CloudReverseGeocodingOptions object based on the parameters set in the UI
            var searchRadius = 400;
            var searchRadiusDistanceUnit = DistanceUnit.Meter;
            var pointProjectionInSrid = 3857;
            var searchPoint = new PointShape(x, y);
            options.MaxResults = 20;

            options.LocationCategories = CloudLocationCategories.All;

            // Run the reverse geocode
            var searchResult = await reverseGeocodingCloudClient.SearchPointAsync(x, y, pointProjectionInSrid,
                searchRadius, searchRadiusDistanceUnit, options);

            // Handle an exception returned from the service
            if (searchResult.Exception != null)
            {
                await DisplayAlert("Alert", searchResult.Exception.Message, "Error");
                return;
            }

            // Update the UI
            await DisplaySearchResults(searchPoint, searchRadius, searchResult);
        }

        /// <summary>
        ///     Update the UI based on the search results from the reverse geocode
        /// </summary>
        private async Task DisplaySearchResults(PointShape searchPoint, int searchRadius,
            CloudReverseGeocodingResult searchResult)
        {
            // Get the 'Search Radius' layer from the MapView
            var searchRadiusFeatureLayer = (InMemoryFeatureLayer)mapView.FindFeatureLayer("Search Radius");
            searchRadiusFeatureLayer.Open();
            // Clear the existing features and add new features showing the area that was searched by the reverse geocode
            searchRadiusFeatureLayer.Clear();
            searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(new EllipseShape(searchPoint, searchRadius)));
            searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(searchPoint));

            // Get the 'Result Feature' layer and clear it
            var selectedResultItemFeatureLayer = (InMemoryFeatureLayer)mapView.FindFeatureLayer("Result Feature Geometry");
            selectedResultItemFeatureLayer.Open();
            selectedResultItemFeatureLayer.Clear();

            // If a match was found for the geocode, update the UI
            if (searchResult?.BestMatchLocation != null)
            {
                // Get the 'Best Match' PopupOverlay from the MapView and clear it
                var bestMatchPopupOverlay = (PopupOverlay)mapView.Overlays["Best Match Popup Overlay"];
                bestMatchPopupOverlay.Popups.Clear();

                // Get the location of the 'Best Match' found within the search radius
                var bestMatchLocation = searchResult.BestMatchLocation.LocationFeature.GetShape()
                    .GetClosestPointTo(searchPoint, GeographyUnit.Meter);
                if (bestMatchLocation == null)
                    bestMatchLocation = searchResult.BestMatchLocation.LocationFeature.GetShape().GetCenterPoint();

                // Create a popup to display the best match, and add it to the PopupOverlay
                var bestMatchPopup = new Popup();
                bestMatchPopup.Text = "Best Match: " + searchResult.BestMatchLocation.LocationName;
                bestMatchPopup.Position = bestMatchLocation;
                bestMatchPopupOverlay.Popups.Add(bestMatchPopup);

                lsbAddresses.ItemsSource = searchResult.NearbyLocations;
                
                txtSearchResultsBestMatch.Text = "Best Match: " + searchResult.BestMatchLocation.Address;
            }
            else
            {
                txtSearchResultsBestMatch.Text = "No address or place matches found for this location";
            }

            // Set the map extent to show the results of the search
            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     When a location is selected in the UI, draw the matching feature found and center the map on it
        /// </summary>
        private async void lsbSearchResults_SelectionChanged(object sender, EventArgs e)
        {
            var selectedResultList = (ListView)sender;
            if (selectedResultList.SelectedItem != null)
            {
                // Get the selected location
                var locationFeature = ((CloudReverseGeocodingLocation)selectedResultList.SelectedItem).LocationFeature;

                // Get the 'Result Feature' layer from the MapView
                var selectedResultItemFeatureLayer =
                    (InMemoryFeatureLayer)mapView.FindFeatureLayer("Result Feature Geometry");

                // Clear the existing features and add the geometry of the selected location
                selectedResultItemFeatureLayer.Clear();
                selectedResultItemFeatureLayer.InternalFeatures.Add(new Feature(locationFeature.GetShape()));

                await mapView.RefreshAsync();
            }
        }
    }
}