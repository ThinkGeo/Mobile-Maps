using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The time zone under a point, from the ThinkGeo Cloud, with the zone's own area
/// drawn on the map. Tap anywhere to ask again.
/// </summary>
public class Timezone : SamplePage
{
    private readonly DynamicSource _timezoneArea = new();
    private TimeZoneCloudClient _client;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddDynamic(_timezoneArea);
        map.Basemap = new GpuBasemap(style);

        _client = new TimeZoneCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };

        _status = AddStatusLabel();
        _status.MaxLines = 4;
        AddToPanel(new Label { Text = "Tap the map for another zone.", FontSize = 11, TextColor = Colors.Gray });

        // The desktop sample listens for a left click; a phone has a tap, and the
        // event hands over screen coordinates rather than a world point.
        map.SingleTap += (_, e) => _ = ShowZoneAsync(map.ToWorldCoordinate(e.X, e.Y));

        map.CenterPoint = new PointShape(-10618080, 4557170);
        map.MapScale = 33258550;
        _ = map.RefreshAsync();

        // Frisco, TX
        _ = ShowZoneAsync(new PointShape(-10779572.80, 3915268.68));
    }

    private async Task ShowZoneAsync(PointShape point)
    {
        CloudTimeZoneResult result;
        try
        {
            _status.Text = "asking the service...";
            // The point is in the map's Spherical Mercator (3857), so the service is told so.
            result = await _client.GetTimeZoneByCoordinateAsync(point.X, point.Y, 3857);
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
            return;
        }

        _status.Text = $"Time zone: {result.TimeZone}\n"
            + $"Local time: {result.CurrentLocalTime}\n"
            + $"Daylight saving time: {(result.DaylightSavingsActive ? "yes" : "no")}";

        _timezoneArea.Clear();
        if (result.Shape != null)
        {
            _timezoneArea.SetArea("zone", new Feature(result.Shape),
                new GeoColor(60, GeoColors.DodgerBlue), GeoColors.DodgerBlue, 2f);
        }

        await _map.RefreshAsync();
    }
}
