using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Colour families from the ThinkGeo Cloud, used as a choropleth ramp: ask the
/// service for six colours of a kind, then class Frisco's census blocks by their
/// housing unit count.
/// </summary>
public class ColorUtilities : SamplePage
{
    private static readonly string[] Families =
    {
        "Hue", "Quality", "Analogous", "Complementary", "Contrasting", "Triad", "Tetrad",
    };

    private readonly FeatureSourceVectorTileSource _tiles = new();
    private ColorCloudClient _client;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        // The census data is in Texas North Central (2276); the map is in Spherical
        // Mercator (3857).
        var housingUnits = new ShapeFileFeatureSource(SampleShared.Shapefile("Frisco 2010 Census Housing Units.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };
        _tiles.FeatureSources.Add("housing", housingUnits);

        housingUnits.Open();
        var extent = housingUnits.GetBoundingBox();
        housingUnits.Close();

        _client = new ColorCloudClient
        {
            ClientId = SampleShared.ClientId,
            ClientSecret = SampleShared.ClientSecret,
        };

        AddChoicePicker(Families, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        extent.ScaleUp(20);
        map.CurrentExtent = extent;

        _ = ShowAsync(0);
    }

    private async Task ShowAsync(int index)
    {
        _status.Text = "asking the colour service…";
        try
        {
            var colors = await GetColorsAsync(Families[index], 6);
            if (colors.Count == 0)
            {
                _status.Text = "The service returned no colours.";
                return;
            }

            // One class per colour the service returned - some families return more
            // colours than were asked for - spread over the range of the H_UNITS
            // column (0 to 6,000).
            var step = Math.Ceiling(6000d / colors.Count / 100) * 100;
            var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            });
            style.AddStyle(StepStyle(colors, step), _tiles);

            if (_map.Basemap == null) _map.Basemap = new GpuBasemap(style);
            else await _map.Basemap.SetStyleAsync(style);

            _status.Text = $"{Families[index]} · the service returned {colors.Count} colours";
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    /// <summary>
    /// The class breaks as a style document's step expression - the GPU path's form
    /// of the desktop sample's ClassBreakStyle, and the same breaks.
    /// </summary>
    private static string StepStyle(IReadOnlyList<GeoColor> colors, double step)
    {
        var stops = new System.Text.StringBuilder();
        for (var i = 1; i < colors.Count; i++)
        {
            var color = colors[colors.Count - i - 1 < 0 ? 0 : colors.Count - i - 1];
            stops.Append(FormattableString.Invariant(
                $", {i * step}, \"rgba({color.R},{color.G},{color.B},0.75)\""));
        }

        var first = colors[colors.Count - 1];
        return FormattableString.Invariant($$"""
        {
          "layers": [
            { "type": "fill",
              "source-layer": "housing",
              "paint": {
                  "fill-outline-color": "rgba(120,120,120,0.6)",
                  "fill-color": ["step", ["to-number", ["get", "H_UNITS"]],
                      "rgba({{first.R}},{{first.G}},{{first.B}},0.75)"{{stops}}] }
            }
          ]
        }
        """);
    }

    /// <summary>
    /// Hue and Quality answer one family; the other families come back as one list per
    /// base colour the service chose, and all of them are used.
    /// </summary>
    private async Task<Collection<GeoColor>> GetColorsAsync(string family, int numberOfColors)
    {
        Dictionary<GeoColor, Collection<GeoColor>> families;
        switch (family)
        {
            case "Hue":
                return await _client.GetColorsInHueFamilyAsync(numberOfColors);
            case "Quality":
                return await _client.GetColorsInQualityFamilyAsync(numberOfColors);
            case "Analogous":
                families = await _client.GetColorsInAnalogousFamilyAsync(numberOfColors);
                break;
            case "Complementary":
                families = await _client.GetColorsInComplementaryFamilyAsync(numberOfColors);
                break;
            case "Contrasting":
                families = await _client.GetColorsInContrastingFamilyAsync(numberOfColors);
                break;
            case "Triad":
                families = await _client.GetColorsInTriadFamilyAsync(numberOfColors);
                break;
            default:
                families = await _client.GetColorsInTetradFamilyAsync(numberOfColors);
                break;
        }

        var all = new Collection<GeoColor>();
        foreach (var list in families.Values)
        {
            foreach (var color in list) all.Add(color);
        }

        return all;
    }
}
