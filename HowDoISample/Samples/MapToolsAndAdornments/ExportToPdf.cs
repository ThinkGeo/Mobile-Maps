using System.Diagnostics;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The map on screen written out as a true vector PDF - geometry as PDF paths and
/// labels as real, selectable text - and handed to the system to open, file or print.
/// </summary>
/// <remarks>
/// The GPU draws triangles, and triangles cannot be a vector PDF. The same style
/// document can be drawn a second way: <see cref="MvtTilesAsyncLayer"/> takes the
/// style.json the GPU map is wearing, resolves it with the same compile every
/// renderer shares, and draws it through the classic canvas styles onto whatever
/// GeoCanvas it is handed - here <see cref="PdfGeoCanvas"/>, which is SkiaSharp and
/// the BCL and nothing else, so this works on a phone exactly as it does on a server.
/// <para>
/// The desktop gallery wraps this in a layout designer: a preview you can zoom, page
/// setup, draggable layout layers, tables. None of that belongs on a phone. What
/// belongs is the one thing the designer is for - a sheet with the map, a title and a
/// scale on it - and then the real PDF, opened in the system's own viewer, which is a
/// better preview than any we could draw and is also the artifact itself.
/// </para>
/// </remarks>
public class ExportToPdf : SamplePage
{
    private sealed record Sheet(string Name, PrinterPageSize Size, PrinterOrientation Orientation);

    /// <summary>
    /// Which way up, and nothing else. The strip used to mix two questions - A4
    /// portrait, A4 landscape, Letter - so two of its cells differed by orientation
    /// and the third by a paper size that is six millimetres narrower and eighteen
    /// shorter, a difference nobody can see in a map on a sheet. Which way up is the
    /// one choice that changes the picture. Letter is the same call with
    /// <see cref="PrinterPageSize.AnsiA"/>, and every other size the printer knows is
    /// a value of that enum.
    /// </summary>
    private static readonly Sheet[] Sheets =
    {
        new("Portrait", PrinterPageSize.A4, PrinterOrientation.Portrait),
        new("Landscape", PrinterPageSize.A4, PrinterOrientation.Landscape),
    };

    /// <summary>
    /// The city limits as a boundary rather than as a wash. It was a translucent
    /// orange fill, which over a whole city on a sheet of paper reads as a stain
    /// over the map rather than as where the city ends.
    /// </summary>
    private const string CityLimitStyle = """
    {
      "layers": [
        { "type": "line",
          "source-layer": "citylimits",
          "paint": { "line-color": "#E0533C", "line-width": 2 } }
      ]
    }
    """;

    /// <summary>Margin, title band and scale band on the sheet, in inches.</summary>
    private const double Margin = 0.5;
    private const double TitleHeight = 0.5;
    private const double ScaleHeight = 0.45;

    private MapView _map;
    private Label _status;
    private Button _export;
    private int _sheet;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // The screen gets the same two layers the sheet does: the published light
        // style, and the city limits over it. They were only on the sheet, so the
        // line under this map promised what you see is what is printed and the PDF
        // came back with a boundary across it that had never been on the screen.
        var style = new MapStyle();
        style.AddStyle(ThinkGeoVectorStyles.Light, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });

        var cityLimitTiles = new FeatureSourceVectorTileSource();
        cityLimitTiles.FeatureSources.Add("citylimits", new ShapeFileFeatureSource(
            SampleShared.Shapefile("FriscoCityLimits.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        });
        style.AddStyle(CityLimitStyle, cityLimitTiles);

        map.Basemap = new GpuBasemap(style);

        AddChoiceStrip(Sheets.Select(sheet => sheet.Name).ToList(), 0, index => _sheet = index);

        _export = new Button { Text = "Export PDF", FontSize = 13, Padding = new Thickness(14, 6) };
        _export.Clicked += async (_, _) => await ExportAsync();
        AddToPanel(_export);

        _status = AddStatusLabel();
        _status.Text = "What you see is what is printed.";

        // The whole city, because the city limits are what this sample puts on the
        // sheet beside the basemap: at the street scale this opened on, the boundary
        // was off the screen and the sheet came back with a line nobody had seen.
        map.CenterPoint = new PointShape(-10779000, 3915000);
        map.MapScale = 180000;
        _ = map.RefreshAsync();

    }

    private async Task ExportAsync()
    {
        _export.IsEnabled = false;
        _status.Text = "Drawing the sheet…";

        try
        {
            var extent = _map.CurrentExtent;
            var sheet = Sheets[_sheet];
            var path = Path.Combine(FileSystem.CacheDirectory, "map.pdf");

            var watch = Stopwatch.StartNew();
            await Task.Run(() => ExportAsync(extent, sheet, path));
            watch.Stop();

            var size = new FileInfo(path).Length / 1024d;
            _status.Text = FormattableString.Invariant(
                $"{size:N0} KB of vector PDF in {watch.ElapsedMilliseconds:N0} ms - text and all");

            // The system's own viewer: open it, file it, mail it, AirPrint it. A PDF
            // that only ever existed inside the application would not have been worth
            // making.
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Map PDF",
                File = new ShareFile(path),
            });
        }
        catch (Exception exception)
        {
            _status.Text = "Export failed: " + exception.Message;
        }
        finally
        {
            _export.IsEnabled = true;
        }
    }

    /// <summary>
    /// The layout and the export: one sheet, the map on it with a title over and a
    /// scale under, written to a PDF.
    /// </summary>
    private static async Task ExportAsync(RectangleShape extent, Sheet sheet, string path)
    {
        var page = new PagePrinterLayoutAsyncLayer(sheet.Size, sheet.Orientation);
        var paper = page.GetPosition(PrintingUnit.Inch);

        var frameWidth = paper.Width - (Margin * 2);
        var frameHeight = paper.Height - (Margin * 2) - TitleHeight - ScaleHeight;
        var centerX = paper.GetCenterPoint().X;
        var top = paper.UpperLeftPoint.Y - Margin;

        var title = new LabelPrinterLayoutAsyncLayer(
            "Frisco, Texas", new GeoFont("Arial", 16, DrawingFontStyles.Bold), GeoBrushes.Black);
        title.SetPosition(frameWidth, TitleHeight, centerX, top - (TitleHeight / 2), PrintingUnit.Inch);

        // The frame has a shape of its own, so the extent is widened to it the way the
        // map widens one to the screen; handed over as it is, the sheet would print a
        // crop of what was on screen.
        var mapLayer = new MapPrinterLayoutAsyncLayer(
            BuildPrintableLayers(),
            MapUtil.GetDrawingExtent(extent, frameWidth, frameHeight),
            GeographyUnit.Meter);
        mapLayer.SetPosition(frameWidth, frameHeight, centerX,
            top - TitleHeight - (frameHeight / 2), PrintingUnit.Inch);

        // The neatline. A map on paper has an edge; without one the streets simply
        // stop somewhere in the middle of the sheet.
        mapLayer.BackgroundMask = new AreaStyle(new GeoPen(GeoColors.DimGray, 1));

        var scale = new ScaleLinePrinterLayoutAsyncLayer(mapLayer);
        scale.SetPosition(frameWidth / 2, ScaleHeight, centerX,
            paper.LowerRightPoint.Y + Margin + (ScaleHeight / 2), PrintingUnit.Inch);

        var layoutPage = new PrinterLayoutPage(page);
        layoutPage.Layers.Add(mapLayer);
        layoutPage.Layers.Add(title);
        layoutPage.Layers.Add(scale);

        var document = new PrinterLayoutDocument();
        document.Pages.Add(layoutPage);

        using var stream = File.Create(path);
        await document.ExportPdfAsync(stream);
    }

    /// <summary>
    /// The same map, drawn the other way: the style document the GPU is wearing, over
    /// the same tiles, through the classic canvas styles - which is what can become a
    /// vector page.
    /// </summary>
    private static IEnumerable<LayerBase> BuildPrintableLayers()
    {
        var streets = new MvtTilesAsyncLayer(ThinkGeoVectorStyles.Light);

        // The published style's tile URL carries a {key} placeholder - the key is
        // deliberately not baked into a public document - so it is filled in on the
        // way out, which is what the web viewer's transformRequest does.
        streets.SendingHttpRequest += (_, e) =>
        {
            var uri = e.HttpRequestMessage?.RequestUri;
            if (uri != null && uri.OriginalString.Contains("{key}"))
            {
                e.HttpRequestMessage.RequestUri =
                    new Uri(uri.OriginalString.Replace("{key}", SampleShared.CloudApiKey));
            }
        };

        var cityLimits = new ShapeFileFeatureLayer(SampleShared.Shapefile("FriscoCityLimits.shp"))
        {
            FeatureSource = { ProjectionConverter = new ProjectionConverter(2276, 3857) },
        };
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
            new GeoPen(GeoColor.FromHtml("#E0533C"), 2));
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        return new LayerBase[] { streets, cityLimits };
    }
}
