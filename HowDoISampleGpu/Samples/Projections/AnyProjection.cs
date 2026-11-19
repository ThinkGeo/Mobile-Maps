using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The world drawn in any projection a proj string can describe - the map morphs
/// from one to the next without the camera moving.
/// </summary>
public class AnyProjection : SamplePage
{
    private sealed record Choice(string Name, string Definition, RectangleShape InitialView = null);

    private static readonly Choice[] Choices =
    {
        new("Web Mercator", "3857"),
        new("Equirectangular", "4326"),
        new("Robinson", "+proj=robin +datum=WGS84 +units=m +no_defs"),
        new("Mollweide", "+proj=moll +datum=WGS84 +units=m +no_defs"),
        new("Eckert IV", "+proj=eck4 +datum=WGS84 +units=m +no_defs"),
        new("Cylindrical equal-area", "+proj=cea +datum=WGS84 +units=m +no_defs"),
        new("Sinusoidal", "+proj=sinu +lon_0=0 +datum=WGS84 +units=m +no_defs"),
        new("Bonne", "+proj=bonne +lat_1=45 +lon_0=0 +datum=WGS84 +units=m +no_defs"),
        // A projection made for one region: outside it the shape is wrong, which is
        // the point of showing it with a view of its own.
        new("Albers (North America)",
            "+proj=aea +lat_1=29.5 +lat_2=45.5 +lat_0=23 +lon_0=-96 +datum=NAD83 +units=m +no_defs",
            new RectangleShape(-170, 72, -52, 12)),
        new("UTM zone 55S (Australia)",
            "+proj=utm +zone=55 +south +datum=WGS84 +units=m +no_defs",
            new RectangleShape(128, -9, 136, -44)),
    };

    private const double WorldHalfSize = 20037508.342789244;
    private static readonly TimeSpan MorphDuration = TimeSpan.FromMilliseconds(900);
    private static readonly ProjectionConverter LatLonToMercator = new(4326, 3857);

    private MapView _map;
    private Label _status;
    private Choice _choice = Choices[0];

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(Choices.Select(c => c.Name).ToList(), 0, index => _ = ShowAsync(Choices[index]));
        _status = AddStatusLabel();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        FrameWorld(_choice);
        _status.Text = _choice.Name;
        _ = map.RefreshAsync();
    }

    private async Task ShowAsync(Choice choice)
    {
        if (choice.Definition == _choice.Definition) return;
        _choice = choice;

        var target = choice.Definition switch
        {
            "3857" => DisplayProjection.WebMercator,
            "4326" => DisplayProjection.FromProjString("+proj=longlat +datum=WGS84 +no_defs"),
            _ => DisplayProjection.FromProjString(choice.Definition),
        };

        FrameWorld(choice);
        await _map.RefreshAsync();

        try
        {
            // The camera does not move: the screen centre keeps the same ground point
            // and only the shape under it changes.
            await _map.Basemap.AnimateDisplayProjectionAsync(target, MorphDuration);
            _status.Text = choice.Name;
        }
        catch (OperationCanceledException)
        {
            // Another projection was picked before this morph finished.
        }
    }

    /// <summary>
    /// A projection made for one region opens on that region; a world projection
    /// opens on the world. Regions are written in degrees, because that is how a
    /// proj string's own bounds are published - they are converted here.
    /// </summary>
    private void FrameWorld(Choice choice)
    {
        if (choice.InitialView == null)
        {
            _map.CurrentExtent = new RectangleShape(-WorldHalfSize, WorldHalfSize, WorldHalfSize, -WorldHalfSize);
            return;
        }

        LatLonToMercator.Open();
        _map.CurrentExtent = (RectangleShape)LatLonToMercator.ConvertToExternalProjection(choice.InitialView);
    }
}
