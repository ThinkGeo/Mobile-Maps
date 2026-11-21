using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Forty-four forecast hours of NDFD sky cover, played as an animation over a grid
/// fill, with five palettes and a slider that scrubs the hours.
/// </summary>
/// <remarks>
/// The desktop sample reads each hour out of the GRIB with GdalGrid, band by band.
/// GDAL's natives are not published for iOS or Android, so the whole forecast is
/// converted once on the desktop - a byte per cell, since sky cover is a percentage -
/// and shipped as one flat file. The hours are still read one at a time, as the
/// desktop reads bands: turning all forty-four into float grids up front is five
/// million cells before anything is on screen, which a phone should not do and a
/// simulator will not finish.
/// </remarks>
public class NdfdSkyGridAnimation : SamplePage, IDisposable
{
    private static readonly string[] Palettes =
    {
        "Rainbow", "Gray", "Warm", "Cool", "Viridis",
    };

    private GpuBasemap _basemap;
    private MapStyle _style;
    private InMemoryGridSource _grid;
    private IReadOnlyList<ColorStop> _palette;
    private IDispatcherTimer _timer;

    private float[][,] _frames;
    private string[] _frameTimes;
    private MapView _map;
    private Label _timeText;
    private Slider _frameSlider;
    private bool _playing;
    private bool _loading;
    private int _wantedFrame;
    private int _rows;
    private int _columns;
    private long _bandsOffset;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        _style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        _basemap = new GpuBasemap(_style);
        map.Basemap = _basemap;
        map.CurrentExtent = new RectangleShape(-14_100_000, 6_800_000, -7_300_000, 2_500_000);

        // Two rows: when this frame is for, with the run of hours beside it, and the
        // palettes under that.
        //
        // No play button. It plays, and a finger on the slider is how you stop it -
        // which is the gesture anyone tries first anyway, and it costs no card. What
        // the button bought was a way to hold one hour still; letting go of the
        // slider gives that back a second and a half later, which is long enough to
        // read a frame and short enough not to feel stuck.
        _timeText = new Label
        {
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
        };

        _frameSlider = new Slider { Minimum = 0, Maximum = 1, Value = 0, IsEnabled = false };
        _frameSlider.ValueChanged += (_, e) => ShowFrame((int)e.NewValue);
        _frameSlider.DragStarted += (_, _) => _playing = false;
        _frameSlider.DragCompleted += (_, _) => _playing = true;

        var timeRow = new Grid { ColumnSpacing = 12 };
        timeRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        timeRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        timeRow.Children.Add(_timeText);
        timeRow.Children.Add(_frameSlider);
        Grid.SetColumn((BindableObject)_frameSlider, 1);
        AddToPanel(timeRow);

        AddChoiceStrip(Palettes, 0, index =>
        {
            _palette = CreatePalette(Palettes[index]);
            if (_grid == null) return;

            _style.RemoveGridFill(_grid).AddGridFill(_grid, new GridFillStyle { ColorStops = _palette });
            _ = _basemap.SetStyleAsync(_style);
        });

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(150);
        _timer.Tick += (_, _) =>
        {
            // A frame still being read holds the animation, so no hour is skipped.
            if (_playing && !_loading && _frames != null)
            {
                _frameSlider.Value = (_frameSlider.Value + 1) % _frames.Length;
            }
        };

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        await _map.RefreshAsync();

        try
        {
            await Task.Run(LoadForecast);
        }
        catch (Exception exception)
        {
            _timeText.Text = "Failed to load the NDFD grid: " + exception.Message;
            return;
        }

        _frames[0] = ReadBand(0);
        _grid = new InMemoryGridSource(_frames[0], Extent, Projection);
        _palette ??= CreatePalette("Weather Rainbow");
        _style.AddGridFill(_grid, new GridFillStyle { ColorStops = _palette });
        await _basemap.SetStyleAsync(_style);

        ShowTime(0);
        _frameSlider.Maximum = _frames.Length - 1;
        _frameSlider.IsEnabled = true;
        _playing = true;
        _timer.Start();
    }

    private RectangleShape Extent { get; set; }

    private Projection Projection { get; set; }

    // Dragging the slider asks for an hour; the one asked for last is read off the UI
    // thread and shown, and the hours dragged past on the way are skipped.
    private async void ShowFrame(int index)
    {
        if (_grid == null || _frames == null) return;

        _wantedFrame = Math.Max(0, Math.Min(_frames.Length - 1, index));
        if (_loading) return;

        _loading = true;
        try
        {
            int wanted;
            do
            {
                wanted = _wantedFrame;
                _frames[wanted] ??= await Task.Run(() => ReadBand(wanted));
            }
            while (wanted != _wantedFrame);

            _grid.SetValues(_frames[_wantedFrame]);
            ShowTime(_wantedFrame);
        }
        catch (Exception exception)
        {
            _timeText.Text = "Failed to read the forecast hour: " + exception.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    /// <summary>
    /// When this frame is for, and nothing else. "forecast hour 8 of 44" sat on a
    /// line of its own saying what the slider's own position already says.
    /// </summary>
    private void ShowTime(int index) => _timeText.Text = _frameTimes[index] ?? "(no time)";

    /// <summary>
    /// Reads the header - the grid's shape, the ground it covers, its projection, and
    /// the hour each band is valid for - and leaves the bands where they are.
    /// </summary>
    private void LoadForecast()
    {
        var path = Path.Combine(SampleShared.DataFolder, "Ndfd", "ndfd-sky.bin");
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "TGSK") throw new InvalidDataException("not a packed sky forecast");

        var bands = reader.ReadInt32();
        _rows = reader.ReadInt32();
        _columns = reader.ReadInt32();
        var minX = reader.ReadDouble();
        var minY = reader.ReadDouble();
        var maxX = reader.ReadDouble();
        var maxY = reader.ReadDouble();
        var projection = new string(reader.ReadChars(reader.ReadInt32()));

        Extent = new RectangleShape(minX, maxY, maxX, minY);
        Projection = new Projection(projection);

        _frameTimes = new string[bands];
        for (var band = 0; band < bands; band++)
        {
            var unixSeconds = reader.ReadInt64();
            _frameTimes[band] = unixSeconds == 0
                ? null
                : DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime
                    .ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);
        }

        _bandsOffset = stream.Position;
        _frames = new float[bands][,];
    }

    /// <summary>
    /// One forecast hour. Sky cover is a percentage, 1..100; 0 and anything above 100
    /// is no data, which the grid draws as nothing.
    /// </summary>
    private float[,] ReadBand(int index)
    {
        var values = new float[_rows, _columns];
        var row = new byte[_columns];

        using var stream = File.OpenRead(Path.Combine(SampleShared.DataFolder, "Ndfd", "ndfd-sky.bin"));
        stream.Seek(_bandsOffset + ((long)index * _rows * _columns), SeekOrigin.Begin);

        for (var y = 0; y < _rows; y++)
        {
            var read = 0;
            while (read < _columns)
            {
                var got = stream.Read(row, read, _columns - read);
                if (got <= 0) throw new EndOfStreamException();
                read += got;
            }

            for (var x = 0; x < _columns; x++)
            {
                var v = row[x];
                values[y, x] = v <= 0 || v > 100 ? float.NaN : v;
            }
        }

        return values;
    }

    private static IReadOnlyList<ColorStop> CreatePalette(string name)
    {
        var table = new GeoColor[101];
        for (var v = 1; v <= 100; v++)
        {
            var ratio = v / 100d;
            table[v] = name switch
            {
                "Gray" => Lerp(new[] { new byte[] { 200, 215, 230 }, new byte[] { 130, 145, 160 } }, ratio, (byte)(ratio * 210)),
                "Warm" => Lerp(new[] { new byte[] { 255, 245, 180 }, new byte[] { 255, 200, 0 }, new byte[] { 240, 100, 0 }, new byte[] { 180, 0, 0 } }, ratio, (byte)(40 + (ratio * 195))),
                "Cool" => Lerp(new[] { new byte[] { 190, 230, 255 }, new byte[] { 0, 190, 240 }, new byte[] { 0, 80, 200 }, new byte[] { 0, 10, 110 } }, ratio, (byte)(40 + (ratio * 195))),
                "Viridis" => Lerp(new[] { new byte[] { 68, 1, 84 }, new byte[] { 59, 82, 139 }, new byte[] { 33, 145, 140 }, new byte[] { 94, 201, 98 }, new byte[] { 253, 231, 37 } }, ratio, (byte)(50 + (ratio * 185))),
                _ => Lerp(new[] { new byte[] { 24, 64, 196 }, new byte[] { 0, 174, 239 }, new byte[] { 0, 186, 124 }, new byte[] { 246, 224, 52 }, new byte[] { 246, 143, 31 }, new byte[] { 214, 37, 35 } }, ratio, (byte)(48 + (ratio * 180))),
            };
        }

        // Ten stops over the percentage; the fill blends between them.
        var stops = new List<ColorStop> { new(1, table[1]) };
        for (var v = 10; v <= 100; v += 10)
        {
            stops.Add(new ColorStop(v, table[v]));
        }

        return stops;
    }

    private static GeoColor Lerp(byte[][] stops, double ratio, byte alpha)
    {
        var scaled = ratio * (stops.Length - 1);
        var lo = Math.Max(0, Math.Min(stops.Length - 1, (int)Math.Floor(scaled)));
        var hi = Math.Min(stops.Length - 1, lo + 1);
        var t = scaled - lo;
        return GeoColor.FromArgb(
            alpha,
            (byte)(stops[lo][0] + (t * (stops[hi][0] - stops[lo][0]))),
            (byte)(stops[lo][1] + (t * (stops[hi][1] - stops[lo][1]))),
            (byte)(stops[lo][2] + (t * (stops[hi][2] - stops[lo][2]))));
    }

    public void Dispose()
    {
        _playing = false;
        _timer?.Stop();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
