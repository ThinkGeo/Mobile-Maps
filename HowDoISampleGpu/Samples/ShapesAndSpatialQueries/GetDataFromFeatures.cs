using System.Text;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a feature carries besides its shape: tap a park and its columns - the ones
/// the file declares and the ones the SDK computes - are listed.
/// </summary>
public class GetDataFromFeatures : SamplePage
{
    private const string ParksStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "parks",
          "paint": {
              "fill-color": "rgba(147,112,219,0.2)",
              "fill-outline-color": "#9370DB" }
        }
      ]
    }
    """;

    private readonly DynamicSource _picked = new();

    // The tile cutter reads the source it is given on its own threads, so a source
    // that is not safe to read from two threads at once is queried through a copy.
    private FeatureSource _parks;
    private QueryTools _query;
    private MapView _map;
    private Label _columns;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var parks = new ShapeFileFeatureSource(SampleShared.Shapefile("Parks.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        var tiles = new FeatureSourceVectorTileSource();
        tiles.FeatureSources.Add("parks", parks);
        _parks = parks.ConcurrentAccess == ConcurrentAccess.Safe ? parks : parks.CloneDeep();
        _query = new QueryTools(_parks);

        var cloud = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };
        var style = new MapStyle(ThinkGeoVectorStyles.Dark, cloud).AddStyle(ParksStyle, tiles);
        style.AddDynamic(_picked);
        map.Basemap = new GpuBasemap(style);

        _status = AddStatusLabel();
        _columns = AddToPanel(new Label { FontSize = 11, FontFamily = "Courier", MaxLines = 10 });

        map.SingleTap += (_, e) => Pick(map.ToWorldCoordinate(e.X, e.Y));

        _parks.Open();
        var bounds = _parks.GetBoundingBox();
        var count = _parks.GetAllFeatures(ReturningColumnsType.NoColumns).Count;
        _parks.Close();

        _status.Text = $"{count} parks - tap one";
        map.CurrentExtent = bounds;
        _ = map.RefreshAsync();
    }

    private void Pick(PointShape point)
    {
        _parks.Open();
        var hit = _query.GetFeaturesContaining(point, ReturningColumnsType.AllColumns).FirstOrDefault();
        _parks.Close();

        _picked.Clear();
        if (hit == null)
        {
            _status.Text = "no park there - tap inside one";
            _columns.Text = string.Empty;
        }
        else
        {
            _picked.SetArea("picked", hit, new GeoColor(110, GeoColors.Orange), GeoColors.OrangeRed, 2f);
            _status.Text = hit.ColumnValues.TryGetValue("NAME", out var name) ? name : "park";
            _columns.Text = ColumnText(hit);
        }

        _ = _map.RefreshAsync();
    }

    private static string ColumnText(Feature feature)
    {
        var text = new StringBuilder();
        foreach (var column in feature.ColumnValues)
        {
            text.AppendLine($"{column.Key}: {column.Value}");
        }

        return text.ToString().TrimEnd();
    }
}
