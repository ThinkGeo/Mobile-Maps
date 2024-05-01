using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreatePointStyle
{
    private PointStyle _predefinedStyle;
    private PointStyle _imageStyle;
    private PointStyle _fontStyle;
    private LayerOverlay _layerOverlay;
    private bool _initialized;
    public CreatePointStyle()
    {
        InitializeComponent();
    }

    private async void CreatePointStyle_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));

        _layerOverlay = new LayerOverlay();
        // Project the layer's data to match the projection of the map
        hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Add the layer to a layer overlay
        _layerOverlay.Layers.Add(hotelsLayer);

        // Add the overlay to the map
        MapView.Overlays.Add(_layerOverlay);

        // Create a predefined point style
        _predefinedStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoBrushes.White, 2));

        // Create an image point style
        //_imageStyle = new PointStyle(new GeoImage(Path.Combine(FileSystem.Current.AppDataDirectory, "Resources", "Images", "hotel_icon.png")))
        _imageStyle = new PointStyle(new GeoImage(await FileSystem.OpenAppPackageFileAsync("hotel_icon.png")))
            {
                SymbolSize = 40,
                IsActive = false
            };

        // Create a font point style
        _fontStyle = new PointStyle(new GeoFont("Verdana", 16, DrawingFontStyles.Bold), "@", GeoBrushes.Black)
            {
                Mask = new AreaStyle(GeoBrushes.White),
                MaskType = MaskType.Circle,
                IsActive = false
            };

        // Add the point style to the collection of custom styles for ZoomLevel 1.
        hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(_predefinedStyle);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(_imageStyle);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(_fontStyle);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 9000;
        MapView.CenterPoint = new PointShape(-10777289, 3908744);
        await MapView.RefreshAsync();
    }

    /// <summary>
    /// Create a pointStyle using a PointSymbol and add it to the Hotels layer
    /// </summary>
    private async void PredefinedStyle_OnChecked(object sender, EventArgs e)
    {
        if (sender is RadioButton { IsChecked: false })
            return;

        if (_predefinedStyle == null)
            return;

        _predefinedStyle.IsActive = true;
        _imageStyle.IsActive = false;
        _fontStyle.IsActive = false;

        await _layerOverlay.RefreshAsync();
    }

    /// <summary>
    /// Create a pointStyle using an icon image and add it to the Hotels layer
    /// </summary>
    private async void ImageStyle_OnChecked(object sender, EventArgs e)
    {
        if (sender is RadioButton { IsChecked: false })
            return;

        if (_predefinedStyle == null)
            return;

        _predefinedStyle.IsActive = false;
        _imageStyle.IsActive = true;
        _fontStyle.IsActive = false;

        await _layerOverlay.RefreshAsync();
    }

    /// <summary>
    /// Create a pointStyle using a font symbol and add it to the Hotels layer
    /// </summary>
    private async void FontStyle_Checked(object sender, EventArgs e)
    {
        if (sender is RadioButton { IsChecked: false })
            return;

        if (_predefinedStyle == null)
            return;

        _predefinedStyle.IsActive = false;
        _imageStyle.IsActive = false;
        _fontStyle.IsActive = true;

        await _layerOverlay.RefreshAsync();
    }
}