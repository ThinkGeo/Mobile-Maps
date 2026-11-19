using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a shape can be asked about itself: its area, its length, its centre, and
/// the shortest line from it to somewhere else.
/// </summary>
public class ShapeMeasurements : SamplePage
{
    private const string DataStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "data",
          "paint": { "fill-color": "rgba(147,112,219,0.25)", "fill-outline-color": "#9370DB" } },
        { "type": "line",
          "source-layer": "data",
          "paint": { "line-color": "#9370DB", "line-width": 2 } }
      ]
    }
    """;

    /// <summary>Toyota Stadium, the fixed end of the shortest-line measurement.</summary>
    private static readonly PointShape Stadium = new(-10779651.5, 3915933.0);

    private static readonly string[] Measurements =
    {
        "Area of a park",
        "Length of a trail",
        "Centre of a park",
        "Shortest line to the stadium",
    };

    private readonly DynamicSource _measured = new();
    private readonly DynamicSource _stadium = new();

    private GpuBasemap _basemap;
    private FeatureSource _data;
    private QueryTools _query;
    private MapView _map;
    private Label _result;
    private int _measurement;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        AddChoicePicker(Measurements, 0, index =>
        {
            _measurement = index;
            // The measurements do not all read the same file, so switching one
            // restyles the map over a new source rather than rebuilding the basemap -
            // which would start from an empty scene and flash.
            _ = SwitchDataAsync();
        });
        _result = AddToPanel(new Label { FontSize = 13, FontAttributes = FontAttributes.Bold });
        AddToPanel(new Label { Text = "Tap a shape to measure it.", FontSize = 11, TextColor = Colors.Gray });

        _basemap = new GpuBasemap(ComposeStyle());
        map.Basemap = _basemap;

        map.SingleTap += (_, e) => Measure(map.ToWorldCoordinate(e.X, e.Y));

        map.CenterPoint = new PointShape(-10778340, 3915490);
        map.MapScale = 36110;
        _stadium.SetPoint("stadium", new Feature(Stadium), color: GeoColors.Gold, sizeInPixel: 14f);
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// The data the chosen measurement reads, and the tiles it is cut into. Parks for
    /// the ones that measure an area, trails for the one that measures along a line.
    /// </summary>
    private MapStyle ComposeStyle()
    {
        var file = _measurement == 1 ? "Hike_Bike.shp" : "Parks.shp";

        // Nothing here is drawn from a layer, so nothing here needs one: a query runs
        // on a FeatureSource. The tile cutter reads the source it is given on its own
        // threads, so a source that is not safe to read from two threads at once is
        // queried through a copy.
        var drawn = new ShapeFileFeatureSource(SampleShared.Shapefile(file))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };
        _data = drawn.ConcurrentAccess == ConcurrentAccess.Safe ? drawn : drawn.CloneDeep();
        _query = new QueryTools(_data);

        var tiles = new FeatureSourceVectorTileSource();
        tiles.FeatureSources.Add("data", drawn);

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        }).AddStyle(DataStyle, tiles);
        style.AddDynamic(_measured).AddDynamic(_stadium);
        return style;
    }

    private async Task SwitchDataAsync()
    {
        _measured.Clear();
        _result.Text = string.Empty;
        await _basemap.SetStyleAsync(ComposeStyle());
        await _map.RefreshAsync();
    }

    private void Measure(PointShape at)
    {
        _data.Open();
        var hit = _query.GetFeaturesNearestTo(at, GeographyUnit.Meter, 1, ReturningColumnsType.NoColumns)
            .FirstOrDefault();
        _data.Close();

        if (hit == null) return;

        var shape = hit.GetShape();
        _measured.Clear();

        switch (_measurement)
        {
            case 0:
            {
                var area = ((AreaBaseShape)shape).GetArea(new Projection(3857), AreaUnit.SquareKilometers);
                _measured.SetArea("measured", hit, new GeoColor(110, GeoColors.Orange), GeoColors.OrangeRed, 2f);
                _result.Text = area.ToString("f3", CultureInfo.CurrentCulture) + " sq km";
                break;
            }

            case 1:
            {
                var length = ((LineBaseShape)shape).GetLength(new Projection(3857), DistanceUnit.Kilometer);
                _measured.SetLine("measured", hit, GeoColors.OrangeRed, 4f);
                _result.Text = length.ToString("f3", CultureInfo.CurrentCulture) + " km";
                break;
            }

            case 2:
            {
                var centre = shape.GetCenterPoint();
                _measured.SetArea("measured", hit, new GeoColor(90, GeoColors.Orange), GeoColors.OrangeRed, 2f);
                _measured.SetPoint("centre", new Feature(centre), color: GeoColors.OrangeRed, sizeInPixel: 14f);

                // The map is in web-mercator metres; a centre point is read in
                // longitude and latitude.
                var toLonLat = new ProjectionConverter(3857, 4326);
                toLonLat.Open();
                var lonLat = (PointShape)toLonLat.ConvertToExternalProjection(centre);
                toLonLat.Close();
                _result.Text = "lon " + lonLat.X.ToString("f5", CultureInfo.CurrentCulture)
                    + ", lat " + lonLat.Y.ToString("f5", CultureInfo.CurrentCulture);
                break;
            }

            default:
            {
                var line = shape.GetShortestLineTo(Stadium, GeographyUnit.Meter);
                _measured.SetArea("measured", hit, new GeoColor(90, GeoColors.Orange), GeoColors.OrangeRed, 2f);
                _measured.SetLine("shortest", new Feature(line), GeoColors.Gold, 3f);
                var length = line.GetLength(new Projection(3857), DistanceUnit.Kilometer);
                _result.Text = length.ToString("f3", CultureInfo.CurrentCulture) + " km to the stadium";
                break;
            }
        }

        _ = _map.RefreshAsync();
    }
}
