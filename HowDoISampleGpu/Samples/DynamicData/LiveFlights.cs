using System.Text.Json;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Every aircraft airborne right now, from OpenSky, each one dead-reckoned forward
/// from its last report at fifty frames a second - thousands of markers rewritten
/// per frame with no tile cut.
/// </summary>
public class LiveFlights : SamplePage, IDisposable
{
    private const string OpenSkyUrl = "https://opensky-network.org/api/states/all";
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(20) };

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
    private double _speed = 1;

    protected override async void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();
        _status = AddToPanel(new Label { FontSize = 12, MaxLines = 3 });

        var speed = new Slider { Minimum = 1, Maximum = 60, Value = 1 };
        speed.ValueChanged += (_, e) => _speed = e.NewValue;
        AddToPanel(speed);

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
        _timer.Interval = TimeSpan.FromMilliseconds(50);
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
    /// Each aircraft moved along its own heading by however long the last frame took.
    /// Nothing is fetched again: the sky is dead-reckoned from one report.
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

            if (flight.Lat > 85 || flight.Lat < -85)
            {
                _tracks.Remove(flight.Id);
                continue;
            }

            if (flight.Lon > 180) flight.Lon -= 360;
            if (flight.Lon < -180) flight.Lon += 360;

            _tracks.SetPoint(flight.Id, new Feature(flight.Lon, flight.Lat),
                flight.TrackDegrees, ColorForAltitude(flight.AltitudeMeters), 11f);
        }

        _ = _map.RefreshAsync();
    }

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
