using System.Text;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class TimezoneCloudServices
{
    private bool _initialized;
    private TimeZoneCloudClient _timeZoneCloudClient;
    public TimezoneCloudServices()
    {
        InitializeComponent();
        mapView.SingleTap += Map_SingleTap;
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

        mapView.MapTools.Add(new ZoomMapTool());

        // Set the map's unit of measurement to meters (Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Create a PopupOverlay to display time zone information based on locations input by the user
        var timezoneInfoPopupOverlay = new PopupOverlay();

        // Add the overlay to the map
        mapView.Overlays.Add("Timezone Info Popup Overlay", timezoneInfoPopupOverlay);

        // Add a new InMemoryFeatureLayer to hold the timezone shapes
        var timezonesFeatureLayer = new InMemoryFeatureLayer();

        // Add a style to use to draw the timezone polygons
        timezonesFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        timezonesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                2);

        // Add the layer to an overlay, and add it to the map
        var timezonesLayerOverlay = new LayerOverlay();
        timezonesLayerOverlay.Layers.Add("Timezone Feature Layer", timezonesFeatureLayer);
        mapView.Overlays.Add("Timezone Layer Overlay", timezonesLayerOverlay);

        // Initialize the TimezoneCloudClient with our ThinkGeo Cloud credentials
        _timeZoneCloudClient = new TimeZoneCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        // Set the Map Extent
        mapView.CenterPoint = new PointShape(-10777600, 3915260);
        mapView.MapScale = 30000000;

        // Draw the map before showing popups so they appear in the correct location on first load
        await mapView.RefreshAsync();

        // Get Timezone info for Frisco, TX
        await GetTimeZoneInfo(-10779572.80, 3915268.68);
    }

    private async void Map_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        //Run the timezone info query
        var pointInWorldCoordinate = mapView.ToWorldCoordinate(e.X, e.Y);
        await GetTimeZoneInfo(pointInWorldCoordinate.X, pointInWorldCoordinate.Y);
    }

    /// <summary>
    ///     Use the TimezoneCloudClient to query for timezone information
    /// </summary>
    private async Task GetTimeZoneInfo(double lon, double lat)
    {
        CloudTimeZoneResult result;
        try
        {
            // Get timezone info based on the lon, lat, and input projection (Spherical Mercator in this case)
            result = await _timeZoneCloudClient.GetTimeZoneByCoordinateAsync(lon, lat, 3857);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            return;
        }


        // Get the timezone info popup overlay from the Map
        var timezoneInfoPopupOverlay = (PopupOverlay)mapView.Overlays["Timezone Info Popup Overlay"];

        // Clear the existing info popups from the map
        timezoneInfoPopupOverlay.Children.Clear();

        // Build a string description of the timezone
        var timezoneInfoString = new StringBuilder();
        timezoneInfoString.AppendLine($"Time Zone: {result.TimeZone}");
        timezoneInfoString.AppendLine($"Current Local Time: {result.CurrentLocalTime}");
        timezoneInfoString.AppendLine($"Daylight Savings Active: {result.DaylightSavingsActive}");

        // Display the timezone info on a popup on the map
        var popup = new Popup();
        popup.Text = timezoneInfoString.ToString();
        popup.Position = new PointShape(lon, lat);
        timezoneInfoPopupOverlay.Children.Add(popup);

        // Clear the timezone feature layer of previous features        
        var timezonesLayerOverlay = (LayerOverlay)mapView.Overlays["Timezone Layer Overlay"];
        var timezonesFeatureLayer = (InMemoryFeatureLayer)timezonesLayerOverlay.Layers["Timezone Feature Layer"];
        timezonesFeatureLayer.Open();
        timezonesFeatureLayer.InternalFeatures.Clear();

        // Use a ProjectionConverter to convert the shape to Spherical Mercator
        var converter = new ProjectionConverter(3857, 4326);
        converter.Open();

        // Add the new timezone polygon to the map
        var convertedShape = converter.ConvertToInternalProjection(result.Shape);
        timezonesFeatureLayer.InternalFeatures.Add(new Feature(convertedShape));

        converter.Close();
        timezonesFeatureLayer.Close();

        // Refresh and redraw the map
        await timezoneInfoPopupOverlay.RefreshAsync();
        await timezonesLayerOverlay.RefreshAsync();
    }
}
