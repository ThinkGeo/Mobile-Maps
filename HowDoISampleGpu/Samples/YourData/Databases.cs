using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same map over three different databases: a SQLite file carried with the app,
/// and two servers reached over the network. The style never learns which it was.
/// </summary>
/// <remarks>
/// The desktop sample offers five. GeoPackage goes through GDAL and File
/// GeoDatabase through Esri's Windows-only native SDK, neither of which ships for
/// iOS or Android; Oracle has no mobile client either. The three that are left are
/// the three a phone can actually open, and they are the interesting spread anyway:
/// a file you carry and two servers you call.
/// </remarks>
public class Databases : SamplePage
{
    private const string StyleJson = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "data",
          "filter": ["==", ["geometry-type"], "Polygon"],
          "paint": {
              "fill-color": "rgba(90,140,200,0.35)",
              "fill-outline-color": "#8AB4F0" }
        },

        { "type": "line",
          "source-layer": "data",
          "paint": {
              "line-color": "#8AB4F0",
              "line-width": 2 }
        },

        { "type": "circle",
          "source-layer": "data",
          "filter": ["==", ["geometry-type"], "Point"],
          "paint": {
              "circle-radius": 4,
              "circle-color": "#C0392B",
              "circle-stroke-color": "#FFFFFF",
              "circle-stroke-width": 1.5 }
        }
      ]
    }
    """;

    private static readonly string[] Choices = { "SQLite file", "PostgreSQL server", "SQL Server" };

    // The basemap under the data. One source for the life of the sample: a restyle
    // keeps the sources it recognizes, so the map below holds still while the layer
    // above it changes table.
    private readonly ThinkGeoVectorTileSource _cloud = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    private FeatureSourceVectorTileSource _current;
    private MapView _map;
    private Label _status;
    private int _request;
    private int _chosen;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        AddChoicePicker(Choices, 0, index =>
        {
            _chosen = index;
            _ = ShowChosenAsync();
        });

        _status = AddStatusLabel();
        _ = ShowChosenAsync();
    }

    /// <summary>
    /// The chosen table, read by whatever source understands it, cut into tiles and
    /// laid over the cloud basemap. The style never learns which database it was.
    /// </summary>
    /// <remarks>
    /// Asking the table where it is happens off the UI thread: for the two servers
    /// that is a network round trip. A tap on another database while one is still
    /// answering wins, and the late answer is dropped.
    /// </remarks>
    private async Task ShowChosenAsync()
    {
        var request = ++_request;
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Dark, _cloud);

        FeatureSourceVectorTileSource data = null;
        RectangleShape bounds = null;

        try
        {
            var source = OpenChosen();
            bounds = await Task.Run(() =>
            {
                source.Open();
                var box = source.GetBoundingBox();
                source.Close();
                return box;
            });

            if (request != _request) return;

            data = new FeatureSourceVectorTileSource();
            data.FeatureSources.Add("data", source);

            // Whole-country polygons over a network round trip are worth capping: past
            // zoom 8 the covering level-8 tile is served instead of re-querying and
            // re-simplifying the same continents at every detent.
            if (_chosen == 1) data.MaxDataZoom = 8;

            style.AddStyle(StyleJson, data);
            _status.TextColor = Colors.SeaGreen;
            _status.Text = source.GetType().Name;
        }
        catch (Exception exception)
        {
            if (request != _request) return;

            // The file comes from the sample data and the servers are
            // demodb.thinkgeo.com; without them there is nothing to draw, and saying
            // so beats an empty window. Drivers wrap the real reason, so unwrap to it.
            while (exception.InnerException != null) exception = exception.InnerException;

            _status.TextColor = Colors.IndianRed;
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        // Restyling in place, not a new basemap: assigning a basemap starts from an
        // empty scene, and that blank moment is the flash a viewer sees when switching
        // tables.
        var retired = _current;
        _current = data;

        if (_map.Basemap == null)
        {
            _map.Basemap = new GpuBasemap(style);
        }
        else
        {
            await _map.Basemap.SetStyleAsync(style);
        }

        if (request == _request)
        {
            if (bounds != null && bounds.Width > 0 && bounds.Height > 0)
            {
                bounds.ScaleUp(20);
                _map.CurrentExtent = bounds;
            }

            await _map.RefreshAsync();
        }

        retired?.Dispose();
    }

    /// <summary>
    /// The line that differs. The first is a database you carry as a file, opened
    /// through a connection string; the other two are servers you connect to. Each
    /// converts from the plane its own table stores.
    /// </summary>
    private FeatureSource OpenChosen() => _chosen switch
    {
        0 => new SqliteFeatureSource(
            "Data Source=" + Path.Combine(SampleShared.DataFolder, "SQLite", "frisco-restaurants.sqlite") + ";",
            "restaurants", "id", "geometry")
        { ProjectionConverter = new ProjectionConverter(2276, 3857) },

        1 => new PostgreSqlFeatureSource(
            "User ID=ThinkGeoTest;Password=ThinkGeoTestPassword;Host=demodb.thinkgeo.com;Port=5432;Database=postgres;Pooling=true;",
            "countries", "gid", 4326)
        { ProjectionConverter = new ProjectionConverter(4326, 3857) },

        _ => new SqlServerFeatureSource(
            "Server=demodb.thinkgeo.com;Database=thinkgeo;User Id=ThinkGeoTest;Password=ThinkGeoTestPassword;TrustServerCertificate=True;",
            "frisco_coyote_sightings", "id")
        { ProjectionConverter = new ProjectionConverter(2276, 3857) },
    };
}
