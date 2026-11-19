using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same map from several vector tile providers. A new provider is a new style
/// over the same map: the basemap is restyled in place and the camera holds.
/// </summary>
public class VectorMapProviders : SamplePage
{
    private static readonly (string Name, string StyleUri)[] Providers =
    {
        ("ThinkGeo Cloud - Light", null),
        ("ThinkGeo Cloud - Dark", null),
        ("OpenFreeMap - Liberty", "https://tiles.openfreemap.org/styles/liberty"),
        ("OpenFreeMap - Positron", "https://tiles.openfreemap.org/styles/positron"),
        ("OpenFreeMap - Dark", "https://tiles.openfreemap.org/styles/dark"),
    };

    private readonly ThinkGeoVectorTileSource _thinkGeo = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(-10779700, 3912000);
        map.MapScale = 18100;

        AddChoicePicker(Providers.Select(p => p.Name).ToList(), 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    private async Task ShowAsync(int index)
    {
        try
        {
            var provider = Providers[index];
            var style = provider.StyleUri == null
                ? new MapStyle(index == 0 ? ThinkGeoVectorStyles.Light : ThinkGeoVectorStyles.Dark, _thinkGeo)
                : new MapStyle(provider.StyleUri);

            if (_map.Basemap == null)
            {
                _map.Basemap = new GpuBasemap(style);
            }
            else
            {
                await _map.Basemap.SetStyleAsync(style);
            }

            _status.Text = provider.StyleUri ?? nameof(ThinkGeoVectorTileSource);
        }
        catch (Exception exception)
        {
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
