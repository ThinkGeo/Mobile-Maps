using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Vector data from a service. The first entries are whole basemaps from a tile
/// provider; the last two are feature services - WFS and OGC API - Features - whose
/// features are merged into the basemap's style and drawn by the same renderer. Both
/// arrive as a source plus a style, which is why they are one sample: the question is
/// "how do I put a vector service on the map".
/// </summary>
public class VectorMapProviders : SamplePage
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

    private const string PlacesStyle = """
    {
      "layers": [
        { "type": "circle",
          "source-layer": "places",
          "paint": { "circle-radius": 4, "circle-color": "#FFC857", "circle-stroke-width": 1, "circle-stroke-color": "#4A3A12" } }
      ]
    }
    """;

    private static readonly (string Name, string StyleUri, string Shows)[] Providers =
    {
        ("Cloud Light", null, "ThinkGeo's own streets, light"),
        ("Cloud Dark", null, "The same tiles under a dark style"),
        ("OFM Liberty", "https://tiles.openfreemap.org/styles/liberty", "OpenFreeMap's Liberty, from OpenStreetMap data"),
        ("OFM Positron", "https://tiles.openfreemap.org/styles/positron", "Positron: a pale backdrop meant to be drawn over"),
        ("OFM Dark", "https://tiles.openfreemap.org/styles/dark", "The same OpenStreetMap data, dark"),
        ("WFS parcels", null, "Cadastral parcels in Amsterdam, queried from the Kadaster"),
        ("OGC API places", null, "Named places in Spain, queried from IGN"),
    };

    private const int WfsIndex = 5;
    private const int OgcApiIndex = 6;

    private readonly ThinkGeoVectorTileSource _thinkGeo = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    /// <summary>The style behind each entry, built on its first showing.</summary>
    private readonly Dictionary<int, MapStyle> _styles = new();

    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(-10779700, 3912000);
        map.MapScale = 18100;

        AddChoiceStrip(Providers.Select(p => p.Name).ToList(), 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    private async Task ShowAsync(int index)
    {
        try
        {
            var provider = Providers[index];

            // Each entry keeps the style it built. A style that has been compiled once
            // is handed back over without fetching its document or its sprite sheet
            // again and without compiling it again - what is left is the re-styling of
            // the tiles the map already holds, which is the part that cannot be
            // avoided. Built fresh on every tap, going back to a style already seen
            // costs the two downloads and the compile a second time.
            if (!_styles.TryGetValue(index, out var style))
            {
                style = index is WfsIndex or OgcApiIndex
                    ? BuildFeatureServiceStyle(index)
                    : provider.StyleUri == null
                        ? new MapStyle(index == 0 ? ThinkGeoVectorStyles.Light : ThinkGeoVectorStyles.Dark, _thinkGeo)
                        : new MapStyle(provider.StyleUri);

                _styles[index] = style;
            }

            if (_map.Basemap == null)
            {
                _map.Basemap = new GpuBasemap(style);
            }
            else
            {
                await _map.Basemap.SetStyleAsync(style);
            }

            Arrive(index);

            // What is on the screen, not the style's URL or the name of the class
            // behind it: the URL is in the table above and the class follows from
            // the cell that was tapped.
            _status.Text = provider.Shows;
        }
        catch (Exception exception)
        {
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    /// <summary>
    /// A feature service, laid over the basemap's own layers. Its features are tiled on
    /// the way in, so the renderer draws them as it draws everything else.
    /// </summary>
    private MapStyle BuildFeatureServiceStyle(int index)
    {
        var basemapTiles = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };

        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, basemapTiles);

        if (index == WfsIndex)
        {
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

            style.AddStyle(ParcelStyle, parcels);
            return style;
        }

        // Below zoom 12 no tile asks the service: a tile there would be a bounding-box
        // query for the places of a whole province.
        var places = new FeatureSourceVectorTileSource { MinDataZoom = 12 };
        places.FeatureSources.Add("places", new OgcApiAsyncFeatureSource(
            "https://api-features.ign.es", "namedplace")
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        });

        style.AddStyle(PlacesStyle, places);
        return style;
    }

    /// <summary>The basemaps share a view; each feature service has its own country.</summary>
    private void Arrive(int index)
    {
        switch (index)
        {
            case WfsIndex:
                _map.CenterPoint = new PointShape(544950, 6867650);
                _map.MapScale = 4000;
                break;

            case OgcApiIndex:
                _map.CenterPoint = new PointShape(235690, 5057360);
                _map.MapScale = 36200;
                break;
        }
    }

}
