using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Drawing the map into an image file rather than onto the screen: the raster basemap
/// and Frisco's zoning drawn straight into a GeoImage with a GeoCanvas, at the map's
/// own extent and size, with the world file and .prj beside it so the PNG opens in
/// place in any GIS.
/// </summary>
/// <remarks>
/// The desktop sample offers a second mode that copies what is on screen, through
/// MapView.GetSnapshot. MAUI has no counterpart, and the off-screen path is the one
/// that matters on a phone anyway: nothing on screen is copied, so the image is not
/// limited to what the screen happens to show, and the export works the same whether
/// the map is visible or not.
/// </remarks>
public class ExportAnImage : SamplePage
{
    private MapView _map;
    private Label _status;
    private Image _preview;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10779270, 3911750);
        map.MapScale = 288900;

        var markerOverlay = new SimpleMarkerOverlay();
        markerOverlay.AddMarker("centre", new Marker
        {
            Position = map.CenterPoint,
            WidthRequest = 14,
            HeightRequest = 14,
            Content = new Microsoft.Maui.Controls.Shapes.Ellipse
            {
                Fill = new SolidColorBrush(Colors.OrangeRed),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2,
            },
        });
        map.Overlays.Add(markerOverlay);

        var export = new Button { Text = "Export a PNG", FontSize = 12, Padding = new Thickness(10, 4) };
        export.Clicked += async (_, _) => await ExportAsync();
        AddToPanel(export);

        _status = AddStatusLabel();
        _status.Text = "Draws the basemap and the zoning into a file at the map's extent";
        _preview = AddToPanel(new Image { HeightRequest = 110, Aspect = Aspect.AspectFit, IsVisible = false });

        _ = map.RefreshAsync();
    }

    private async Task ExportAsync()
    {
        try
        {
            _status.Text = "Drawing…";

            var extent = _map.CurrentExtent;
            var width = (int)_map.CanvasWidth;
            var height = (int)_map.CanvasHeight;

            var image = await DrawLayersOffScreenAsync(extent, width, height);

            // A phone has no save dialog; the app's own documents folder is where an
            // export lands, and a share sheet would take it from there.
            var path = Path.Combine(FileSystem.AppDataDirectory, "map.png");
            image.Save(path);
            WriteWorldAndProjectionFiles(path, extent, width, height);

            _preview.Source = ImageSource.FromFile(path);
            _preview.IsVisible = true;
            _status.Text = FormattableString.Invariant($"{width} x {height} px, with map.pgw and map.prj beside it");
        }
        catch (Exception exception)
        {
            _status.Text = "Export failed: " + exception.Message.Split('\n')[0];
        }
    }

    private static async Task<GeoImage> DrawLayersOffScreenAsync(RectangleShape extent, int width, int height)
    {
        var basemapLayer = new ThinkGeoRasterMapsAsyncLayer(SampleShared.ClientId, SampleShared.ClientSecret);

        // The zoning data is in Texas North Central (2276); the image is drawn in
        // Spherical Mercator (3857).
        var zoningLayer = new ShapeFileFeatureLayer(SampleShared.Shapefile("Zoning.shp"));
        zoningLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoBrushes.Blue));
        zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var image = new GeoImage(width, height);
        var canvas = GeoCanvas.CreateDefaultGeoCanvas();
        canvas.BeginDrawing(image, extent, GeographyUnit.Meter);

        // The labels already placed, so a later layer's labels do not collide with them.
        var labels = new Collection<SimpleCandidate>();

        await basemapLayer.OpenAsync();
        await basemapLayer.DrawAsync(canvas, labels);
        await basemapLayer.CloseAsync();
        canvas.Flush();

        zoningLayer.Open();
        zoningLayer.Draw(canvas, labels);
        zoningLayer.Close();
        canvas.Flush();

        canvas.EndDrawing();
        return image;
    }

    /// <summary>
    /// The world file places the pixels: map units per pixel, then the world position
    /// of the centre of the top-left pixel. With the .prj naming the projection, the
    /// PNG opens in place in any GIS.
    /// </summary>
    private static void WriteWorldAndProjectionFiles(string imagePath, RectangleShape extent, int width, int height)
    {
        var pixelWidth = extent.Width / width;
        var pixelHeight = extent.Height / height;

        File.WriteAllLines(Path.ChangeExtension(imagePath, ".pgw"), new[]
        {
            pixelWidth.ToString(CultureInfo.InvariantCulture),
            "0",
            "0",
            (-pixelHeight).ToString(CultureInfo.InvariantCulture),
            (extent.UpperLeftPoint.X + (pixelWidth / 2)).ToString(CultureInfo.InvariantCulture),
            (extent.UpperLeftPoint.Y - (pixelHeight / 2)).ToString(CultureInfo.InvariantCulture),
        });

        File.WriteAllText(Path.ChangeExtension(imagePath, ".prj"), Projection.ConvertEpsgToWkt(3857));
    }
}
