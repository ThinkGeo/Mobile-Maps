using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A feature source of your own - here over a CSV file of points - drawn by the
/// style document like any other.
/// </summary>
public class CustomFeatureSources : SamplePage
{
    private const string StyleJson = """
    {
      "layers": [
        { "type": "circle",
          "source-layer": "vehicle-route",
          "paint": {
              "circle-radius": 5,
              "circle-color": "#1E90FF",
              "circle-stroke-color": "#FFFFFF",
              "circle-stroke-width": 2 }
        }
      ]
    }
    """;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // The CSV is in latitude and longitude (4326); the map is in Spherical
        // Mercator (3857).
        var vehicleRoute = new SimpleCsvFeatureSource(
            Path.Combine(SampleShared.DataFolder, "Csv", "vehicle-route.csv"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        };

        vehicleRoute.Open();
        var extent = vehicleRoute.GetBoundingBox();
        vehicleRoute.Close();

        extent.ScaleUp(50);
        map.CurrentExtent = extent;

        // The source has no file name of its own to be known by, so the style is
        // given one.
        var tileSource = new FeatureSourceVectorTileSource();
        tileSource.FeatureSources.Add("vehicle-route", vehicleRoute);

        var cloud = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark, cloud).AddStyle(StyleJson, tileSource));

        _ = map.RefreshAsync();
    }
}

/// <summary>
/// A CSV feature source with the least a FeatureSource needs: all of its features.
/// The base class answers every other query from them, which is fine for a small
/// file; for a large one, override the spatial query methods as well.
/// </summary>
public class SimpleCsvFeatureSource : FeatureSource
{
    private Collection<Feature> _features = new();

    public SimpleCsvFeatureSource(string csvPathFileName) => CsvPathFileName = csvPathFileName;

    public string CsvPathFileName { get; }

    /// <summary>
    /// Each line is "latitude,longitude". The file is read once, on open: tiles can
    /// be cut from the source on several threads at once.
    /// </summary>
    protected override void OpenCore()
    {
        var features = new Collection<Feature>();
        foreach (var line in File.ReadAllLines(CsvPathFileName))
        {
            var parts = line.Split(',');
            features.Add(new Feature(
                double.Parse(parts[1], CultureInfo.InvariantCulture),
                double.Parse(parts[0], CultureInfo.InvariantCulture)));
        }

        _features = features;
    }

    protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        => _features;
}
