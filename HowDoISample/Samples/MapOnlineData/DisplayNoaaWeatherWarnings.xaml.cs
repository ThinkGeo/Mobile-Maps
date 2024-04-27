using System.Collections.ObjectModel;
using System.Text;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayNoaaWeatherWarnings
{
    private bool _initialized;
    private FeatureLayer _noaaWeatherWarningsFeatureLayer;
    public DisplayNoaaWeatherWarnings()
    {
        InitializeComponent();

        MapView.SingleTap += MapView_SingleTap;
    }

    private async void NOAAWeatherWarningLayer_OnSizeChanged(object sender, EventArgs e)
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
        var noaaWeatherWarningsOverlay = new LayerOverlay();
        MapView.Overlays.Add("Noaa Weather Warning", noaaWeatherWarningsOverlay);

        // Create the new layer and set the projection as the data is in srid 4326 and our background is srid 3857 (spherical mercator).
        _noaaWeatherWarningsFeatureLayer = new NoaaWeatherWarningsFeatureLayer();
        _noaaWeatherWarningsFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

        // Add the new layer to the overlay we created earlier
        noaaWeatherWarningsOverlay.Layers.Add("Noaa Weather Warning", _noaaWeatherWarningsFeatureLayer);

        // Create the weather warnings style and add it on zoom level 1 and then apply it to all zoom levels up to 20.
        _noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherWarningsStyle());
        _noaaWeatherWarningsFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 20_000_000;
        MapView.CenterPoint = new PointShape(-10807059, 5045074);

        // Add a PopupOverlay to the map, to display feature information
        var popupOverlay = new PopupOverlay();
        MapView.Overlays.Add("Info Popup Overlay", popupOverlay);

        // Refresh the map.
        await MapView.RefreshAsync();

        LoadingLayout.IsVisible = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (MapView.Overlays["Noaa Weather Warning"] is not LayerOverlay overlay) return;
        var layer = overlay.Layers["Noaa Weather Warning"] as FeatureLayer;
        layer?.Close();
    }

    private async Task DisplayFeatureInfo(Collection<Feature> features)
    {
        if (features.Count > 0)
        {
            var weatherWarningString = new StringBuilder();

            // Each column in a feature is a data attribute
            // Add all attribute pairs to the info string
            foreach (var feature in features)
                weatherWarningString.AppendLine($"{feature.ColumnValues["TITLE"]}");

            // Create a new popup with the park info string
            var popupOverlay = (PopupOverlay)MapView.Overlays["Info Popup Overlay"];
            var popup = new Popup()
            {
                Position = features[0].GetShape().GetCenterPoint(),
                Text = weatherWarningString.ToString()
            };

            // Clear the popup overlay and add the new popup to it
            popupOverlay.Children.Clear();
            popupOverlay.Children.Add(popup);

            // Refresh the overlay to redraw the popups
            await popupOverlay.RefreshAsync();
        }
    }

    public static string ToMultiline(string str)
    {
        var sb = new StringBuilder();
        var words = str.Split(' ');
        var line = new StringBuilder();

        foreach (var word in words)
        {
            if (line.Length + word.Length + 1 > 30)
            {
                sb.AppendLine(line.ToString().TrimEnd());
                line.Clear();
            }
            line.Append(word + " ");
        }

        if (line.Length > 0)
        {
            sb.Append(line.ToString().TrimEnd());
        }

        return sb.ToString();
    }

    private async void MapView_SingleTap(object _, SingleTapMapViewEventArgs e)
    {
        // Find the feature that was tapped on by querying the layer for features containing the tapped coordinates

        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);

        var selectedFeatures =
            _noaaWeatherWarningsFeatureLayer.QueryTools.GetFeaturesContaining(pointInWorldCoordinate, ReturningColumnsType.AllColumns);

        // If a feature was selected, get the data from it and display it
        if (selectedFeatures != null)
            await DisplayFeatureInfo(selectedFeatures);
    }
}