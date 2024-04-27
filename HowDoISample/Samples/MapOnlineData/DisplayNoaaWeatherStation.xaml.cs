using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayNoaaWeatherStation
{
    private bool _initialized;
    private NoaaWeatherStationFeatureLayer _noaaWeatherStationLayer;
    public DisplayNoaaWeatherStation()
    {
        InitializeComponent();
    }

    private async void NOAAWeatherStationLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create background world map with vector tile requested from ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var weatherOverlay = new LayerOverlay();
        MapView.Overlays.Add("Weather", weatherOverlay);

        // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
        _noaaWeatherStationLayer = new NoaaWeatherStationFeatureLayer
        {
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(4326, 3857)
            }
        };

        // Add the new layer to the overlay we created earlier
        weatherOverlay.Layers.Add("Noaa Weather Stations", _noaaWeatherStationLayer);

        // Create the weather stations style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
        _noaaWeatherStationLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherStationStyle());
        _noaaWeatherStationLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 20_000_000;
        MapView.CenterPoint = new PointShape(-10807059, 5045074);
        await MapView.RefreshAsync();

        LoadingLayout.IsVisible = false;
    }
}