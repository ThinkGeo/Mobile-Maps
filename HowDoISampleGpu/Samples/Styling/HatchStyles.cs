using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A hatch brush as a fill pattern: the style names an image, and the image is a
/// <see cref="GeoHatchBrush"/> the application supplies. Every hatch the SDK has,
/// in one picker.
/// </summary>
public class HatchStyles : SamplePage
{
    private const string StyleJson = """
    {
      "layers": [

        { "type": "background",
          "paint": { "background-color": "#1F2226" } },

        { "type": "fill",
          "source-layer": "Parks",
          "paint": { "fill-pattern": "hatch" } },

        { "type": "line",
          "source-layer": "Parks",
          "paint": {
              "line-color": "#5C8DFF",
              "line-width": 2.5 }
        },

        { "type": "symbol",
          "source-layer": "Parks",
          "layout": {
              "text-field": ["get", "NAME"],
              "text-font": ["Noto Sans Bold"],
              "text-size": 11 },
          "paint": { "text-color": "#8FD19E" }
        }
      ]
    }
    """;

    private static readonly GeoHatchStyle[] Hatches = Enum.GetValues(typeof(GeoHatchStyle))
        .Cast<GeoHatchStyle>()
        .OrderBy(style => style.ToString(), StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private FeatureSourceVectorTileSource _parks;
    private MapView _map;
    private Label _status;
    private GeoHatchStyle _hatch = GeoHatchStyle.Cross;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        map.CenterPoint = new PointShape(-10777610, 3909120);
        map.MapScale = 2260;

        // Every restyle passes this same source: a document on its own has no data
        // to draw.
        _parks = new FeatureSourceVectorTileSource();
        _parks.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile("Parks.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        });

        var startAt = Array.IndexOf(Hatches, _hatch);
        AddChoicePicker(Hatches.Select(h => h.ToString()).ToList(), startAt < 0 ? 0 : startAt, index =>
        {
            _hatch = Hatches[index];
            _ = ApplyAsync();
        });
        _status = AddStatusLabel();

        map.Basemap = new GpuBasemap(ComposeStyle());
        _status.Text = _hatch.ToString();
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// The style names "hatch"; the image behind that name is a GeoHatchBrush. The
    /// document never learns which hatch it is, which is why changing one is a
    /// restyle of the same document.
    /// </summary>
    private MapStyle ComposeStyle()
    {
        var style = new MapStyle(StyleJson, _parks);
        style.Images.Add("hatch", new GeoHatchBrush(_hatch, GeoColors.LightGray));
        return style;
    }

    private async Task ApplyAsync()
    {
        try
        {
            await _map.Basemap.SetStyleAsync(ComposeStyle());
            _status.Text = _hatch.ToString();
        }
        catch (Exception exception)
        {
            _status.Text = "not applied - " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
