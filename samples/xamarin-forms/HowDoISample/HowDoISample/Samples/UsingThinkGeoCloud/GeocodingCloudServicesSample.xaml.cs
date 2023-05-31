using System;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to use the GeocodingCloudClient to access the Geocoding APIs available from the ThinkGeo Cloud
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeocodingCloudServicesSample : ContentPage
    {
        private GeocodingCloudClient geocodingCloudClient;

        public GeocodingCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay
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

            // Create a marker overlay to display the geocoded locations that will be generated, and add it to the map
            var geocodedLocationsOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("Geocoded Locations Overlay", geocodedLocationsOverlay);

            // Set the map extent to Frisco, TX
            mapView.CurrentExtent =
                new RectangleShape(-10798419.605087, 3934270.12359632, -10759021.6785336, 3896039.57306867);

            // Initialize the GeocodingCloudClient using our ThinkGeo Cloud credentials
            geocodingCloudClient = new GeocodingCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~",
                "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            cboSearchType.SelectedIndex = 0;
            cboLocationType.SelectedIndex = 0;

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Search for an address using the GeocodingCloudClient
        /// </summary>
        private async Task<CloudGeocodingResult> PerformGeocodingQuery()
        {
            // Show a loading graphic to let users know the request is running
            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            var options = new CloudGeocodingOptions();
            // Set up the CloudGeocodingOptions object based on the parameters set in the UI
            options.MaxResults = int.Parse(txtMaxResults.Text);
            options.SearchMode = (string) cboSearchType.SelectedItem == "Fuzzy"
                ? CloudGeocodingSearchMode.FuzzyMatch
                : CloudGeocodingSearchMode.ExactMatch;
            options.LocationType = (CloudGeocodingLocationType) Enum.Parse(typeof(CloudGeocodingLocationType),
                (string) cboLocationType.SelectedItem);
            options.ResultProjectionInSrid = 3857;

            // Run the geocode
            var searchString = txtSearchString.Text.Trim();
            var searchResult = await geocodingCloudClient.SearchAsync(searchString, options);

            // Hide the loading graphic
            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;

            return searchResult;
        }

        /// <summary>
        ///     Update the UI based on the results of a Cloud Geocoding Query
        /// </summary>
        private async Task UpdateSearchResultsOnUI(CloudGeocodingResult searchResult)
        {
            // Clear the locations list and existing location markers on the map
            var geocodedLocationOverlay = (SimpleMarkerOverlay) mapView.Overlays["Geocoded Locations Overlay"];
            geocodedLocationOverlay.Markers.Clear();
            lsbLocations.ItemsSource = null;
            await geocodedLocationOverlay.RefreshAsync();

            // Update the UI with the number of results found and the list of locations found
            txtSearchResultsDescription.Text = $"Found {searchResult.Locations.Count} matching locations.";
            lsbLocations.ItemsSource = searchResult.Locations;
            if (searchResult.Locations.Count > 0)
            {
                lsbLocations.IsVisible = true;
                lsbLocations.SelectedItem = searchResult.Locations[0];
            }
        }

        /// <summary>
        ///     Search for an address using the GeocodingCloudClient and update the UI
        /// </summary>
        private async void Search_Click(object sender, EventArgs e)
        {
            await CollapseExpander();

            // Perform some simple validation on the input text boxes
            if (await ValidateSearchParameters())
            {
                // Run the Cloud Geocoding query
                var searchResult = await PerformGeocodingQuery();

                // Handle an error returned from the geocoding service
                if (searchResult.Exception != null)
                {
                    await DisplayAlert("Error", searchResult.Exception.Message, "OK");
                    return;
                }

                // Update the UI based on the results
                await UpdateSearchResultsOnUI(searchResult);
            }
        }

        /// <summary>
        ///     When a location is selected in the UI, add a marker at that location and center the map on it
        /// </summary>
        private async void lsbLocations_SelectionChanged(object sender, EventArgs e)
        {
            // Get the selected location
            var chosenLocation = lsbLocations.SelectedItem as CloudGeocodingLocation;
            if (chosenLocation != null)
            {
                // Get the MarkerOverlay from the MapView
                var geocodedLocationOverlay = (SimpleMarkerOverlay) mapView.Overlays["Geocoded Locations Overlay"];

                // Clear the existing markers and add a new marker at the chosen location
                geocodedLocationOverlay.Markers.Clear();
                geocodedLocationOverlay.Markers.Add(CreateNewMarker(chosenLocation.LocationPoint));

                // Center the map on the chosen location
                mapView.CurrentExtent = chosenLocation.BoundingBox;
                var standardZoomLevelSet = new ZoomLevelSet();
                await mapView.ZoomToScaleAsync(standardZoomLevelSet.ZoomLevel18.Scale);
                await mapView.RefreshAsync();
            }
        }

        /// <summary>
        ///     Helper function to change the tip shown for different Search Types
        /// </summary>
        private void cboSearchType_SelectionChanged(object sender, EventArgs e)
        {
            var pickerContent = (string) cboSearchType.SelectedItem;

            if (pickerContent != null)
                switch (pickerContent)
                {
                    case "Fuzzy":
                        txtSearchTypeDescription.Text =
                            "(Returns both exact and approximate matches for the search address)";
                        break;
                    case "Exact":
                        txtSearchTypeDescription.Text = "(Only returns exact matches for the search address)";
                        break;
                }
        }

        /// <summary>
        ///     Helper function to change the tip shown for different Location Types
        /// </summary>
        private void cboLocationType_SelectionChanged(object sender, EventArgs e)
        {
            var pickerItem = (string) cboLocationType.SelectedItem;

            if (pickerItem != null)
                switch (pickerItem)
                {
                    case "Default":
                        txtLocationTypeDescription.Text = "(Searches for any matches to the search string)";
                        break;
                    case "Address":
                        txtLocationTypeDescription.Text = "(Searches for addresses matching the search string)";
                        break;
                    case "Street":
                        txtLocationTypeDescription.Text = "(Searches for streets matching the search string)";
                        break;
                    case "City":
                        txtLocationTypeDescription.Text = "(Searches for cities matching the search string)";
                        break;
                    case "County":
                        txtLocationTypeDescription.Text = "(Searches for counties matching the search string)";
                        break;
                    case "ZipCode":
                        txtLocationTypeDescription.Text = "(Searches for zip codes matching the search string)";
                        break;
                    case "State":
                        txtLocationTypeDescription.Text = "(Searches for states matching the search string)";
                        break;
                }
        }

        /// <summary>
        ///     Helper function to perform simple validation on the input text boxes
        /// </summary>
        private async Task<bool> ValidateSearchParameters()
        {
            // Check if the address text box is empty
            if (string.IsNullOrWhiteSpace(txtSearchString.Text))
            {
                txtSearchString.Focus();
                await DisplayAlert("Alert", "Please enter an address to search", "OK");
                return false;
            }

            // Check if the 'Max Results' text box has a valid value
            if (string.IsNullOrWhiteSpace(txtMaxResults.Text) ||
                !(int.TryParse(txtMaxResults.Text, out var result) && result > 0 && result < 101))
            {
                txtMaxResults.Focus();
                await DisplayAlert("Alert", "Please enter a number between 1 - 100", "OK");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Create a new map marker using preloaded image assets
        /// </summary>
        private Marker CreateNewMarker(PointShape point)
        {
            return new Marker
            {
                Position = point,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Resources/AQUA.png"),
                YOffset = -17
            };
        }

        private async Task CollapseExpander()
        {
            controlsExpander.IsExpanded = false;
            await Task.Delay((int) controlsExpander.CollapseAnimationLength);
        }
    }
}