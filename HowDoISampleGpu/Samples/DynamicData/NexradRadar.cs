using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The last half hour of American weather radar, looping - six images fetched once
/// and switched by visibility, so nothing is re-fetched between frames.
/// </summary>
public class NexradRadar : SamplePage, IDisposable
{
    private const string FrameUrlTemplate = "https://mesonet.agron.iastate.edu/data/gis/images/4326/USCOMP/n0q_{0}.png";
    private const int FrameCount = 6;   // n0q_5 (25 minutes ago) .. n0q_0 (now), 5-minute steps

    /// <summary>The published CONUS composite: 0.005-degree pixels from (-126, 50) down to (-65, 23).</summary>
    private static readonly RectangleShape ImageExtent = new(-126, 50, -65, 23);

    private static readonly HttpClient HttpClient = new();

    private readonly GeoImageRasterTileSource[] _frames = new GeoImageRasterTileSource[FrameCount];
    private IDispatcherTimer _timer;
    private MapStyle _style;
    private MapView _map;
    private Label _status;
    private int _frameIndex;

    protected override async void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();
        _status = AddToPanel(new Label { FontSize = 12, MaxLines = 2 });

        try
        {
            // One source and one layer per frame, oldest first. Stepping the loop only
            // switches which layer is visible: a frame's tiles stay loaded, so nothing
            // is fetched again and the map never shows a gap between two frames.
            _style = new MapStyle();
            _style.AddStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            });

            for (var i = 0; i < FrameCount; i++)
            {
                var minutesAgo = (FrameCount - 1 - i) * 5;
                _status.Text = FormattableString.Invariant($"Loading the radar, {i + 1} of {FrameCount}…");
                var png = await HttpClient.GetByteArrayAsync(string.Format(FrameUrlTemplate, minutesAgo / 5));

                // The mosaic is a black-backed palette PNG; black is its "no echo".
                _frames[i] = new GeoImageRasterTileSource(png, ImageExtent, new Projection(4326))
                {
                    TransparentColor = GeoColors.Black,
                };
                _style.AddRaster(_frames[i], new RasterPaint(0.75f), FrameId(i));
                _style.SetLayerVisibility(FrameId(i), i == FrameCount - 1);
            }

            map.Basemap = new GpuBasemap(_style);
            map.CurrentExtent = new RectangleShape(-14026255, 6446275, -7235766, 2632018);
            _frameIndex = FrameCount - 1;
            ShowStatus(_frameIndex);
            await map.RefreshAsync();

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(700);
            _timer.Tick += (_, _) => ShowNextFrame();
            _timer.Start();
        }
        catch (Exception exception)
        {
            _status.Text = "Radar unavailable: " + exception.Message.Split('\n')[0];
            await map.RefreshAsync();
        }
    }

    private void ShowNextFrame()
    {
        var next = (_frameIndex + 1) % FrameCount;
        _style.SetLayerVisibility(FrameId(_frameIndex), false);
        _style.SetLayerVisibility(FrameId(next), true);
        _frameIndex = next;
        ShowStatus(next);
        _ = _map.RefreshAsync();
    }

    private void ShowStatus(int frame)
    {
        var minutesAgo = (FrameCount - 1 - frame) * 5;
        _status.Text = minutesAgo == 0
            ? "NEXRAD composite, courtesy of Iowa State Mesonet - now"
            : FormattableString.Invariant(
                $"NEXRAD composite, courtesy of Iowa State Mesonet - {minutesAgo} minutes ago");
    }

    private static string FrameId(int frame) => FormattableString.Invariant($"radar-{frame}");

    /// <summary>
    /// The timer outlives the page unless it is stopped, and six decoded mosaics are
    /// worth releasing on a phone.
    /// </summary>
    public void Dispose()
    {
        _timer?.Stop();
        foreach (var frame in _frames) frame?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
