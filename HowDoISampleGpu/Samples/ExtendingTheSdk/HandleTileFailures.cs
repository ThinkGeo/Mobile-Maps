using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a map does when a tile cannot be loaded. The raster source's key is wrong on
/// purpose, so the service answers 401 for every tile and the handler decides what
/// the map shows in its place.
/// </summary>
public class HandleTileFailures : SamplePage
{
    private ThinkGeoRasterTileSource _aerial;
    private Label _count;
    private Label _failure;
    private int _failures;
    private int _choice;   // 0 = default picture, 1 = nothing, 2 = nothing but ask again

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(
            new[] { "Draw the default picture", "Leave the tile out", "Leave it out and ask again" }, 0,
            index =>
            {
                _choice = index;
                // The source forgets its tiles and asks for them again, so they fail
                // under the new choice.
                _aerial?.Refresh();
            });
        _count = AddStatusLabel();
        _failure = AddToPanel(new Label { FontSize = 11, TextColor = Colors.Firebrick, MaxLines = 3 });

        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });

        // The key is wrong on purpose: the service answers 401 for every tile.
        _aerial = new ThinkGeoRasterTileSource("not-a-valid-key", ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1);
        style.AddRaster(_aerial, sourceId: "aerial", slot: StyleLayerSlot.AboveFills);

        var basemap = new GpuBasemap(style);
        basemap.TileLoadFailed += (_, e) =>
        {
            e.ShowOnMap = _choice == 0;
            e.Retry = _choice == 2;
            _failures++;
            _count.Text = _failures == 1 ? "1 tile request failed" : $"{_failures} tile requests failed";
            _failure.Text = $"{e.Kind} tile {e.Zoom}/{e.X}/{e.Y} of \"{e.SourceName}\": {e.Exception.Message}";
        };

        map.Basemap = basemap;
        map.CenterPoint = new PointShape(-10777290, 3908740);
        map.MapScale = 36000;
        _ = map.RefreshAsync();
    }
}
