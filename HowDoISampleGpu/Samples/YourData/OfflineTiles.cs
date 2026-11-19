using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A map with no network at all: tiles read from an archive file on the device.
/// MBTiles and PMTiles, vector and raster - the renderer never learns which.
/// </summary>
public class OfflineTiles : SamplePage
{
    private static readonly string[] Archives =
    {
        "MBTiles - vector (the world)",
        "PMTiles - vector (Frisco)",
        "MBTiles - raster",
        "PMTiles - raster (Stamen Toner)",
    };

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        AddChoicePicker(Archives, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    /// <summary>
    /// A different archive is a different basemap - nothing carries over - so each
    /// choice is a new one, opened at the archive's own extent.
    /// </summary>
    private async Task ShowAsync(int index)
    {
        try
        {
            var (name, style, extent) = await OpenAsync(index);
            _map.Basemap = new GpuBasemap(style);
            _map.CurrentExtent = extent;
            _status.Text = name;
        }
        catch (Exception exception)
        {
            // Without the archive there is nothing to draw, and saying so beats an
            // empty window.
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    /// <summary>The line that differs: which class reads which archive.</summary>
    private static async Task<(string Name, MapStyle Style, RectangleShape Extent)> OpenAsync(int index)
    {
        switch (index)
        {
            case 0:
            {
                var tiles = new MbTilesVectorTileSource(Data("Mbtiles", "maplibre.mbtiles"));
                await tiles.OpenAsync();
                return (nameof(MbTilesVectorTileSource),
                    new MapStyle(File.ReadAllText(Data("Mbtiles", "style.json")), tiles),
                    tiles.GetBoundingBox());
            }

            case 1:
            {
                var tiles = new PmTilesVectorTileSource(Data("Pmtiles", "frisco.pmtiles"));
                await tiles.OpenAsync();
                return (nameof(PmTilesVectorTileSource),
                    new MapStyle(File.ReadAllText(Data("Pmtiles", "style.json")), tiles),
                    tiles.GetBoundingBox());
            }

            // The raster archives are opened before they are handed over: open is where
            // they read their own zoom range, so the renderer stretches the last level
            // instead of asking for levels the file does not have.
            case 2:
            {
                var tiles = new MbTilesRasterTileSource(Data("Mbtiles", "test.mbtiles"));
                await tiles.OpenAsync();
                return (nameof(MbTilesRasterTileSource), new MapStyle().AddRaster(tiles), MaxExtents.SphericalMercator);
            }

            default:
            {
                var tiles = new PmTilesRasterTileSource(Data("Pmtiles", "stamen_toner.pmtiles"));
                await tiles.OpenAsync();
                return (nameof(PmTilesRasterTileSource), new MapStyle().AddRaster(tiles), MaxExtents.SphericalMercator);
            }
        }
    }

    private static string Data(params string[] parts)
        => Path.Combine(new[] { SampleShared.DataFolder }.Concat(parts).ToArray());
}
