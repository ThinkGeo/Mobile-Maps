using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same map from five vector file formats. The chosen file is read by whatever
/// source understands it, cut into tiles and laid over the cloud basemap - the style
/// never learns which format it was.
/// </summary>
public class VectorFileFormats : SamplePage
{
    private const string DataStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "data",
          "paint": { "fill-color": "rgba(240,166,90,0.35)", "fill-outline-color": "#F2A65A" } },
        { "type": "line",
          "source-layer": "data",
          "paint": { "line-color": "#F2A65A", "line-width": 2 } },
        { "type": "circle",
          "source-layer": "data",
          "paint": { "circle-radius": 3.5, "circle-color": "#F2A65A" } }
      ]
    }
    """;

    private static readonly string[] Formats =
    {
        "Shapefile - Frisco parks",
        "GeoJSON - Pittsburgh historic districts",
        "GPX - hike and bike trails",
        "MapInfo TAB - Frisco city limits",
        "TinyGeo - Frisco zoning",
    };

    // The basemap under the data. One source for the life of the sample: a restyle
    // keeps the sources it recognizes, so the map below holds still while the layer
    // above it changes file.
    private readonly ThinkGeoVectorTileSource _cloud = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    private MapView _map;
    private Label _status;
    private int _request;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        AddChoicePicker(Formats, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    /// <summary>The line that differs: which class reads which file.</summary>
    private static FeatureSource OpenChosen(int index) => index switch
    {
        0 => new ShapeFileFeatureSource(SampleShared.Shapefile("Parks.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        },
        1 => new GeoJsonFeatureSource(Data("GeoJson", "pittsburghpacity-designated-historic-districts.geojson"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        },
        // KML is read through KmlGdalFeatureSource, which needs the GDAL natives.
        // Those ship for Windows and Linux, not for iOS or Android, so the format is
        // left out here rather than offered and failing - see SampleCatalog.NotOnMobile.
        2 => new GpxFeatureSource(Data("Gpx", "Hike_Bike.gpx"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        },
        3 => new TabFeatureSource(Data("Tab", "City_ETJ.tab"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        },
        _ => new TinyGeoFeatureSource(Data("TinyGeo", "Zoning.tgeo"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        },
    };

    /// <summary>
    /// Opening the file to ask where it is happens off the UI thread: some of these
    /// are megabytes of parsing. A choice made while one is still opening wins, and
    /// the late answer is dropped.
    /// </summary>
    private async Task ShowAsync(int index)
    {
        var request = ++_request;
        _status.Text = "opening " + Formats[index] + "…";

        try
        {
            var source = OpenChosen(index);
            var bounds = await Task.Run(() =>
            {
                source.Open();
                try { return source.GetBoundingBox(); }
                finally { source.Close(); }
            });

            if (request != _request) return;

            var tiles = new FeatureSourceVectorTileSource();
            tiles.FeatureSources.Add("data", source);

            var style = new MapStyle();
            style.AddStyle(ThinkGeoVectorStyles.Dark, _cloud);
            style.AddStyle(DataStyle, tiles);

            _map.Basemap = new GpuBasemap(style);
            bounds.ScaleUp(20);
            _map.CurrentExtent = bounds;
            _status.Text = source.GetType().Name;
        }
        catch (Exception exception)
        {
            if (request != _request) return;
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private static string Data(params string[] parts)
        => Path.Combine(new[] { SampleShared.DataFolder }.Concat(parts).ToArray());
}
