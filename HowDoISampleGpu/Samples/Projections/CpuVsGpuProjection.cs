using System.Diagnostics;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same six projections asked of both sides at once: above, the classic pipeline,
/// which reprojects the data on the way in; below, the GPU basemap, which reprojects
/// the picture on the way out.
/// </summary>
/// <remarks>
/// Not a SamplePage: this one needs two maps. The desktop gallery puts them side by
/// side; a phone is tall and narrow, so they are stacked, which keeps each one as wide
/// as the screen - the dimension a world projection needs most.
/// </remarks>
public class CpuVsGpuProjection : ContentPage
{
    private const string CountriesFile = "Countries02.shp";
    private const int SourceSrid = 4326;
    private const double WorldHalfSize = 20037508.342789244;

    private const string StyleJson = """
    {
      "layers": [
        { "type": "background", "paint": { "background-color": "#1F2226" } },

        { "type": "fill",
          "source-layer": "countries",
          "paint": {
              "fill-color": "#4A4D52",
              "fill-outline-color": "#8A8F96" }
        }
      ]
    }
    """;

    /// <summary>What each choice asks both sides for.</summary>
    private static readonly (string Name, string ProjString)[] Choices =
    {
        ("Robinson", "+proj=robin +datum=WGS84 +units=m +no_defs"),
        ("Mollweide", "+proj=moll +datum=WGS84 +units=m +no_defs"),
        ("Eckert IV", "+proj=eck4 +datum=WGS84 +units=m +no_defs"),
        ("Cylindrical equal-area", "+proj=cea +datum=WGS84 +units=m +no_defs"),
        ("Equirectangular", "+proj=eqc +datum=WGS84 +units=m +no_defs"),
        ("Mercator", "+proj=merc +datum=WGS84 +units=m +no_defs"),
    };

    private readonly MapView _cpuMap;
    private readonly MapView _gpuMap;
    private readonly Label _cpuReadout;
    private readonly Label _gpuReadout;
    private readonly Label _datumReadout;

    private ShapeFileFeatureLayer _cpuLayer;
    private bool _ready;

    public CpuVsGpuProjection()
    {
        _cpuMap = NewMap();
        _gpuMap = NewMap();

        _cpuReadout = Readout();
        _gpuReadout = Readout();
        _datumReadout = Readout();

        var picker = new Picker { FontSize = 14 };
        foreach (var (name, _) in Choices) picker.Items.Add(name);
        picker.SelectedIndex = 0;
        picker.SelectedIndexChanged += async (_, _) =>
        {
            if (_ready && picker.SelectedIndex >= 0) await ApplyAsync(Choices[picker.SelectedIndex].ProjString);
        };

        var datum = new Button { Text = "Ask both for a historic datum", FontSize = 12, Padding = new Thickness(10, 4) };
        datum.Clicked += (_, _) => ShowDatum();

        var card = new Border
        {
            Content = new ScrollView
            {
                MaximumHeightRequest = 200,
                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children = { picker, _cpuReadout, _gpuReadout, datum, _datumReadout },
                },
            },
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            Padding = new Thickness(12, 10),
            Margin = new Thickness(10, 10, 10, 0),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
        };

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Star),
            },
        };
        grid.Add(card, 0, 0);
        grid.Add(Titled("reprojected on the way in - the classic pipeline", _cpuMap), 0, 1);
        grid.Add(Titled("reprojected on the way out - the GPU basemap", _gpuMap), 0, 2);
        Content = grid;

        // Both maps, not just one: the CPU side is framed from its own canvas size, so
        // building while it is still unlaid out leaves it at a scale of nothing.
        var built = false;

        void WhenBothSized()
        {
            if (built) return;
            if (_cpuMap.Width <= 0 || _cpuMap.Height <= 0 || _gpuMap.Width <= 0 || _gpuMap.Height <= 0) return;
            built = true;
            Build();
        }

        _cpuMap.SizeChanged += (_, _) => WhenBothSized();
        _gpuMap.SizeChanged += (_, _) => WhenBothSized();
    }

    private static MapView NewMap() => new()
    {
        MapUnit = GeographyUnit.Meter,
        IsRotationEnabled = true,
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill,
        Behaviors = { new AutoLoadMapViewBehavior() },
    };

    private static Label Readout() => new()
    {
        FontSize = 11,
        TextColor = Colors.DimGray,
        FontFamily = "Courier",
    };

    private static View Titled(string text, MapView map)
    {
        var grid = new Grid { Children = { map } };
        grid.Add(new Label
        {
            Text = text,
            FontSize = 10,
            TextColor = Colors.White,
            BackgroundColor = Color.FromRgba(0, 0, 0, 140),
            Padding = new Thickness(8, 3),
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(8, 8, 0, 0),
        });
        return grid;
    }

    private void Build()
    {
        // ---- above: projected on the way in ----
        _cpuLayer = new ShapeFileFeatureLayer(SampleShared.Shapefile(CountriesFile));
        _cpuLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            new AreaStyle(new GeoPen(GeoColor.FromHtml("#8A8F96"), 1), new GeoSolidBrush(GeoColor.FromHtml("#4A4D52")));
        _cpuLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // The desktop sample asks for TileType.SingleTile here; a single-tile overlay
        // draws nothing on MAUI, so this one keeps the default.
        var cpuOverlay = new LayerOverlay();
        cpuOverlay.Layers.Add(_cpuLayer);
        _cpuMap.Overlays.Add(cpuOverlay);
        // MAUI's MapView has no BackgroundOverlay, so the dark ground behind the
        // countries is the view's own background colour.
        _cpuMap.BackgroundColor = Color.FromArgb("#1F2226");

        // ---- below: projected on the way out ----
        // The one CPU transform this side does: 4326 to web mercator, once, so the
        // tiles are in the coordinates the renderer speaks. Everything the projection
        // picker asks for after that is done by the shader.
        var tiles = new FeatureSourceVectorTileSource();
        tiles.FeatureSources.Add("countries", new ShapeFileFeatureSource(SampleShared.Shapefile(CountriesFile))
        {
            ProjectionConverter = new ProjectionConverter(SourceSrid, 3857),
        });

        _gpuMap.Basemap = new GpuBasemap(new MapStyle(StyleJson, tiles));

        _ready = true;
        _ = ApplyAsync(Choices[0].ProjString);
    }

    private async Task ApplyAsync(string projString)
    {
        // ---- the CPU side: a new converter, and every feature read again ----
        var cpuClock = Stopwatch.StartNew();
        _cpuLayer.Close();
        _cpuLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(SourceSrid, projString);
        _cpuLayer.Open();
        _cpuMap.CurrentExtent = _cpuLayer.GetBoundingBox();
        await _cpuMap.RefreshAsync();
        cpuClock.Stop();

        // The milliseconds say which side does work, not how fast a phone is: a
        // simulator's are not a device's.
        _cpuReadout.Text = string.Create(CultureInfo.InvariantCulture,
            $"above: {cpuClock.ElapsedMilliseconds,4} ms   read and reprojected again: yes");

        // ---- the GPU side: a different shader, and not one byte of data ----
        var gpuClock = Stopwatch.StartNew();
        var failed = string.Empty;
        try
        {
            var projection = DisplayProjection.FromProjString(projString);
            _gpuMap.Basemap.DisplayProjection = projection;
            _gpuMap.CurrentExtent = ProjectedWorldBounds(projection);
            await _gpuMap.RefreshAsync();
        }
        catch (NotSupportedException exception)
        {
            failed = exception.Message;
        }

        gpuClock.Stop();
        _gpuReadout.Text = failed.Length > 0
            ? "below: not a family this side implements"
            : string.Create(CultureInfo.InvariantCulture,
                $"below: {gpuClock.ElapsedMilliseconds,4} ms   read and reprojected again: no");

        if (failed.Length > 0) _datumReadout.Text = failed;
    }

    /// <summary>
    /// The world's outline after this projection, so the GPU side frames what it draws
    /// the way the CPU side frames what it read.
    /// </summary>
    private static RectangleShape ProjectedWorldBounds(DisplayProjection projection)
    {
        var minX = double.MaxValue;
        var maxX = double.MinValue;
        var minY = double.MaxValue;
        var maxY = double.MinValue;

        void Sample(double x, double y)
        {
            var display = projection.ToDisplay(new PointShape(x, y));
            if (double.IsNaN(display.X) || double.IsNaN(display.Y)) return;
            if (display.X < minX) minX = display.X;
            if (display.X > maxX) maxX = display.X;
            if (display.Y < minY) minY = display.Y;
            if (display.Y > maxY) maxY = display.Y;
        }

        const int steps = 64;
        var edge = WorldHalfSize * 0.9999;
        for (var i = 0; i <= steps; i++)
        {
            var t = -edge + (2.0 * edge * i / steps);
            Sample(t, edge);
            Sample(t, -edge);
            Sample(-edge, t);
            Sample(edge, t);
        }

        if (minX > maxX || minY > maxY)
        {
            return new RectangleShape(-WorldHalfSize, WorldHalfSize, WorldHalfSize, -WorldHalfSize);
        }

        var marginX = (maxX - minX) * 0.03;
        var marginY = (maxY - minY) * 0.03;
        return new RectangleShape(minX - marginX, maxY + marginY, maxX + marginX, minY - marginY);
    }

    /// <summary>
    /// The same historic datum asked of both sides. The point is not that one throws -
    /// it is WHY: the CPU side owns the data, so it can shift it; the GPU side only
    /// owns the drawing, and a shader has nowhere to put a datum, so such a definition
    /// leaves the shader families and is drawn through the PROJ grid path, with a
    /// warning in the log.
    /// </summary>
    private void ShowDatum()
    {
        const string historic = "+proj=lcc +lat_1=33 +lat_2=45 +lon_0=-96 +datum=NAD27 +units=m +no_defs";

        string cpu;
        try
        {
            var converter = new ProjectionConverter(SourceSrid, historic);
            converter.Open();
            var moved = (PointShape)converter.ConvertToExternalProjection(new PointShape(-96, 39));
            converter.Close();
            cpu = string.Create(CultureInfo.InvariantCulture,
                $"CPU: fine - (-96, 39) lands at ({moved.X:N0}, {moved.Y:N0}) on NAD27");
        }
        catch (Exception exception)
        {
            cpu = "CPU: " + exception.Message;
        }

        string gpu;
        try
        {
            DisplayProjection.FromProjString(historic);
            gpu = "GPU: grid path - no shader family can apply a datum shift, so the definition is drawn through PROJ; the log carries a warning saying so";
        }
        catch (NotSupportedException exception)
        {
            gpu = "GPU: refused - " + exception.Message;
        }

        _datumReadout.Text = cpu + Environment.NewLine + Environment.NewLine + gpu;
    }
}
