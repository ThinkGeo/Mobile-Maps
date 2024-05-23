using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace HowDoISample.Miscellaneous;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GenerateMapImage
{
    private bool _initialized;
    private ShapeFileFeatureLayer _zoningLayer;
    public GenerateMapImage()
    {
        InitializeComponent();
    }

    private async void GenerateMapImage_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        LoadingIndicator.IsRunning = true;
        LoadingLayout.IsVisible = true;

        MapImage.Source = await Task.Run(GenerateMapImage1);

        LoadingIndicator.IsRunning = false;
        LoadingLayout.IsVisible = false;
    }

    private ImageSource GenerateMapImage1()
    {
        var layersToDraw = new Collection<Layer>();

        // Create the background world maps using vector tiles stored locally in our MBTiles file and also set the styling though a json file
        var mbTilesLayer = new ThinkGeoMBTilesLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Mbtiles", "Frisco.mbtiles"),
            new Uri(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Json", "thinkgeo-world-streets-light.json")));

        mbTilesLayer.Open();
        layersToDraw.Add(mbTilesLayer);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        _zoningLayer = new ShapeFileFeatureLayer
        {
            ShapePathFilename = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Zoning.shp"),
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(2276, 3857)
            }
        };

        _zoningLayer.Open();

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        _zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoBrushes.Blue));
        _zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        layersToDraw.Add(_zoningLayer);

        // Create a GeoCanvas to do the drawing
        var canvas = GeoCanvas.CreateDefaultGeoCanvas();

        // Create a GeoImage as the image to draw on
        var geoImage = new GeoImage(800, 600);

        // Start the drawing by specifying the image, extent and map units
        canvas.BeginDrawing(geoImage, MapUtil.GetDrawingExtent(_zoningLayer.GetBoundingBox(), 800, 600),
            GeographyUnit.Meter);

        // This collection is used during drawing to pass labels in between layers, so we can track collisions
        var labels = new Collection<SimpleCandidate>();

        // Loop through all the layers and draw them to the GeoCanvas
        // The flush is to compact styles that use different drawing levels
        foreach (var layer in layersToDraw)
        {
            layer.Draw(canvas, labels);
            canvas.Flush();
        }

        // End drawing, we can now use the GeoImage
        canvas.EndDrawing();

        // Create a memory stream and save the GeoImage as a standard PNG formatted image
        var imageStream = new MemoryStream();
        geoImage.Save(imageStream, GeoImageFormat.Png);

        // Reset the image stream back to the beginning
        imageStream.Position = 0;

        //return ImageSource.FromStream(() => { return imageStream; });
        return ImageSource.FromStream(() => imageStream);
    }
}