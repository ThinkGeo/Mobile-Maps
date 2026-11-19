using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The ground with its real shape: satellite imagery draped over a terrain source,
/// hill-shaded and tilted, over the Grand Canyon.
/// </summary>
public class Terrain3D : SamplePage
{
    private const string HillshadeStyle = """
    {
      "layers": [
        { "type": "hillshade",
          "source": "terrain-dem",
          "paint": { "hillshade-exaggeration": 0.4 } }
      ]
    }
    """;

    /// <summary>The south rim, looking north across the gorge.</summary>
    private static readonly RectangleShape StartExtent =
        new(-12_534_000, 4_346_000, -12_484_000, 4_314_000);

    private GpuBasemap _basemap;
    private MapStyle _style;
    private MapView _map;
    private Switch _terrain;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // The AWS open terrarium tiles (no key), as a code-built RasterDemSource.
        // SetTerrain adds it under the id "terrain-dem", which the hillshade layer
        // above reads.
        var dem = new RasterDemSource(
            "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png",
            DemEncoding.Terrarium);

        _style = new MapStyle()
            .AddRaster(new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1),
                sourceId: "thinkgeo")
            .SetTerrain(dem, 1.35)
            .AddStyle(HillshadeStyle);

        _basemap = new GpuBasemap(_style);
        map.Basemap = _basemap;

        var row = new HorizontalStackLayout { Spacing = 8 };
        _terrain = new Switch { IsToggled = true };
        _terrain.Toggled += (_, _) => _ = ToggleTerrainAsync();
        row.Add(new Label { Text = "Terrain", FontSize = 14, VerticalOptions = LayoutOptions.Center });
        row.Add(_terrain);
        AddToPanel(row);

        AddToPanel(new Label { Text = "Tilt", FontSize = 11, TextColor = Colors.Gray });
        var tilt = new Slider { Minimum = 0, Maximum = 75, Value = 63 };
        tilt.ValueChanged += (_, e) => map.TiltAngle = e.NewValue;
        AddToPanel(tilt);

        map.CurrentExtent = StartExtent;
        map.TiltAngle = 63;
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// Terrain lives on the style; re-handing the style applies the toggle without
    /// rebuilding a tile.
    /// </summary>
    private async Task ToggleTerrainAsync()
    {
        if (_terrain.IsToggled) _style.SetTerrain("terrain-dem", 1.35);
        else _style.RemoveTerrain();

        await _basemap.SetStyleAsync(_style);
        await _map.RefreshAsync();
    }
}
