using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A WFS V2 service merged into the basemap's style and drawn by the renderer -
/// the Dutch national cadastral parcels, from PDOK.
/// </summary>
public class WFS : SamplePage
{
    private const string ParcelStyle = """
    {
      "layers": [
        { "type": "line",
          "source-layer": "parcels",
          "paint": { "line-color": "#E05C2B", "line-width": 1.2 } }
      ]
    }
    """;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // Below zoom 15 no tile asks the service: a tile there would be a bounding-box
        // query for tens of thousands of parcels.
        var parcels = new FeatureSourceVectorTileSource { MinDataZoom = 15 };

        // The service's own CRS is EPSG:28992, but it also publishes EPSG:3857 - ask it
        // for web mercator rather than converting afterwards: the server holds the datum
        // transformation for its national grid, and a generic client-side conversion
        // lands the parcels about 170 m off the basemap.
        parcels.FeatureSources.Add("parcels", new WfsV2AsyncFeatureSource(
            "https://service.pdok.nl/kadaster/kadastralekaart/wfs/v5_0",
            "kadastralekaart:Perceel")
        {
            TimeoutInSeconds = 120,
            Crs = "urn:ogc:def:crs:EPSG::3857",
        });

        var basemapTiles = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, basemapTiles);
        style.AddStyle(ParcelStyle, parcels);
        map.Basemap = new GpuBasemap(style);

        map.CenterPoint = new PointShape(544950, 6867650);
        map.MapScale = 4000;
        _ = map.RefreshAsync();
    }
}
