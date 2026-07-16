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

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
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
        mapView.Overlays.Add(backgroundOverlay);

        // Set the map's unit of measurement to meters (Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Create a new feature layer to display selected locations returned from the geocode and create styles for it
        var selectedResultItemFeatureLayer = new InMemoryFeatureLayer();
        // Add a point, line, and polygon style to the layer. These styles control how the shapes will be drawn
        selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple);
        selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 6, false);
        selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        selectedResultItemFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a new overlay to display the selected locations returned from the geocode and add it to the map
        var searchFeaturesOverlay = new LayerOverlay();
        searchFeaturesOverlay.Layers.Add("Result Feature Geometry", selectedResultItemFeatureLayer);
        mapView.Overlays.Add("Search Features Overlay", searchFeaturesOverlay);

        // Set the map extent
        mapView.CenterPoint = new PointShape(-10777932, 3912260);
        mapView.MapScale = 100000;
        await mapView.RefreshAsync();
    }

    // Search for an address using the GeocodingCloudClient and update the UI
    private async void Search_Click(object sender, EventArgs e)
    {
        // Run the Cloud Geocoding query using the address entered in the UI
        var searchString = SearchEntry.Text?.Trim();
        if (string.IsNullOrEmpty(searchString))
        {
            await DisplayAlert("Error", "Please enter an address to search", "OK");
            return;
        }

        // Show a loading graphic to let users know the request is running
        LoadingLayout.IsVisible = true;

        var options = new CloudGeocodingOptions
        {
            MaxResults = 10,
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
            ZoomToLocation(searchResult.Locations[0]);
        }
    }

    private async void ZoomToLocation(CloudGeocodingLocation chosenLocation)
    {
        // Get the 'Result Feature' layer from the Map
        var searchFeaturesOverlay = (LayerOverlay)mapView.Overlays["Search Features Overlay"];
        var selectedResultItemFeatureLayer = (InMemoryFeatureLayer)searchFeaturesOverlay.Layers["Result Feature Geometry"];

        // Clear the existing features and add a new feature at the chosen location
        selectedResultItemFeatureLayer.Open();
        selectedResultItemFeatureLayer.Clear();
        selectedResultItemFeatureLayer.InternalFeatures.Add(new Feature(chosenLocation.Shape));

        // Center the map on the chosen location
        await mapView.ZoomToExtentAsync(chosenLocation.BoundingBox.GetCenterPoint(),
        2000, 0);
        await searchFeaturesOverlay.RefreshAsync();
    }
}
