using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Live American weather from NOAA: every reporting station's temperature, and the
/// warnings in force, each a feature source that fetches when it opens.
/// </summary>
public class NOAAWeather : SamplePage
{
    private const string StationsStyle = """
    {
      "layers": [
        { "type": "circle",
          "source-layer": "stations",
          "paint": {
              "circle-radius": 4.5,
              "circle-color": ["step", ["to-number", ["get", "Temperature"]],
                  "rgba(69,117,180,1)",
                  32, "rgba(145,191,219,1)",
                  50, "rgba(255,255,191,1)",
                  70, "rgba(252,141,89,1)",
                  85, "rgba(215,48,39,1)"],
              "circle-stroke-color": "rgba(120,120,120,1)",
              "circle-stroke-width": 1 }
        },
        { "type": "symbol",
          "source-layer": "stations",
          "minzoom": 5,
          "layout": {
              "text-field": ["concat", ["get", "Temperature"], "°"],
              "text-font": ["Noto Sans Bold"],
              "text-size": 11,
              "text-offset": [0, -0.9],
              "text-anchor": "bottom" },
          "paint": {
              "text-color": "#E8E8E8",
              "text-halo-color": "#1F2226",
              "text-halo-width": 1.5 }
        }
      ]
    }
    """;

    private const string WarningsStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "warnings",
          "paint": {
              "fill-color": ["match", ["get", "SEVERITY"],
                  "Extreme", "rgba(215,25,28,0.40)",
                  "Severe", "rgba(244,109,67,0.40)",
                  "Moderate", "rgba(253,174,97,0.35)",
                  "Minor", "rgba(171,217,233,0.35)",
                  "rgba(150,150,150,0.30)"] }
        },
        { "type": "line",
          "source-layer": "warnings",
          "paint": {
              "line-color": ["match", ["get", "SEVERITY"],
                  "Extreme", "rgba(215,25,28,1)",
                  "Severe", "rgba(244,109,67,1)",
                  "Moderate", "rgba(253,174,97,1)",
                  "Minor", "rgba(171,217,233,1)",
                  "rgba(150,150,150,1)"],
              "line-width": 1.5 }
        }
      ]
    }
    """;

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

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark, _cloud));
        map.CenterPoint = new PointShape(-10807050, 5045070);
        map.MapScale = 34016000;

        AddChoicePicker(new[] { "Reporting stations", "Warnings in force" }, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = map.RefreshAsync();
        _ = ShowAsync(0);
    }

    /// <summary>
    /// The feed downloads when it opens, so a choice can arrive while the other feed
    /// is still on its way: the last one wins and a late answer is dropped.
    /// </summary>
    private async Task ShowAsync(int index)
    {
        var request = ++_request;
        _status.Text = "Downloading from NOAA…";

        try
        {
            var (styleJson, data, status) = index == 0
                ? await LoadStationsAsync()
                : await LoadWarningsAsync();

            if (request != _request) return;

            var style = new MapStyle();
            style.AddStyle(ThinkGeoVectorStyles.Dark, _cloud);
            style.AddStyle(styleJson, data);
            await _map.Basemap.SetStyleAsync(style);
            _status.Text = status;
        }
        catch (Exception exception)
        {
            if (request != _request) return;
            _status.Text = "Unavailable: " + exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private static async Task<(string StyleJson, FeatureSourceVectorTileSource Data, string Status)> LoadStationsAsync()
    {
        var stations = new NoaaWeatherStationFeatureSource
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        };
        var count = await Task.Run(() =>
        {
            stations.Open();
            return stations.GetCount();
        });

        var data = new FeatureSourceVectorTileSource();
        data.FeatureSources.Add("stations", stations);
        return (StationsStyle, data, FormattableString.Invariant($"{count} stations reporting"));
    }

    private static async Task<(string StyleJson, FeatureSourceVectorTileSource Data, string Status)> LoadWarningsAsync()
    {
        var warnings = new NoaaWeatherWarningsFeatureSource
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        };
        var count = await Task.Run(() =>
        {
            warnings.Open();
            return warnings.GetCount();
        });

        var data = new FeatureSourceVectorTileSource();
        data.FeatureSources.Add("warnings", warnings);
        return (WarningsStyle, data, FormattableString.Invariant($"{count} warnings in force"));
    }
}
