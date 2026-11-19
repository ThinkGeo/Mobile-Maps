using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Ten real telescope pictures of M87, each inside the one before it, crossfaded by
/// zoom - a powers-of-ten dive from the galaxy to its black hole, on a grid of its
/// own that has nothing to do with the Earth.
/// </summary>
public class ZoomToBlackHole : SamplePage
{
    /// <summary>
    /// Where each picture lies, each one inside the one before it. The pictures are
    /// arranged, not measured, so the numbers are in no real unit.
    /// </summary>
    private static readonly (string Id, RectangleShape Extent)[] Pictures =
    {
        ("m87_0", new RectangleShape(-9121.505491, 7004.839967, 9121.505491, -7004.839967)),
        ("m87_1", new RectangleShape(113.333791, 3997.249537, 1806.666209, 2282.750463)),
        ("m87_2", new RectangleShape(661.536655, 3515.692504, 1390.463345, 2760.307496)),
        ("m87_3", new RectangleShape(958.298048, 3167.258683, 1057.701952, 3112.741317)),
        ("m87_4", new RectangleShape(998.599842, 3170.1752, 1039.400158, 3147.8248)),
        ("m87_5", new RectangleShape(999.994603, 3154.709799, 1010.205397, 3150.290201)),
        ("m87_6", new RectangleShape(1000.395001, 3152.892729, 1004.204999, 3150.707271)),
        ("m87_7", new RectangleShape(1000.540677, 3151.408154, 1001.559323, 3150.891846)),
        ("m87_8", new RectangleShape(1000.604466, 3151.195571, 1000.945534, 3150.958029)),
        ("m87_9", new RectangleShape(1000.69292, 3151.051128, 1000.70308, 3151.040872)),
    };

    /// <summary>The view the dive starts from, a stop for each picture as it comes in,
    /// and the black hole last.</summary>
    private static readonly (double X, double Y, double Scale)[] Stops =
    {
        (0, 0, 50000),
        (960, 3140, 20000),
        (1026, 3138, 7000),
        (1008, 3140, 1000),
        (1019, 3159, 400),
        (1005.1, 3152.5, 100),
        (1002.3, 3151.8, 40),
        (1001.05, 3151.15, 8),
        (1000.775, 3151.0768, 2),
        (1000.698, 3151.046, 0.7),
        (1000.698, 3151.046, 0.03),
    };

    private const string StyleJson = """
    {
      "layers": [
        { "type": "background", "paint": { "background-color": "#000000" } },

        { "type": "raster", "source": "m87_0",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              -0.57, 0, -0.45, 0.2, -0.32, 0.4, -0.17, 0.6, -0.01, 0.8, 0.17, 1,
              3.49, 1, 3.64, 0.9, 3.81, 0.8, 4.00, 0.7, 4.23, 0.6, 4.49, 0.5, 4.50, 0] } },

        { "type": "raster", "source": "m87_1",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              2.75, 0, 2.87, 0.2, 3.00, 0.4, 3.15, 0.6, 3.31, 0.8, 3.49, 1,
              4.49, 1, 4.64, 0.9, 4.81, 0.8, 5.00, 0.7, 5.23, 0.6, 5.49, 0.5, 5.50, 0] } },

        { "type": "raster", "source": "m87_2",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              4.27, 0, 4.39, 0.2, 4.52, 0.4, 4.66, 0.6, 4.82, 0.8, 5.00, 1,
              8.40, 1, 8.55, 0.9, 8.72, 0.8, 8.91, 0.7, 9.13, 0.6, 9.40, 0.5, 9.41, 0] } },

        { "type": "raster", "source": "m87_3",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              7.07, 0, 7.19, 0.2, 7.32, 0.4, 7.47, 0.6, 7.63, 0.8, 7.81, 1,
              10.72, 1, 10.87, 0.9, 11.04, 0.8, 11.23, 0.7, 11.45, 0.6, 11.72, 0.5, 11.73, 0] } },

        { "type": "raster", "source": "m87_4",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              8.40, 0, 8.52, 0.2, 8.65, 0.4, 8.79, 0.6, 8.95, 0.8, 9.13, 1,
              11.72, 1, 11.87, 0.9, 12.04, 0.8, 12.23, 0.7, 12.45, 0.6, 12.72, 0.5, 12.73, 0] } },

        { "type": "raster", "source": "m87_5",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              10.40, 0, 10.52, 0.2, 10.65, 0.4, 10.79, 0.6, 10.95, 0.8, 11.13, 1,
              14.45, 1, 14.61, 0.9, 14.78, 0.8, 14.97, 0.7, 15.19, 0.6, 15.45, 0.5, 15.46, 0] } },

        { "type": "raster", "source": "m87_6",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              11.72, 0, 11.84, 0.2, 11.97, 0.4, 12.11, 0.6, 12.27, 0.8, 12.45, 1,
              16.04, 1, 16.19, 0.9, 16.36, 0.8, 16.55, 0.7, 16.78, 0.6, 17.04, 0.5, 17.05, 0] } },

        { "type": "raster", "source": "m87_7",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              14.04, 0, 14.16, 0.2, 14.29, 0.4, 14.43, 0.6, 14.60, 0.8, 14.78, 1,
              17.04, 1, 17.19, 0.9, 17.36, 0.8, 17.55, 0.7, 17.78, 0.6, 18.04, 0.5, 18.05, 0] } },

        { "type": "raster", "source": "m87_8",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              16.04, 0, 16.16, 0.2, 16.29, 0.4, 16.43, 0.6, 16.60, 0.8, 16.78, 1] } },

        { "type": "raster", "source": "m87_9",
          "paint": { "raster-opacity": ["interpolate", ["linear"], ["zoom"],
              17.55, 0, 17.67, 0.2, 17.81, 0.4, 17.95, 0.6, 18.11, 0.8, 18.29, 1] } }
      ]
    }
    """;

    private CancellationTokenSource _dive = new();
    private MapView _map;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // The galaxy's own grid: the outermost picture, deep enough for the last one.
        var grid = TileMatrixSet.CreateTileMatrixSet(512,
            new RectangleShape(-9121.505491, 9121.505491, 9121.505491, -9121.505491),
            GeographyUnit.Meter, 24);

        var basemap = new GpuBasemap(new MapStyle(StyleJson));
        foreach (var (id, extent) in Pictures)
        {
            var image = File.ReadAllBytes(Path.Combine(SampleShared.DataFolder, "Jpg", id + ".jpg"));
            basemap.RasterTileSources.Add(id, new GeoImageRasterTileSource(image, extent, tileMatrixSet: grid));
        }

        map.Basemap = basemap;
        map.CenterPoint = new PointShape(Stops[0].X, Stops[0].Y);
        map.MapScale = Stops[0].Scale;

        var buttons = new HorizontalStackLayout { Spacing = 10 };
        buttons.Add(Button("Dive", DiveAsync));
        buttons.Add(Button("Back to the galaxy", BackAsync));
        AddToPanel(buttons);
        PutPanelAtTop();

        _ = map.RefreshAsync();
    }

    private Button Button(string text, Func<Task> action)
    {
        var button = new Button { Text = text, FontSize = 12, Padding = new Thickness(10, 4) };
        button.Clicked += async (_, _) => await action();
        return button;
    }

    private async Task DiveAsync()
    {
        _dive.Cancel();
        _dive = new CancellationTokenSource();
        var token = _dive.Token;

        try
        {
            for (var i = 1; i < Stops.Length; i++)
            {
                // ZoomToExtentAsync, not ZoomToAsync: the MAUI map's centre-plus-scale
                // overload is the one that takes a rotation, and the dive needs both.
                await _map.ZoomToExtentAsync(new PointShape(Stops[i].X, Stops[i].Y), Stops[i].Scale, 0,
                    cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // Another dive, or the way back, started first.
        }
    }

    private async Task BackAsync()
    {
        _dive.Cancel();
        _dive = new CancellationTokenSource();

        try
        {
            await _map.ZoomToExtentAsync(new PointShape(Stops[0].X, Stops[0].Y), Stops[0].Scale, 0,
                cancellationToken: _dive.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }
}
