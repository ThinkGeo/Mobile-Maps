using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The style document's own decision-making: match, step, filter, case and zoom
/// interpolation, each deciding a country's colour from its own attributes rather
/// than from code.
/// </summary>
public class RenderBasedOnRules : SamplePage
{
    private const string Background = """
        { "type": "background",
          "paint": { "background-color": "#1F2226" } },
    """;

    private static readonly (string Name, string Json)[] Rules =
    {
        ("match - by currency", $$"""
        {
          "layers": [
            {{Background}}
            { "type": "fill",
              "source-layer": "Countries02",
              "paint": {
                  "fill-outline-color": "#FFFFFF",
                  "fill-color": ["match", ["get", "CURR_TYPE"],
                      "Dollar", "rgba(31,119,180,0.8)",
                      "Franc", "rgba(44,160,44,0.8)",
                      "Pound", "rgba(214,39,40,0.8)",
                      "Peso", "rgba(148,103,189,0.8)",
                      "rgba(233,232,214,0.8)"] }
            }
          ]
        }
        """),

        ("step - by population", $$"""
        {
          "layers": [
            {{Background}}
            { "type": "fill",
              "source-layer": "Countries02",
              "paint": {
                  "fill-outline-color": "#FFFFFF",
                  "fill-color": ["step", ["get", "POP_CNTRY"],
                      "rgba(233,232,214,0.85)",
                      1000000, "rgba(253,208,162,0.85)",
                      10000000, "rgba(253,141,60,0.85)",
                      50000000, "rgba(217,71,1,0.85)",
                      200000000, "rgba(127,39,4,0.85)"] }
            }
          ]
        }
        """),

        ("filter - landlocked only", $$"""
        {
          "layers": [
            {{Background}}
            { "type": "fill",
              "source-layer": "Countries02",
              "paint": {
                  "fill-color": "rgba(233,232,214,0.8)",
                  "fill-outline-color": "#FFFFFF" }
            },
            { "type": "fill",
              "source-layer": "Countries02",
              "filter": ["==", ["get", "LANDLOCKED"], "Y"],
              "paint": {
                  "fill-color": "rgba(46,139,87,0.8)",
                  "fill-outline-color": "#FFFFFF" }
            }
          ]
        }
        """),

        ("case - two conditions", $$"""
        {
          "layers": [
            {{Background}}
            { "type": "fill",
              "source-layer": "Countries02",
              "paint": {
                  "fill-outline-color": "#FFFFFF",
                  "fill-color": ["case",
                      ["all", [">", ["get", "POP_CNTRY"], 10000000],
                              ["==", ["get", "LANDLOCKED"], "Y"]],
                      "rgba(255,255,0,0.8)",
                      "rgba(233,232,214,0.8)"] }
            }
          ]
        }
        """),

        ("zoom - width and labels", $$"""
        {
          "layers": [
            {{Background}}
            { "type": "fill",
              "source-layer": "Countries02",
              "paint": { "fill-color": "rgba(233,232,214,0.8)" } },
            { "type": "line",
              "source-layer": "Countries02",
              "paint": {
                  "line-color": "#8C8C7A",
                  "line-width": ["interpolate", ["linear"], ["zoom"], 1, 0.5, 6, 4] }
            },
            { "type": "symbol",
              "source-layer": "Countries02",
              "minzoom": 2,
              "maxzoom": 4,
              "layout": {
                  "text-field": ["get", "ISO_3DIGIT"],
                  "text-font": ["Noto Sans Bold"],
                  "text-size": 11 },
              "paint": { "text-color": "#5A5A50" }
            },
            { "type": "symbol",
              "source-layer": "Countries02",
              "minzoom": 4,
              "layout": {
                  "text-field": ["get", "CNTRY_NAME"],
                  "text-font": ["Noto Sans Regular"],
                  "text-size": ["step", ["zoom"], 12, 6, 15] },
              "paint": { "text-color": "#333333" }
            }
          ]
        }
        """),
    };

    private FeatureSourceVectorTileSource _tiles;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // Every restyle passes this same source: a document on its own has no data
        // to draw.
        _tiles = new FeatureSourceVectorTileSource();
        _tiles.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile("Countries02.shp"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        });

        AddChoicePicker(Rules.Select(r => r.Name).ToList(), 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        map.Basemap = new GpuBasemap(new MapStyle(Rules[0].Json, _tiles));
        map.CurrentExtent = MaxExtents.SphericalMercator;
        _status.Text = Rules[0].Name;
        _ = map.RefreshAsync();
    }

    private async Task ShowAsync(int index)
    {
        try
        {
            await _map.Basemap.SetStyleAsync(new MapStyle(Rules[index].Json, _tiles));
            _status.Text = Rules[index].Name;
        }
        catch (Exception exception)
        {
            // A document that does not compile changes nothing; the map keeps the
            // last one that worked.
            _status.Text = "not applied - " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
