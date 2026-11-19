using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Addresses to places and back: search for a place by name, or tap the map and ask
/// what is there.
/// </summary>
public class Geocoding : SamplePage
{
    private readonly DynamicSource _searchedArea = new();
    private readonly DynamicSource _selectedResult = new();

    // The map is in Spherical Mercator (3857); people read and type latitude and
    // longitude (4326).
    private readonly ProjectionConverter _latLon = new(3857, 4326);

    private GeocodingCloudClient _geocoding;
    private ReverseGeocodingCloudClient _reverseGeocoding;
    private MapView _map;
    private Entry _search;
    private Label _status;
    private Label _results;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.Images.Add("center", new PointStyle(PointSymbolType.Cross, 20, GeoBrushes.Red));
        style.Images.Add("match", new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple));
        style.AddDynamic(_searchedArea);
        style.AddDynamic(_selectedResult);
        map.Basemap = new GpuBasemap(style);

        map.CenterPoint = new PointShape(-10778720, 3915154);
        map.MapScale = 202090;

        _geocoding = new GeocodingCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };
        _reverseGeocoding = new ReverseGeocodingCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };
        _latLon.Open();

        _search = new Entry { Placeholder = "An address or a place", Text = "Frisco City Hall", FontSize = 14 };
        _search.Completed += (_, _) => _ = SearchAsync();
        AddToPanel(_search);

        var go = new Button { Text = "Search", FontSize = 12, Padding = new Thickness(8, 4) };
        go.Clicked += (_, _) => _ = SearchAsync();
        AddToPanel(go);

        _status = AddStatusLabel();
        _results = AddToPanel(new Label { FontSize = 11, FontFamily = "Courier", MaxLines = 5 });
        AddToPanel(new Label
        {
            Text = "Tap the map to ask what is there instead.",
            FontSize = 11, TextColor = Colors.Gray,
        });

        map.SingleTap += (_, e) => _ = ReverseAsync(map.ToWorldCoordinate(e.X, e.Y));

        _ = map.RefreshAsync();
        _ = SearchAsync();
    }

    private async Task SearchAsync()
    {
        var text = _search.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            _status.Text = "Type an address or a place to search for.";
            return;
        }

        _status.Text = "searching…";
        try
        {
            var result = await _geocoding.SearchAsync(text, new CloudGeocodingOptions
            {
                MaxResults = 5,
                ResultProjectionInSrid = 3857,
            });

            _searchedArea.Clear();
            _selectedResult.Clear();

            if (result.Exception != null)
            {
                _status.Text = result.Exception.Message;
                return;
            }

            _status.Text = $"Found {result.Locations.Count} matching locations.";
            _results.Text = string.Join("\n", result.Locations.Take(5).Select(l => l.LocationName));

            var first = result.Locations.FirstOrDefault();
            if (first?.LocationPoint != null)
            {
                _selectedResult.SetPoint("match", new Feature(first.LocationPoint), sizeInPixel: 24f, iconName: "match");
                _map.CenterPoint = first.LocationPoint;
                _map.MapScale = 20000;
            }
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private async Task ReverseAsync(PointShape point)
    {
        const int searchRadius = 400;

        // The circle the service is asked to look inside, drawn before the answer
        // comes back so the tap is acknowledged at once.
        _searchedArea.Clear();
        _searchedArea.SetArea("searched", new Feature(new EllipseShape(point, searchRadius)),
            new GeoColor(10, GeoColors.DodgerBlue), new GeoColor(100, GeoColors.DodgerBlue), 2f);
        _searchedArea.SetPoint("centre", new Feature(point), sizeInPixel: 20f, iconName: "center");
        _selectedResult.Clear();
        await _map.RefreshAsync();

        var latLon = _latLon.ConvertToExternalProjection(point.X, point.Y);
        _status.Text = FormattableString.Invariant($"asking about {latLon.Y:0.000000}, {latLon.X:0.000000}…");

        try
        {
            var result = await _reverseGeocoding.SearchPointAsync(
                point.X, point.Y, 3857, searchRadius, DistanceUnit.Meter,
                new CloudReverseGeocodingOptions { MaxResults = 5 });

            if (result.Exception != null)
            {
                _status.Text = result.Exception.Message;
                return;
            }

            // A reverse answer is a best match plus whatever else is nearby, not one
            // flat list - so the best match leads and the neighbours follow it.
            var nearby = result.NearbyLocations ?? new System.Collections.ObjectModel.Collection<CloudReverseGeocodingLocation>();
            _status.Text = result.BestMatchLocation?.LocationName ?? $"{nearby.Count} within {searchRadius} m";
            _results.Text = string.Join("\n", nearby.Take(5).Select(
                l => FormattableString.Invariant($"{l.LocationName}  ({l.DistanceFromQueryFeature:0} m {l.DirectionFromQueryFeature})")));
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
