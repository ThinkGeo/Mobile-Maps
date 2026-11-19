using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// An OGC API - Features service merged into the basemap's style and drawn by the
/// renderer - Spain's national named places, from IGN.
/// </summary>
public class OGCAPIFeatureServer : SamplePage
{
    private const string PlacesStyle = """
    {
      "layers": [
        { "type": "circle",
          "source-layer": "places",
          "paint": { "circle-radius": 4, "circle-color": "#FFC857", "circle-stroke-width": 1, "circle-stroke-color": "#4A3A12" } }
      ]
    }
    """;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // Below zoom 12 no tile asks the service: a tile there would be a bounding-box
        // query for the places of a whole province.
        var places = new FeatureSourceVectorTileSource { MinDataZoom = 12 };
        places.FeatureSources.Add("places", new OgcApiAsyncFeatureSource(
            "https://api-features.ign.es", "namedplace")
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        });

        var basemapTiles = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, basemapTiles);
        style.AddStyle(PlacesStyle, places);
        map.Basemap = new GpuBasemap(style);

        map.CenterPoint = new PointShape(235690, 5057360);
        map.MapScale = 36200;
        _ = map.RefreshAsync();
    }
}
