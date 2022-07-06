using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

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

            cboLocationCategories.SelectedIndex = 0;

            mapView.Refresh();
        }

        /// <summary>
        ///     Perform the reverse geocode when the user taps on the map
        /// </summary>
        private void mapView_MapSingleTap(object sender, TouchMapViewEventArgs e)
        {
            // Set the coordinates in the UI
            txtCoordinates.Text = string.Format("{0},{1}", e.PointInWorldCoordinate.Y.ToString("0.000000"),
                e.PointInWorldCoordinate.X.ToString("0.000000"));

            // Run the reverse geocode
            PerformReverseGeocode();
        }

        /// <summary>
        ///     Perform the reverse geocode when the user taps the 'Search' button
        /// </summary>
        private async void Search_Click(object sender, EventArgs e)
        {
            await CollapseExpander();
            // Run the reverse geocode using the coordinates in the 'Location' text box
            PerformReverseGeocode();
        }

        /// <summary>
        ///     Perform the reverse geocode using the ReverseGeocodingCloudClient and update the UI
        /// </summary>
        private async void PerformReverseGeocode()
        {
            // Perform some simple validation on the input text boxes
            if (await ValidateSearchParameters())
            {
                var options = new CloudReverseGeocodingOptions();

                // Set up the CloudReverseGeocodingOptions object based on the parameters set in the UI
                var coordinates = txtCoordinates.Text.Split(',');
                var lat = double.Parse(coordinates[0].Trim());
                var lon = double.Parse(coordinates[1].Trim());
                var searchRadius = int.Parse(txtSearchRadius.Text);
                var searchRadiusDistanceUnit = DistanceUnit.Meter;
                var pointProjectionInSrid = 3857;
                var searchPoint = new PointShape(lon, lat);
                options.MaxResults = int.Parse(txtMaxResults.Text);

                switch ((string) cboLocationCategories.SelectedItem)
                {
                    case "All":
                        options.LocationCategories = CloudLocationCategories.All;
                        break;
                    case "Common":
                        options.LocationCategories = CloudLocationCategories.Common;
                        break;
                    case "None":
                        options.LocationCategories = CloudLocationCategories.None;
                        break;
                    default:
                        options.LocationCategories = CloudLocationCategories.All;
                        break;
                }

                // Show a loading graphic to let users know the request is running
                loadingIndicator.IsRunning = true;
                loadingLayout.IsVisible = true;

                // Run the reverse geocode
                var searchResult = await reverseGeocodingCloudClient.SearchPointAsync(lon, lat, pointProjectionInSrid,
                    searchRadius, searchRadiusDistanceUnit, options);

                // Hide the loading graphic
                loadingIndicator.IsRunning = false;
                loadingLayout.IsVisible = false;

                // Handle an exception returned from the service
                if (searchResult.Exception != null)
                {
                    await DisplayAlert("Alert", searchResult.Exception.Message, "Error");
                    return;
                }

                // Update the UI
                DisplaySearchResults(searchPoint, searchRadius, searchResult);
            }
        }

        /// <summary>
        ///     Update the UI based on the search results from the reverse geocode
        /// </summary>
        private void DisplaySearchResults(PointShape searchPoint, int searchRadius,
            CloudReverseGeocodingResult searchResult)
        {
            // Get the 'Search Radius' layer from the MapView
            var searchRadiusFeatureLayer = (InMemoryFeatureLayer) mapView.FindFeatureLayer("Search Radius");

            // Clear the existing features and add new features showing the area that was searched by the reverse geocode
            searchRadiusFeatureLayer.Clear();
            searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(new EllipseShape(searchPoint, searchRadius)));
            searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(searchPoint));

            // Get the 'Result Feature' layer and clear it
            var selectedResultItemFeatureLayer =
                (InMemoryFeatureLayer) mapView.FindFeatureLayer("Result Feature Geometry");
            selectedResultItemFeatureLayer.Clear();

            // If a match was found for the geocode, update the UI
            if (searchResult?.BestMatchLocation != null)
            {
                // Get the 'Best Match' PopupOverlay from the MapView and clear it
                var bestMatchPopupOverlay = (PopupOverlay) mapView.Overlays["Best Match Popup Overlay"];
                bestMatchPopupOverlay.Popups.Clear();

                // Get the location of the 'Best Match' found within the search radius
                var bestMatchLocation = searchResult.BestMatchLocation.LocationFeature.GetShape()
                    .GetClosestPointTo(searchPoint, GeographyUnit.Meter);
                if (bestMatchLocation == null)
                    bestMatchLocation = searchResult.BestMatchLocation.LocationFeature.GetShape().GetCenterPoint();

                // Create a popup to display the best match, and add it to the PopupOverlay
                var bestMatchPopup = new Popup();
                bestMatchPopup.Content = "Best Match: " + searchResult.BestMatchLocation.LocationName;
                bestMatchPopup.Position = bestMatchLocation;
                bestMatchPopupOverlay.Popups.Add(bestMatchPopup);

                // Sort the locations found into three groups (Addresses, Places, Roads) based on their LocationCategory
                var nearbyLocations = new Collection<CloudReverseGeocodingLocation>(searchResult.NearbyLocations);
                var nearbyAddresses = new Collection<CloudReverseGeocodingLocation>();
                var nearbyPlaces = new Collection<CloudReverseGeocodingLocation>();
                var nearbyRoads = new Collection<CloudReverseGeocodingLocation>();
                foreach (var foundLocation in nearbyLocations)
                    if (foundLocation.LocationCategory.ToLower().Contains("addresspoint"))
                        nearbyAddresses.Add(foundLocation);
                    else if (nameof(CloudLocationCategories.Aeroway).Equals(foundLocation.LocationCategory)
                             || nameof(CloudLocationCategories.Road).Equals(foundLocation.LocationCategory)
                             || nameof(CloudLocationCategories.Rail).Equals(foundLocation.LocationCategory)
                             || nameof(CloudLocationCategories.Waterway).Equals(foundLocation.LocationCategory))
                        nearbyRoads.Add(foundLocation);
                    else if (!nameof(CloudLocationCategories.Intersection).Equals(foundLocation.LocationCategory))
                        nearbyPlaces.Add(foundLocation);

                // Set the data sources for the addresses, roads, and places list boxes
                lsbAddresses.ItemsSource = nearbyAddresses;
                lsbRoads.ItemsSource = nearbyRoads;
                lsbPlaces.ItemsSource = nearbyPlaces;

                lsbPlaces.IsVisible = true;

                txtSearchResultsBestMatch.Text = "Best Match: " + searchResult.BestMatchLocation.Address;
            }
            else
            {
                txtSearchResultsBestMatch.Text = "No address or place matches found for this location";
            }

            // Set the map extent to show the results of the search
            mapView.CurrentExtent =
                AreaBaseShape.ScaleUp(searchRadiusFeatureLayer.GetBoundingBox(), 20).GetBoundingBox();
            mapView.Refresh();
        }

        /// <summary>
        ///     When a location is selected in the UI, draw the matching feature found and center the map on it
        /// </summary>
        private void lsbSearchResults_SelectionChanged(object sender, EventArgs e)
        {
            var selectedResultList = (ListView) sender;
            if (selectedResultList.SelectedItem != null)
            {
                // Get the selected location
                var locationFeature = ((CloudReverseGeocodingLocation) selectedResultList.SelectedItem).LocationFeature;

                // Get the 'Result Feature' layer from the MapView
                var selectedResultItemFeatureLayer =
                    (InMemoryFeatureLayer) mapView.FindFeatureLayer("Result Feature Geometry");

                // Clear the existing features and add the geometry of the selected location
                selectedResultItemFeatureLayer.Clear();
                selectedResultItemFeatureLayer.InternalFeatures.Add(new Feature(locationFeature.GetShape()));

                // Center the map on the chosen location
                mapView.CurrentExtent = locationFeature.GetBoundingBox();
                mapView.ZoomToScale(mapView.ZoomLevelSet.ZoomLevel18.Scale);
                mapView.Refresh();
            }
        }

        /// <summary>
        ///     Helper function to change the tip shown for different CloudLocationCategories
        /// </summary>
        private void cboLocationType_SelectionChanged(object sender, EventArgs e)
        {
            var comboBoxContent = (string) cboLocationCategories.SelectedItem;

            if (comboBoxContent != null)
                switch (comboBoxContent)
                {
                    case "All":
                        txtLocationCategoriesDescription.Text = "(Includes all available location types in the search)";
                        break;
                    case "Common":
                        txtLocationCategoriesDescription.Text =
                            "(Includes only commonly-used 'Place' types in the search)";
                        break;
                    case "None":
                        txtLocationCategoriesDescription.Text = "(Only the best matching result will be returned)";
                        break;
                }
        }

        /// <summary>
        ///     Helper function to perform simple validation on the input text boxes
        /// </summary>
        private async Task<bool> ValidateSearchParameters()
        {
            // Check if the 'Location' text box has a valid value
            if (!string.IsNullOrWhiteSpace(txtCoordinates.Text))
            {
                var coordinates = txtCoordinates.Text.Split(',');

                if (coordinates.Count() != 2)
                {
                    txtCoordinates.Focus();
                    await DisplayAlert("Alert", "Please enter a valid set of coordinates to search", "OK");

                    return false;
                }

                if (!(double.TryParse(coordinates[0].Trim(), out var lat) &&
                      double.TryParse(coordinates[1].Trim(), out var lon)))
                {
                    txtCoordinates.Focus();
                    await DisplayAlert("Alert", "Please enter a valid set of coordinates to search", "Error");

                    return false;
                }
            }
            else
            {
                txtCoordinates.Focus();
                await DisplayAlert("Alert", "Please enter a valid set of coordinates to search", "Error");
                return false;
            }

            // Check if the 'Search Radius' text box has a valid value
            if (string.IsNullOrWhiteSpace(txtSearchRadius.Text) ||
                !(int.TryParse(txtSearchRadius.Text, out var searchRadiusInt) && searchRadiusInt > 0))
            {
                txtSearchRadius.Focus();
                await DisplayAlert("Alert", "Please enter an integer greater than 0", "Error");

                return false;
            }

            // Check if the 'Max Results' text box has a valid value
            if (string.IsNullOrWhiteSpace(txtMaxResults.Text) ||
                !(int.TryParse(txtMaxResults.Text, out var maxResultsInt) && maxResultsInt > 0))
            {
                txtMaxResults.Focus();
                await DisplayAlert("Alert", "Please enter an integer greater than 0", "Error");
                return false;
            }

            return true;
        }

        private void btnNearbyAddresses_Clicked(object sender, EventArgs e)
        {
            lsbAddresses.IsVisible = true;
            lsbRoads.IsVisible = false;
            lsbPlaces.IsVisible = false;
        }

        private void btnNearbyRoads_Clicked(object sender, EventArgs e)
        {
            lsbAddresses.IsVisible = false;
            lsbRoads.IsVisible = true;
            lsbPlaces.IsVisible = false;
        }

        private void btnNearbyPlaces_Clicked(object sender, EventArgs e)
        {
            lsbAddresses.IsVisible = false;
            lsbRoads.IsVisible = false;
            lsbPlaces.IsVisible = true;
        }

        private async Task CollapseExpander()
        {
            controlsExpander.IsExpanded = false;
            await Task.Delay((int) controlsExpander.CollapseAnimationLength);
        }
    }
}