using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ReverseGeocodingCloudServices
{
    private bool _initialized;
    private ReverseGeocodingCloudClient _reverseGeocodingCloudClient;
    private ObservableCollection<CloudReverseGeocodingLocation> _nearByLocations;

    public ReverseGeocodingCloudServices()
    {
        InitializeComponent();
        NearByLocations = [];
        ResultView.IsVisible = false;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
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
        MapView.Overlays.Add(backgroundOverlay);

        // Set the map's unit of measurement to meters (Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

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
        MapView.Overlays.Add("Search Features Overlay", searchFeaturesOverlay);

        // Create a popup overlay to display the best match
        var bestMatchPopupOverlay = new PopupOverlay();
        MapView.Overlays.Add("Best Match Popup Overlay", bestMatchPopupOverlay);

        // Set the map extent to Frisco, TX
        MapView.CenterPoint = new PointShape(-10779570, 3915041);
        MapView.MapScale = 8000;

        // Initialize the ReverseGeocodingCloudClient with our ThinkGeo Cloud credentials
        _reverseGeocodingCloudClient = new ReverseGeocodingCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);
        await MapView.RefreshAsync();
    }

    public ObservableCollection<CloudReverseGeocodingLocation> NearByLocations
    {
        get => _nearByLocations;
        set
        {
            _nearByLocations = value;
            OnPropertyChanged();
        }
    }

    private async void MapView_OnSingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var searchPoint = MapView.ToWorldCoordinate(e.X, e.Y);

        var options = new CloudReverseGeocodingOptions
        {
            MaxResults = 20,
            LocationCategories = CloudLocationCategories.All
        };

        // Run the reverse geocode
        const int searchRadius = 400;
        var searchResult = await _reverseGeocodingCloudClient.SearchPointAsync(searchPoint.X, searchPoint.Y, 3857,
            searchRadius, DistanceUnit.Meter, options);

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
        ResultView.IsVisible = true;

        // Get the 'Search Radius' layer from the MapView        
        var searchFeaturesOverlay = (LayerOverlay)MapView.Overlays["Search Features Overlay"];
        var searchRadiusFeatureLayer = (InMemoryFeatureLayer)searchFeaturesOverlay.Layers["Search Radius"];
        searchRadiusFeatureLayer.Open();
        // Clear the existing features and add new features showing the area that was searched by the reverse geocode
        searchRadiusFeatureLayer.Clear();
        searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(new EllipseShape(searchPoint, searchRadius)));
        searchRadiusFeatureLayer.InternalFeatures.Add(new Feature(searchPoint));

        // Get the 'Result Feature' layer and clear it
        var selectedResultItemFeatureLayer = (InMemoryFeatureLayer)searchFeaturesOverlay.Layers["Result Feature Geometry"];
        selectedResultItemFeatureLayer.Open();
        selectedResultItemFeatureLayer.Clear();

        // If a match was found for the geocode, update the UI
        if (searchResult?.BestMatchLocation != null)
        {
            // Get the 'Best Match' PopupOverlay from the MapView and clear it
            var bestMatchPopupOverlay = (PopupOverlay)MapView.Overlays["Best Match Popup Overlay"];
            bestMatchPopupOverlay.Children.Clear();

            // Get the location of the 'Best Match' found within the search radius
            var bestMatchLocation = searchResult.BestMatchLocation.LocationFeature.GetShape()
                .GetClosestPointTo(searchPoint, GeographyUnit.Meter);
            bestMatchLocation ??= searchResult.BestMatchLocation.LocationFeature.GetShape().GetCenterPoint();

            // Create a popup to display the best match, and add it to the PopupOverlay
            var bestMatchPopup = new Popup();
            bestMatchPopup.Text = "Best Match: " + searchResult.BestMatchLocation.LocationName;
            bestMatchPopup.Position = bestMatchLocation;
            bestMatchPopupOverlay.Children.Add(bestMatchPopup);

            NearByLocations.Clear();
            foreach (var nearbyLocation in searchResult.NearbyLocations)
            {
                if (string.IsNullOrEmpty(nearbyLocation.Address))
                    continue; // skip found intersections 
                NearByLocations.Add(nearbyLocation);
            }
        }

        // Set the map extent to show the results of the search
        await searchFeaturesOverlay.RefreshAsync();
    }



    /// <summary>
    ///     When a location is selected in the UI, draw the matching feature found and center the map on it
    /// </summary>
    private async void lsbSearchResults_SelectionChanged(object sender, EventArgs e)
    {
        var selectedResultList = (ListView)sender;
        if (selectedResultList.SelectedItem == null) return;
        // Get the selected location
        var locationFeature = ((CloudReverseGeocodingLocation)selectedResultList.SelectedItem).LocationFeature;

        // Get the 'Result Feature' layer from the MapView            
        var searchFeaturesOverlay = (LayerOverlay)MapView.Overlays["Search Features Overlay"];
        var selectedResultItemFeatureLayer = (InMemoryFeatureLayer)searchFeaturesOverlay.Layers["Result Feature Geometry"];

        // Clear the existing features and add the geometry of the selected location
        selectedResultItemFeatureLayer.Clear();
        selectedResultItemFeatureLayer.InternalFeatures.Add(new Feature(locationFeature.GetShape()));

        await searchFeaturesOverlay.RefreshAsync();
    }
}