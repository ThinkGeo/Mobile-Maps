using System.Text.Json;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// Every aircraft airborne right now, from OpenSky, each one dead-reckoned forward
/// from its last report at fifty frames a second - thousands of markers rewritten
/// per frame with no tile cut.
/// </summary>
public class LiveFlights : SamplePage, IDisposable
{
    private const string OpenSkyUrl = "https://opensky-network.org/api/states/all";
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(20) };

    private const double HalfWorld = 20037508.342789244;
    private const double EarthRadius = 6378137.0;

    /// <summary>
    /// How often the sky is moved on. Fifteen times a second is smooth for something
    /// crossing the screen in minutes, and leaves the frame between ticks to be drawn
    /// in - the whole sky was being rewritten twenty times a second before, and the
    /// map got sixteen frames in twenty-five seconds because there was no time left.
    /// </summary>
    private static readonly TimeSpan Tick = TimeSpan.FromMilliseconds(66);

    /// <summary>
    /// The most aircraft drawn at once. Every one that reported is kept and moved -
    /// that arithmetic is a few floats each - but only what the camera is looking at
    /// is handed to the map, because a marker off screen costs the same to build and
    /// upload as one on it. Ten thousand of them over a whole hemisphere is also a
    /// grey smear rather than a picture; over anywhere a phone is actually looking,
    /// the cap is not reached.
    /// </summary>
    private const int MaxDrawn = 2000;

    /// <summary>Degrees of sky kept beyond the view, so an aircraft arrives already moving.</summary>
    private const double ViewMarginDegrees = 2;

    private sealed class Flight
    {
        public string Id;
        public string Callsign;
        public double Lon;
        public double Lat;
        public float VelocityMetersPerSecond;
        public float TrackDegrees;
        public float AltitudeMeters;
    }

    private DynamicSource _tracks;
    private Flight[] _flights;
    private IDispatcherTimer _timer;
    private DateTime _lastTick;
    private MapView _map;
    private Label _status;
    /// <summary>
    /// How much faster than real time the sky moves, and where the slider opens.
    /// <para>
    /// At 1 the markers do move - they are rewritten fifteen times a second - but not
    /// anywhere you can see. This view is the width of the United States, about five
    /// and a half kilometres to the pixel on a phone, and an airliner covers a quarter
    /// of a kilometre a second: one pixel every twenty-three seconds. The animation was
    /// real and invisible, which is the worst of both. Run the clock faster and the same
    /// dead reckoning becomes something to watch; the positions drift from the real sky
    /// as it does, which is what the number on the slider is telling you.
    /// </para>
    /// </summary>
    private const double DefaultSpeed = 120;
    private const double MaxSpeed = 400;

    private double _speed = DefaultSpeed;

    protected override async void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        _status = AddToPanel(new Label { FontSize = 12, TextColor = PanelInk, MaxLines = 3 });

        AddSliderRow(new SliderSpec("Speed", 1, MaxSpeed, DefaultSpeed,
            value => _speed = value,
            value => FormattableString.Invariant($"Time x{value:0}")));

        // Coordinates arrive in WGS84 and the source converts.
        _tracks = new DynamicSource(new Projection(4326));
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddDynamic(_tracks);
        map.Basemap = new GpuBasemap(style);
        map.CurrentExtent = new RectangleShape(-14026255, 6446275, -7235766, 2632018);
        await map.RefreshAsync();

        _status.Text = "Fetching the sky…";
        try
        {
            _flights = ParseOpenSky(await HttpClient.GetStringAsync(OpenSkyUrl));
        }
        catch (Exception exception)
        {
            _status.Text = "OpenSky did not answer, so there are no aircraft to show. ("
                + exception.Message.Split('\n')[0] + ")";
            return;
        }

        if (_flights.Length == 0)
        {
            _status.Text = "OpenSky answered without any aircraft. Try again later.";
            return;
        }

        _status.Text = FormattableString.Invariant($"{_flights.Length} aircraft airborne right now");
        _lastTick = DateTime.UtcNow;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = Tick;
        _timer.Tick += (_, _) => Advance();
        _timer.Start();
    }

    private static Flight[] ParseOpenSky(string json)
    {
        using var document = JsonDocument.Parse(json);
        var flights = new List<Flight>(16384);
        var states = document.RootElement.GetProperty("states");
        if (states.ValueKind != JsonValueKind.Array) return flights.ToArray();

        foreach (var state in states.EnumerateArray())
        {
            // Indexes per the OpenSky API: 1 callsign, 5 lon, 6 lat, 7 baro altitude,
            // 8 on-ground, 9 velocity, 10 true track.
            var lon = state[5];
            var lat = state[6];
            var track = state[10];
            if (lon.ValueKind == JsonValueKind.Null || lat.ValueKind == JsonValueKind.Null
                || track.ValueKind == JsonValueKind.Null || state[8].GetBoolean())
            {
                continue;
            }

            flights.Add(new Flight
            {
                Id = "flight-" + flights.Count,
                Callsign = state[1].ValueKind == JsonValueKind.Null ? string.Empty : state[1].GetString().Trim(),
                Lon = lon.GetDouble(),
                Lat = lat.GetDouble(),
                AltitudeMeters = state[7].ValueKind == JsonValueKind.Null ? 0f : (float)state[7].GetDouble(),
                VelocityMetersPerSecond = state[9].ValueKind == JsonValueKind.Null ? 0f : (float)state[9].GetDouble(),
                TrackDegrees = (float)track.GetDouble(),
            });
        }

        return flights.ToArray();
    }

    /// <summary>
    /// Each aircraft moved along its own heading by however long the last frame took,
    /// and the ones the camera can see handed to the map. Nothing is fetched again:
    /// the sky is dead-reckoned from one report.
    /// <para>
    /// No RefreshAsync at the end, and that absence is the sample. A DynamicSource asks
    /// for the frame itself - the first change of a turn requests a render and queues
    /// one rebuild for all of them - so a marker that moved is drawn without the map
    /// being told anything. Asking as well cost the basemap: RefreshAsync starts a draw
    /// pass and cancels the one before it, and twenty of those a second cancelled the
    /// tile scene every time before it could be committed. The sky was fetched, the
    /// aircraft were placed, and the page sat on an empty background for as long as you
    /// left it - no error, because nothing had failed.
    /// </para>
    /// <para>
    /// The Clear and the calls that follow it land as one rebuild, so rewriting the
    /// drawn set each tick costs one push, not one per aircraft.
    /// </para>
    /// </summary>
    private void Advance()
    {
        var now = DateTime.UtcNow;
        var dt = (now - _lastTick).TotalSeconds * _speed;
        _lastTick = now;

        foreach (var flight in _flights)
        {
            var radians = flight.TrackDegrees * Math.PI / 180;
            var meters = flight.VelocityMetersPerSecond * dt;
            flight.Lat += meters * Math.Cos(radians) / 110540.0;
            flight.Lon += meters * Math.Sin(radians)
                / (111320.0 * Math.Max(0.2, Math.Cos(flight.Lat * Math.PI / 180)));

            if (flight.Lon > 180) flight.Lon -= 360;
            if (flight.Lon < -180) flight.Lon += 360;
        }

        var extent = _map.CurrentExtent;
        if (extent == null) return;

        var south = LatitudeOf(extent.LowerLeftPoint.Y) - ViewMarginDegrees;
        var north = LatitudeOf(extent.UpperRightPoint.Y) + ViewMarginDegrees;
        var west = LongitudeOf(extent.LowerLeftPoint.X) - ViewMarginDegrees;
        var east = LongitudeOf(extent.UpperRightPoint.X) + ViewMarginDegrees;
        var wholeWorld = east - west >= 360;

        _tracks.Clear();
        var drawn = 0;

        foreach (var flight in _flights)
        {
            if (drawn == MaxDrawn) break;
            if (flight.Lat < south || flight.Lat > north) continue;

            if (!wholeWorld)
            {
                // The window can straddle the 180th, and an aircraft's longitude is
                // always in -180..180: turn it into the window's own run of degrees
                // before comparing.
                var lon = flight.Lon;
                while (lon < west) lon += 360;
                if (lon > east) continue;
            }

            _tracks.SetPoint(flight.Id, new Feature(flight.Lon, flight.Lat),
                flight.TrackDegrees, ColorForAltitude(flight.AltitudeMeters), 11f);
            drawn++;
        }
    }

    private static double LongitudeOf(double x) => x / HalfWorld * 180;

    private static double LatitudeOf(double y)
        => ((2 * Math.Atan(Math.Exp(y / EarthRadius))) - (Math.PI / 2)) * 180 / Math.PI;

    private static GeoColor ColorForAltitude(float meters) => meters switch
    {
        < 3000f => GeoColor.FromArgb(235, 240, 150, 30),
        < 8000f => GeoColor.FromArgb(235, 70, 180, 120),
        _ => GeoColor.FromArgb(235, 60, 130, 230),
    };

    public void Dispose()
    {
        _timer?.Stop();
        GC.SuppressFinalize(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Dispose();
    }
}
