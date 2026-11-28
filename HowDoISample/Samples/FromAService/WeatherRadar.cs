using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The last half hour of American weather radar, looping - six images fetched once
/// and switched by visibility, so nothing is re-fetched between frames.
/// </summary>
public class WeatherRadar : SamplePage, IDisposable
{
    /// <summary>
    /// One frame of the composite, requested through the server's WMS at a size a
    /// phone can hold: 2048 x 906, already transparent where there is no echo. The
    /// same mosaic is also published as a flat file, at 12200 x 5400 - a quarter of a
    /// gigabyte once decoded, for each of six frames.
    /// </summary>
    private const string FrameUrlTemplate =
        "https://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0q.cgi?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap" +
        "&LAYERS={0}&STYLES=&FORMAT=image/png&TRANSPARENT=TRUE&SRS=EPSG:4326" +
        "&BBOX=-126,23,-65,50&WIDTH=2048&HEIGHT=906";

    private const int FrameCount = 6;   // 25 minutes ago .. now, in 5-minute steps

    /// <summary>The CONUS composite's ground extent, which the request above asks for.</summary>
    private static readonly RectangleShape ImageExtent = new(-126, 50, -65, 23);

    /// <summary>
    /// The server publishes each five-minute step as a layer of its own - the newest
    /// unsuffixed, the rest named by how far back they are - so a frame is a layer name
    /// rather than a time parameter to get right.
    /// </summary>
    private static string LayerFor(int minutesAgo) => minutesAgo == 0
        ? "nexrad-n0q-900913"
        : FormattableString.Invariant($"nexrad-n0q-900913-m{minutesAgo:00}m");

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
        _status = AddToPanel(new Label { FontSize = 12, TextColor = PanelInk, MaxLines = 2 });
        SetAttribution("Radar \u00a9 Iowa State Mesonet \u00b7 " + DefaultAttribution);

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
                var png = await HttpClient.GetByteArrayAsync(
                    string.Format(CultureInfo.InvariantCulture, FrameUrlTemplate, LayerFor(minutesAgo)));

                // Transparent where there is no echo already, so there is no colour to
                // knock out: the WMS answers in RGBA.
                _frames[i] = new GeoImageRasterTileSource(png, ImageExtent, new Projection(4326));
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
            ? "Radar: now"
            : FormattableString.Invariant(
                $"Radar: {minutesAgo} min ago");
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
