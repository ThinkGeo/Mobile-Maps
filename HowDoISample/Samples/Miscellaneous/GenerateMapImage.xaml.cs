using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace HowDoISample.Miscellaneous;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GenerateMapImage
{
    private bool _initialized;
    public GenerateMapImage()
    {
        InitializeComponent();
    }

    private void GenerateMapImage_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        LoadingIndicator.IsRunning = true;
        LoadingLayout.IsVisible = true;

        MapImage.Source = GenerateImage();

        LoadingIndicator.IsRunning = false;
        LoadingLayout.IsVisible = false;
    }

    private ImageSource GenerateImage()
    {
        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var _zoningLayer = new ShapeFileFeatureLayer
        {
            ShapePathFilename = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"),
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(2276, 3857)
            }
        };
        _zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoBrushes.Blue));
        _zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        _zoningLayer.Open();

        // Create a GeoCanvas to do the drawing
        var canvas = GeoCanvas.CreateDefaultGeoCanvas();

        // Create a GeoImage as the image to draw on
        var geoImage = new GeoImage(800, 600);

        // Start the drawing by specifying the image, extent and map units
        canvas.BeginDrawing(geoImage, MapUtil.GetDrawingExtent(_zoningLayer.GetBoundingBox(), 800, 600),
            GeographyUnit.Meter);

        // This collection is used during drawing to pass labels in between layers, so we can track collisions
        var labels = new Collection<SimpleCandidate>();
        _zoningLayer.Draw(canvas, labels);
        
        // End drawing, we can now use the GeoImage
        canvas.EndDrawing();

        // Create a memory stream and save the GeoImage as a standard PNG formatted image
        var imageStream = new MemoryStream();
        geoImage.Save(imageStream, GeoImageFormat.Png);

        // Reset the image stream back to the beginning
        imageStream.Position = 0;

        return ImageSource.FromStream(() => imageStream);
    }
}