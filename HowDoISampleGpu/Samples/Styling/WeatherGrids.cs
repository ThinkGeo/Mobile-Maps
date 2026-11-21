using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Two National Digital Forecast Database grids over the same country: forty-four
/// forecast hours of sky cover played as an animation over a grid fill, and the
/// surface wind drawn as sixty-five thousand particles advected along the field.
/// </summary>
/// <remarks>
/// One sample rather than two, because the two halves differ in one line each - the
/// style registration - and are otherwise the same story: a forecast grid prepared
/// off the device and read as numbers.
/// <para>
/// The desktop samples read the NDFD GRIB and GeoTIFFs with GdalGrid. GDAL's natives
/// are not published for iOS or Android, so each forecast is converted once on the
/// desktop and shipped as one flat file - a byte per cell for sky cover, which is a
/// percentage - which is what a phone application does anyway. The sky hours are
/// still read one at a time, as the desktop reads bands: turning all forty-four into
/// float grids up front is five million cells before anything is on screen.
/// </para>
/// </remarks>
public class WeatherGrids : SamplePage, IDisposable
{
    /// <summary>The ground both forecasts cover.</summary>
    private static readonly RectangleShape Conus =
        new(-14_100_000, 6_800_000, -7_300_000, 2_500_000);

    private GpuBasemap _basemap;
    private MapStyle _style;

    // Sky cover: the hours, read as they are asked for.
    private InMemoryGridSource _grid;
    private IReadOnlyList<ColorStop> _palette;
    private IDispatcherTimer _timer;
    private float[][,] _frames;
    private string[] _frameTimes;
    private bool _playing;
    private bool _loading;
    private int _wantedFrame;
    private int _rows;
    private int _columns;
    private long _bandsOffset;

    // Wind: one hour, as a field the particles are carried along.
    private VectorFieldSource _field;

    private MapView _map;
    private Label _timeText;
    private Slider _frameSlider;
    private View _hoursRow;
    private bool _showingWind;

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
        OpenOn(map, Conus);

        // Two cells and, for the hours, one row under them. The palette chooser that
        // used to sit here as a second strip is gone: two strips stacked read as a
        // settings page, and a weather grid has one palette anyone expects.
        //
        // No play button either. It plays, and a finger on the slider stops it, which
        // is the gesture anyone tries first.
        AddChoiceStrip(new[] { "Sky cover", "Wind" }, 0, index => _ = ShowAsync(index == 1));

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

        var hours = new Grid { ColumnSpacing = 12 };
        hours.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        hours.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        hours.Children.Add(_timeText);
        hours.Children.Add(_frameSlider);
        Grid.SetColumn((BindableObject)_frameSlider, 1);
        _hoursRow = AddToPanel(hours);

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

        _ = ShowAsync(false);
    }

    /// <summary>
    /// Puts one of the two forecasts on the map and takes the other off. Each is read
    /// from its file the first time it is asked for, so opening the sample costs one
    /// of them rather than both.
    /// </summary>
    private async Task ShowAsync(bool wind)
    {
        _showingWind = wind;
        _hoursRow.IsVisible = !wind;

        try
        {
            if (wind)
            {
                _playing = false;
                _timer.Stop();
                if (_grid != null) _style.RemoveGridFill(_grid);

                if (_field == null) await Task.Run(LoadField);
                _style.AddParticleFlow(_field);
                await _basemap.SetStyleAsync(_style);
                _timeText.Text = string.Empty;
                return;
            }

            if (_field != null) _style.RemoveParticleFlow(_field);

            if (_grid == null)
            {
                await Task.Run(LoadForecast);
                _frames[0] = ReadBand(0);
                _grid = new InMemoryGridSource(_frames[0], Extent, Projection);
                _palette ??= CreatePalette("Weather Rainbow");
                _frameSlider.Maximum = _frames.Length - 1;
                _frameSlider.IsEnabled = true;
            }

            _style.AddGridFill(_grid, new GridFillStyle { ColorStops = _palette });
            await _basemap.SetStyleAsync(_style);

            ShowTime((int)_frameSlider.Value);
            _playing = true;
            _timer.Start();
        }
        catch (Exception exception)
        {
            _timeText.Text = "Failed to load the forecast: " + exception.Message;
        }
    }

    private RectangleShape Extent { get; set; }

    private Projection Projection { get; set; }

    // Dragging the slider asks for an hour; the one asked for last is read off the UI
    // thread and shown, and the hours dragged past on the way are skipped.
    private async void ShowFrame(int index)
    {
        if (_grid == null || _frames == null || _showingWind) return;

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

    /// <summary>
    /// Reads the packed forecast hour of speed and direction into one
    /// VectorFieldSource, in the grid's own Lambert projection.
    /// </summary>
    private void LoadField()
    {
        using var stream = File.OpenRead(Path.Combine(SampleShared.DataFolder, "Ndfd", "ndfd-wind.bin"));
        using var reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "TGWF") throw new InvalidDataException("not a packed wind field");

        var rows = reader.ReadInt32();
        var columns = reader.ReadInt32();
        var minX = reader.ReadDouble();
        var minY = reader.ReadDouble();
        var maxX = reader.ReadDouble();
        var maxY = reader.ReadDouble();
        var projection = new string(reader.ReadChars(reader.ReadInt32()));

        var speed = ReadGrid(reader, rows, columns);
        var from = ReadGrid(reader, rows, columns);

        // A meteorological wind direction is where the wind comes FROM; the arrow
        // points where it is going.
        var flow = new FlowVector[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                flow[row, column] = FlowVector.FromSpeedDirection(
                    speed[row, column], (from[row, column] + 180f) % 360f);
            }
        }

        _field = new VectorFieldSource(flow,
            new RectangleShape(minX, maxY, maxX, minY),
            new Projection(projection));
    }

    private static float[,] ReadGrid(BinaryReader reader, int rows, int columns)
    {
        var values = new float[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                values[row, column] = reader.ReadSingle();
            }
        }

        return values;
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
