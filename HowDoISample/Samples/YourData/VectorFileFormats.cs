using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The same map from six vector file formats. The chosen file is read by whatever
/// source understands it, cut into tiles and laid over the cloud basemap - the style
/// never learns which format it was.
/// </summary>
/// <remarks>
/// A spatial SQLite file is one of these: it is a file carried with the app and
/// opened through <see cref="SqliteFeatureSource"/>, like every other entry here.
/// </remarks>
public class VectorFileFormats : SamplePage
{
    /// <summary>
    /// One style for six formats: fills, lines and circles over the same source
    /// layer, so whichever shape the file happens to hold is drawn.
    /// <para>
    /// The circle layer needs a filter. A GPX file hands back every track point as a
    /// point feature of its own, beside the line it belongs to; drawn as circles they
    /// would bury the trails. IsWayPoint is the column GpxFeatureSource marks them
    /// with: "0" for a point inside a track, "1" for a waypoint. Every other format's
    /// points have no such column, so the filter passes them.
    /// </para>
    /// </summary>
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
          "filter": ["!=", ["get", "IsWayPoint"], "0"],
          "paint": { "circle-radius": 3.5, "circle-color": "#F2A65A" } }
      ]
    }
    """;

    /// <summary>Each format, and what the file read through it holds.</summary>
    private static readonly (string Format, string Holds)[] Formats =
    {
        ("Shapefile", "Frisco's parks"),
        ("GeoJSON", "Pittsburgh's historic districts"),
        ("GPX", "Frisco's hike and bike trails, track by track"),
        ("MapInfo TAB", "Frisco's city limits and the land it governs beyond them"),
        ("TinyGeo", "Frisco's zoning areas"),
        ("SQLite", "Frisco's restaurants, a table at a time"),
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

        AddChoiceStrip(Formats.Select(f => f.Format).ToList(), 0, index => _ = ShowAsync(index));
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
        // KML is not offered: it is read through KmlGdalFeatureSource, which needs
        // the GDAL natives, and those are not available on iOS or Android.
        2 => new GpxFeatureSource(Data("Gpx", "Hike_Bike.gpx"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        },
        3 => new TabFeatureSource(Data("Tab", "City_ETJ.tab"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        },
        4 => new TinyGeoFeatureSource(Data("TinyGeo", "Zoning.tgeo"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        },
        // A file like the rest, but named by a connection string rather than a path,
        // and asked for a table, a key column and a geometry column within it.
        _ => new SqliteFeatureSource(
            "Data Source=" + Data("SQLite", "frisco-restaurants.sqlite") + ";",
            "restaurants", "id", "geometry")
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
        _status.Text = "opening " + Formats[index].Format + "…";

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
            _status.Text = Formats[index].Holds;
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
