using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class GeocodingCloudServices
{
    private bool _initialized;
    public GeocodingCloudServices()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a marker overlay to display the geocoded locations that will be generated, and add it to the map
        var geocodedLocationsOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("Geocoded Locations Overlay", geocodedLocationsOverlay);

        // Set the map's unit of measurement to meters (Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10777932, 3912260);
        MapView.MapScale = 100000;
        await MapView.RefreshAsync();
    }

    // Search for an address using the GeocodingCloudClient and update the UI
    private async void Search_Click(object sender, EventArgs e)
    {
        // Run the Cloud Geocoding query
        var searchString = "6101 Frisco Square Blvd, Frisco, TX 75034";

        // Show a loading graphic to let users know the request is running
        LoadingLayout.IsVisible = true;

        var options = new CloudGeocodingOptions
        {
            // Set up the CloudGeocodingOptions object based on the parameters set in the UI
            MaxResults = 10,
            SearchMode = CloudGeocodingSearchMode.FuzzyMatch,
            LocationType = CloudGeocodingLocationType.Default,
            ResultProjectionInSrid = 3857
        };

        // Initialize the GeocodingCloudClient using our ThinkGeo Cloud credentials
        var geocodingCloudClient = new GeocodingCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);
        var searchResult = await geocodingCloudClient.SearchAsync(searchString, options);

        // Hide the loading graphic
        LoadingLayout.IsVisible = false;

        // Handle an error returned from the geocoding service
        if (searchResult.Exception != null)
        {
            await DisplayAlert("Error", searchResult.Exception.Message, "OK");
            return;
        }

        if (searchResult.Locations.Count > 0)
        {
            //LsbLocations.IsVisible = true;
            ZoomToLocation(searchResult.Locations[0]);
        }
    }

    private async void ZoomToLocation(CloudGeocodingLocation chosenLocation)
    {
        // Get the MarkerOverlay from the MapView
        var geocodedLocationOverlay = (SimpleMarkerOverlay)MapView.Overlays["Geocoded Locations Overlay"];

        // Clear the existing markers and add a new marker at the chosen location
        geocodedLocationOverlay.Children.Clear();
        var newMarker = new ImageMarker
        {
            Position = chosenLocation.LocationPoint,
            ImagePath = "marker.png",
            TranslationY = -17,
            WidthRequest = 20,
            HeightRequest = 34
        };
        geocodedLocationOverlay.Children.Add(newMarker);

        // Center the map on the chosen location
        await MapView.ZoomToExtentAsync(chosenLocation.BoundingBox.GetCenterPoint(),
        2000, 0);
    }
}