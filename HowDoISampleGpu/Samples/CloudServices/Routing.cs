using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Driving directions from the ThinkGeo Cloud: a route through five stops in the
/// order given, the same five in the shortest order, and how far one can drive from
/// a point in fifteen minutes to an hour.
/// </summary>
public class Routing : SamplePage
{
    private static readonly Collection<PointShape> Stops = new()
    {
        new PointShape(-10776986.85, 3908680.24),
        new PointShape(-10776836.12, 3912348.04),
        new PointShape(-10778917.01, 3909965.17),
        new PointShape(-10779631.80, 3915721.82),
        new PointShape(-10774900.61, 3912552.36),
    };

    private static readonly TimeSpan[] TravelTimes =
    {
        TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30),
        TimeSpan.FromMinutes(45), TimeSpan.FromMinutes(60),
    };

    private static readonly GeoColor[] TravelTimeColors =
        { GeoColors.Green, GeoColors.Yellow, GeoColors.Orange, GeoColors.Red };

    private static readonly string[] Modes =
    {
        "Route through the stops in order",
        "Route in the shortest order",
        "How far in 15 to 60 minutes",
    };

    // The highlight is added after the result, so a picked segment draws over its route.
    private readonly DynamicSource _result = new();
    private readonly DynamicSource _highlight = new();

    private RoutingCloudClient _client;
    private MapView _map;
    private Label _status;
    private int _mode;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.Images.Add("star", new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple));
        style.AddDynamic(_result);
        style.AddDynamic(_highlight);
        map.Basemap = new GpuBasemap(style);

        map.CenterPoint = new PointShape(-10778720, 3915154);
        map.MapScale = 202090;

        _client = new RoutingCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };

        AddChoicePicker(Modes, 0, index => { _mode = index; _ = RunAsync(); });
        _status = AddToPanel(new Label { FontSize = 12, MaxLines = 3 });
        AddToPanel(new Label
        {
            Text = "In the third mode, tap the map to move the centre.",
            FontSize = 11, TextColor = Colors.Gray,
        });

        map.SingleTap += (_, e) =>
        {
            if (_mode == 2) _ = RunAsync(map.ToWorldCoordinate(e.X, e.Y));
        };

        _ = map.RefreshAsync();
        _ = RunAsync();
    }

    private async Task RunAsync(PointShape serviceAreaCentre = null)
    {
        _result.Clear();
        _highlight.Clear();
        _status.Text = "asking the service…";

        try
        {
            switch (_mode)
            {
                case 0:
                {
                    ShowStopsWhileWaiting();
                    var result = await _client.GetRouteAsync(Stops, 3857,
                        new CloudRoutingGetRouteOptions { TurnByTurn = true });
                    if (_mode != 0) return;
                    if (result.Exception != null) { _status.Text = result.Exception.Message; break; }

                    var labels = result.RouteResult.Waypoints.Select((_, i) => i == 0 ? "Start" : "Stop " + i).ToList();
                    DrawRoute(result.RouteResult.Routes,
                        result.RouteResult.Waypoints.Select(w => new PointShape(w.Coordinate)).ToList(), labels);
                    break;
                }

                case 1:
                {
                    ShowStopsWhileWaiting();

                    // Any stop may come first, and the route comes back to where it started.
                    var options = new CloudRoutingOptimizationOptions
                    {
                        TurnByTurn = true,
                        Source = CloudRoutingTspFixSourceCoordinate.Any,
                        Destination = CloudRoutingTspFixDestinationCoordinate.Any,
                        Roundtrip = true,
                    };
                    var result = await _client.GetOptimizedRouteAsync(Stops, 3857, options);
                    if (_mode != 1) return;
                    if (result.Exception != null) { _status.Text = result.Exception.Message; break; }

                    // The optimizer answers with a TspResult rather than a RouteResult:
                    // the stops come back in the order it chose.
                    var labels = result.TspResult.Waypoints.Select((_, i) => "Stop " + (i + 1)).ToList();
                    DrawRoute(result.TspResult.Routes,
                        result.TspResult.Waypoints.Select(w => new PointShape(w.Coordinate)).ToList(), labels);
                    break;
                }

                default:
                {
                    var centre = serviceAreaCentre ?? _map.CenterPoint;
                    var result = await _client.GetServiceAreaAsync(centre, 3857, TravelTimes);
                    if (_mode != 2) return;
                    if (result.Exception != null) { _status.Text = result.Exception.Message; break; }

                    // Longest first, so the shorter ones draw inside them. A service
                    // area is a shape, not a wrapper around one.
                    var areas = result.ServiceAreaResult.ServiceAreas.ToList();
                    for (var i = areas.Count - 1; i >= 0; i--)
                    {
                        var colour = TravelTimeColors[Math.Min(i, TravelTimeColors.Length - 1)];
                        _result.SetArea("area-" + i, new Feature(areas[i]), new GeoColor(60, colour), colour, 2f);
                    }

                    _result.SetPoint("centre", new Feature(centre), sizeInPixel: 24f, iconName: "star");
                    _status.Text = "15, 30, 45 and 60 minutes by road from the star";
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private void ShowStopsWhileWaiting()
    {
        for (var i = 0; i < Stops.Count; i++)
        {
            _result.SetPoint("stop-" + i, new Feature(Stops[i]), sizeInPixel: 24f, iconName: "star");
        }
    }

    private void DrawRoute(IEnumerable<CloudRoutingRoute> routes, IList<PointShape> stops, IList<string> labels)
    {
        var routeLines = new List<BaseShape>();
        var distance = 0d;
        var duration = TimeSpan.Zero;
        foreach (var route in routes)
        {
            routeLines.Add(route.Shape);
            distance += route.Distance;
            duration += route.Duration;
        }

        // The marker and the label of a stop share its id: one feature, drawn two ways.
        _result.Clear();
        for (var i = 0; i < routeLines.Count; i++)
        {
            _result.SetLine("route-" + i, new Feature(routeLines[i]), GeoColors.MediumPurple, 3f);
        }

        for (var i = 0; i < stops.Count; i++)
        {
            var stop = new Feature(stops[i]);
            stop.ColumnValues["Label"] = labels[i];
            _result.SetPoint("stop-" + i, stop, sizeInPixel: 24f, iconName: "star");
            _result.SetLabel("stop-" + i, stop, "Label", sizeInPixel: 15f, offsetYInPixel: -20f);
        }

        _status.Text = FormattableString.Invariant($"{distance / 1609.344:0.0} miles · {duration:hh\\:mm\\:ss}");
    }
}
