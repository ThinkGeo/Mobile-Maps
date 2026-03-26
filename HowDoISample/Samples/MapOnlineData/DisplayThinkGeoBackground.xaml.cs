using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class DisplayThinkGeoBackground
{
    private bool _initialized;
    private ThinkGeoVectorOverlay _thinkGeoVectorsOverlay;
    private ThinkGeoRasterOverlay _thinkGeoRasterOverlay;
    private CancellationTokenSource _cancellationTokenSource = new();

    public DisplayThinkGeoBackground()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        _thinkGeoVectorsOverlay = new ThinkGeoVectorOverlay(
                SampleKeys.ClientId,
                SampleKeys.ClientSecret, ThinkGeoCloudVectorMapsMapType.Light)
        {
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };

        MapView.Overlays.Add(_thinkGeoVectorsOverlay);

        _thinkGeoRasterOverlay = new ThinkGeoRasterOverlay(
                SampleKeys.ClientId,
                SampleKeys.ClientSecret, ThinkGeoCloudRasterMapsMapType.Light_V2_X2)
        {
            IsVisible = false,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
        };

        MapView.Overlays.Add(_thinkGeoRasterOverlay);

        // Setup Maps Extent and Rotation
        MapView.CenterPoint = new PointShape(1180600, 3916260);
        MapView.MapScale = 100000000;

        MapView.MapRotation = 0;
        MapView.IsRotationEnabled = true;

        await MapView.RefreshAsync(_cancellationTokenSource.Token);
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
                ShowVectorOverlay(
                    ThinkGeoCloudVectorMapsMapType.Light,
                    "ThinkGeoVectorLight_VectorCache",
                    "ThinkGeoVectorLight_RasterCache");
                break;
            case "VectorDark":
                ShowVectorOverlay(
                    ThinkGeoCloudVectorMapsMapType.Dark,
                    "ThinkGeoVectorDark_VectorCache",
                    "ThinkGeoVectorDark_RasterCache");
                break;
            case "VectorCustom":
                ShowCustomVectorOverlay();
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

    private void ShowVectorOverlay(
        ThinkGeoCloudVectorMapsMapType mapType,
        string vectorCacheId,
        string rasterCacheId)
    {
        _thinkGeoVectorsOverlay.IsVisible = true;
        _thinkGeoRasterOverlay.IsVisible = false;
        _thinkGeoVectorsOverlay.MapType = mapType;
        _thinkGeoVectorsOverlay.StyleJsonUri = null;
        _thinkGeoVectorsOverlay.VectorTileCache =
            new FileVectorTileCache(FileSystem.Current.CacheDirectory, vectorCacheId);
        _thinkGeoVectorsOverlay.TileCache =
            new FileRasterTileCache(FileSystem.Current.CacheDirectory, rasterCacheId);
    }

    private void ShowCustomVectorOverlay()
    {
        _thinkGeoVectorsOverlay.IsVisible = true;
        _thinkGeoRasterOverlay.IsVisible = false;
        _thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.CustomizedByStyleJson;

        var jsonPath = Path.Combine(
            FileSystem.Current.AppDataDirectory,
            "Data",
            "Json",
            "thinkgeo-world-streets-cobalt.json");
        _thinkGeoVectorsOverlay.StyleJsonUri = new Uri(jsonPath);
        _thinkGeoVectorsOverlay.VectorTileCache =
            new FileVectorTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorDark_VectorCache");
        _thinkGeoVectorsOverlay.TileCache =
            new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorCobalt_RasterCache");
    }

    private async Task RefreshMapAsync()
    {
        await UpdateCancellationToken();
        await MapView.RefreshAsync(_cancellationTokenSource.Token);
    }

    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
    }
}
