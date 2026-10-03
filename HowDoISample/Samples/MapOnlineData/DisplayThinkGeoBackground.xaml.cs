using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class DisplayThinkGeoBackground
{
    private bool _initialized;
    private ThinkGeoVectorTileOverlay _thinkGeoVectorsOverlay;
    private ThinkGeoRasterOverlay _thinkGeoRasterOverlay;
    private CancellationTokenSource _cancellationTokenSource = new();

    public DisplayThinkGeoBackground()
    {
        InitializeComponent();
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        _thinkGeoVectorsOverlay = new ThinkGeoVectorTileOverlay(SampleKeys.ApiKey)
        {
            VectorTileCache = new FileTileCache(Path.Combine(FileSystem.Current.CacheDirectory, "ThinkGeoVectorTile_VectorCache")),
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorTileLight_RasterCache")
        };

        mapView.Overlays.Add(_thinkGeoVectorsOverlay);

        _thinkGeoRasterOverlay = new ThinkGeoRasterOverlay(
                SampleKeys.ClientId,
                SampleKeys.ClientSecret, ThinkGeoCloudRasterMapsMapType.Light_V2_X2)
        {
            IsVisible = false,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
        };

        mapView.Overlays.Add(_thinkGeoRasterOverlay);

        // Setup Maps Extent and Rotation
        mapView.CenterPoint = new PointShape(1180600, 3916260);
        mapView.MapScale = 100000000;

        mapView.MapRotation = 0;
        mapView.IsRotationEnabled = true;

        await mapView.RefreshAsync(_cancellationTokenSource.Token);
    }

    private async void Radial_Checked(object sender, CheckedChangedEventArgs e)
    {
        if (_thinkGeoVectorsOverlay == null || _thinkGeoRasterOverlay == null)
            return;
        if (!e.Value)
            return;

        var radioButton = (RadioButton)sender;
        switch (radioButton.StyleId)
        {
            case "RasterLightV1":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Light_V1_X2);
                break;
            case "RasterLightV2":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Light_V2_X2);
                break;
            case "RasterDarkV1":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Dark_V1_X2);
                break;
            case "RasterDarkV2":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Dark_V2_X2);
                break;
            case "Aerial":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Aerial2_V2_X2);
                break;
            case "Hybrid":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.Hybrid2_V2_X2);
                break;
            case "TransparentBg":
                ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType.TransparentBackground_V2_X2);
                break;
            case "VectorLight":
                ShowVectorOverlay(ThinkGeoVectorStyles.Light, "ThinkGeoVectorTileLight_RasterCache");
                break;
            case "VectorDark":
                ShowVectorOverlay(ThinkGeoVectorStyles.Dark, "ThinkGeoVectorTileDark_RasterCache");
                break;
            case "VectorCustom":
                // A MapLibre style document of your own, shipped with the app.
                var cobaltStyle = Path.Combine(
                    FileSystem.Current.AppDataDirectory,
                    "Data",
                    "Json",
                    "thinkgeo-world-streets-cobalt-gl.json");
                ShowVectorOverlay(cobaltStyle, "ThinkGeoVectorTileCobalt_RasterCache");
                break;
            default:
                return;
        }

        await RefreshMapAsync();
    }

    private async Task UpdateCancellationToken()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void ShowRasterOverlay(ThinkGeoCloudRasterMapsMapType mapType)
    {
        _thinkGeoVectorsOverlay.IsVisible = false;
        _thinkGeoRasterOverlay.IsVisible = true;
        _thinkGeoRasterOverlay.MapType = mapType;
    }

    private void ShowVectorOverlay(string styleUri, string rasterCacheId)
    {
        _thinkGeoVectorsOverlay.IsVisible = true;
        _thinkGeoRasterOverlay.IsVisible = false;
        _thinkGeoVectorsOverlay.StyleUri = styleUri;
        _thinkGeoVectorsOverlay.TileCache =
            new FileRasterTileCache(FileSystem.Current.CacheDirectory, rasterCacheId);
    }

    private async Task RefreshMapAsync()
    {
        await UpdateCancellationToken();
        await mapView.RefreshAsync(_cancellationTokenSource.Token);
    }

    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
    }
}
