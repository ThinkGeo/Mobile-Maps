using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The Swiss national map from swisstopo's WMTS server as the basemap, read through
/// the server's web-mercator tile matrix set.
/// </summary>
public class WMTS : SamplePage
{
    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;
        _ = BuildMapAsync(map);
    }

    private static async Task BuildMapAsync(MapView map)
    {
        // The source is the data level: opening it negotiates the server's
        // capabilities, and TileMatrixSetName picks the web-mercator grid.
        var source = new WmtsRasterTileSource(new Uri("https://wmts.geo.admin.ch/EPSG/3857/1.0.0"), WmtsServerEncodingType.Restful)
        {
            ActiveLayerName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale",
            ActiveStyleName = "default",
            OutputFormat = "image/png",
            TileMatrixSetName = "3857_19",
        };

        // One call puts the service on the map - no style JSON is written or needed.
        map.Basemap = new GpuBasemap(new MapStyle().AddRaster(source, sourceId: "wmts"));

        // Fit the view to the extent the server declares for this layer.
        // (This public server can be slow.)
        await source.OpenAsync();
        var boundingBox = source.GetBoundingBox();
        map.CenterPoint = boundingBox.GetCenterPoint();
        map.MapScale = MapUtil.GetScale(map.MapUnit, boundingBox, map.CanvasWidth, map.CanvasHeight);
        await map.RefreshAsync();
    }
}
