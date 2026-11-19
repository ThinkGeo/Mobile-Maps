using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// NASA's near-real-time satellite imagery from a WMS server, laid into the basemap's
/// own layers rather than over them.
/// </summary>
public class WMS : SamplePage
{
    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        var imagery = new WmsRasterTileSource(new Uri("https://gibs.earthdata.nasa.gov/wms/epsg3857/best/wms.cgi"));
        imagery.Client.OutputFormat = "image/png";
        imagery.Client.ActiveLayerNames.Add("VIIRS_NOAA20_CorrectedReflectance_TrueColor");

        // Two days back: the newest day's mosaic can still be empty while it is assembled.
        imagery.Client.Parameters["TIME"] = DateTime.UtcNow.AddDays(-2).ToString(
            "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        // A server that needs credentials gets them on every request here, for example:
        // imagery.SendingHttpRequest += (_, e) => e.HttpRequestMessage.Headers.Authorization =
        //     new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-token");

        // The basemap's own layers first, then the imagery inserted into them - above
        // the fills, below the lines and labels. Nothing is reordered: the slot is found
        // by walking the layers the style already declared.
        var basemapTiles = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, basemapTiles);
        style.AddRaster(imagery, slot: StyleLayerSlot.AboveFills);
        map.Basemap = new GpuBasemap(style);

        // North America, at a scale where a day of weather is the subject.
        map.CenterPoint = new PointShape(-10500000, 4500000);
        map.MapScale = 25000000;
        _ = map.RefreshAsync();
    }
}
