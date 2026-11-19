using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Labelling without a glyph server: city names in Inuktitut, Portuguese, Russian,
/// Greek, Chinese and Korean, drawn from SDF glyph files shipped with the app.
/// </summary>
/// <remarks>
/// The desktop sample has a second half this one does not: a folder on the desktop
/// that the user drops .ttf and .otf files into, which the renderer then rasterizes
/// glyphs from. A phone has no such folder - an app's sandbox is not somewhere a
/// person drops files - so on mobile the shipped-glyphs path is the whole story, and
/// it is the one that matters here: an app that must label CJK or Cyrillic offline
/// carries its glyphs rather than reaching for a server.
/// </remarks>
public class LocalFonts : SamplePage
{
    private const string WorldStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "countries",
          "paint": {
              "fill-color": "#EFEBE4",
              "fill-outline-color": "#B9B2A5" }
        }
      ]
    }
    """;

    private const string CitiesStyle = """
    {
      "layers": [
        { "type": "circle",
          "source-layer": "cities",
          "paint": {
              "circle-radius": 4,
              "circle-color": "#C0392B" }
        },

        { "type": "symbol",
          "source-layer": "cities",
          "layout": {
              "text-field": "{name}",
              "text-font": ["Open Sans Semibold"],
              "text-size": 18,
              "text-variable-anchor": ["top", "bottom", "left", "right"],
              "text-radial-offset": 0.6 },
          "paint": {
              "text-color": "#1A1A1A",
              "text-halo-color": "#FFFFFF",
              "text-halo-width": 2 }
        }
      ]
    }
    """;

    private MapView _map;
    private Label _status;
    private bool _fromGlyphFiles = true;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(4000000, 5800000);
        map.MapScale = 240000000;
        PutPanelAtTop();

        AddChoicePicker(new[] { "Glyphs shipped with the app", "Whatever the device has installed" }, 0, index =>
        {
            _fromGlyphFiles = index == 0;
            _ = ShowAsync();
        });

        _status = AddStatusLabel();
        _ = ShowAsync();
    }

    private static Feature City(string name, double longitude, double latitude)
    {
        var city = new Feature(longitude, latitude);
        city.ColumnValues["name"] = name;
        return city;
    }

    // A new basemap each time, on new sources: a basemap that is replaced takes its
    // sources with it.
    private async Task ShowAsync()
    {
        var world = new MbTilesVectorTileSource(Path.Combine(SampleShared.DataFolder, "Mbtiles", "maplibre.mbtiles"));
        await world.OpenAsync();

        var cities = new InMemoryFeatureSource(new[] { new FeatureSourceColumn("name") }, new[]
        {
            City("ᐃᖃᓗᐃᑦ", -68.517, 63.747),
            City("São Paulo", -46.633, -23.550),
            City("Москва", 37.618, 55.751),
            City("Αθήνα", 23.728, 37.984),
            City("香港", 114.169, 22.319),
            City("서울", 126.978, 37.567),
        })
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        };

        var cityTiles = new FeatureSourceVectorTileSource();
        cityTiles.FeatureSources.Add("cities", cities);

        var style = new MapStyle().SetBackground(GeoColor.FromHtml("#C9DCE8"));
        if (_fromGlyphFiles)
        {
            // The glyph URL is a path into the copied data folder, so the ranges come
            // off the device rather than off the network.
            var glyphs = Path.Combine(SampleShared.DataFolder, "Glyphs");
            style.AddStyle(FormattableString.Invariant(
                $$"""{ "glyphs": "{{glyphs.Replace("\\", "/")}}/{fontstack}/{range}.pbf", "layers": [] }"""));
        }

        style.AddStyle(WorldStyle, world).AddStyle(CitiesStyle, cityTiles);

        _map.Basemap = new GpuBasemap(style);
        _status.Text = _fromGlyphFiles
            ? "Open Sans Semibold, from .pbf ranges in the app's data folder"
            : "No glyph source: the device's own fonts draw every script they can";

        await _map.RefreshAsync();
    }
}
