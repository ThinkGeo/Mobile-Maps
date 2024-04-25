using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class ThinkGeoBackgroundOverlays
{
    private bool _initialized;
    private ThinkGeoVectorOverlay _thinkGeoVectorsOverlay;
    private ThinkGeoRasterOverlay _thinkGeoRasterOverlay;
    private CancellationTokenSource _cancellationTokenSource = new();

    public ThinkGeoBackgroundOverlays()
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
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light_V1_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "RasterLightV2":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "RasterDarkV1":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark_V1_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "RasterDarkV2":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark_V2_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "Aerial":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "Hybrid":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid_V2_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "TransparentBg":
                _thinkGeoVectorsOverlay.IsVisible = false;
                _thinkGeoRasterOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.MapType = ThinkGeoCloudRasterMapsMapType.TransparentBackground_V2_X2;

                await UpdateCancellationToken();
                await _thinkGeoRasterOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "VectorLight":
                _thinkGeoVectorsOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.IsVisible = false;
                _thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Light;

                _thinkGeoVectorsOverlay.VectorTileCache = new FileVectorTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_VectorCache");
                _thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache");
                await UpdateCancellationToken();
                await _thinkGeoVectorsOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "VectorDark":
                _thinkGeoVectorsOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.IsVisible = false;
                _thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.Dark;

                _thinkGeoVectorsOverlay.VectorTileCache = new FileVectorTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorDark_VectorCache");
                _thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorDark_RasterCache");
                await UpdateCancellationToken();
                await _thinkGeoVectorsOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
            case "VectorCustom":
                _thinkGeoVectorsOverlay.IsVisible = true;
                _thinkGeoRasterOverlay.IsVisible = false;
                _thinkGeoVectorsOverlay.MapType = ThinkGeoCloudVectorMapsMapType.CustomizedByStyleJson;

                var jsonPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data/Json/thinkgeo-world-streets-cobalt.json");
                _thinkGeoVectorsOverlay.StyleJsonUri = new Uri(jsonPath, UriKind.Relative);

                _thinkGeoVectorsOverlay.VectorTileCache = new FileVectorTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorDark_VectorCache");
                _thinkGeoVectorsOverlay.TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorCobalt_RasterCache");
                await UpdateCancellationToken();
                await _thinkGeoVectorsOverlay.RefreshAsync(_cancellationTokenSource.Token);
                break;
        }
    }

    private async Task UpdateCancellationToken()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://cloud.thinkgeo.com/");
    }
}