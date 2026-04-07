using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapNavigation;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ZoomToExtent
{
    private bool _initialized;
    private ShapeFileFeatureLayer _friscoBoundary;
    private AnimationSettings _animationSettings;

    public ZoomToExtent()
    {
        InitializeComponent();
        mapView.MapRotationChanged += Map_MapRotationChanged;
    }

    private void Map_MapRotationChanged(object sender, MapRotationChangedMapViewEventArgs e)
    {
        CompassButton.Rotation = (float)(mapView.MapRotation);
    }

    private async void Map_SizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        _animationSettings = new AnimationSettings { Duration = 1000 };
        // Set the map's unit to meter (the unit for Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Add ThinkGeo Cloud Maps as the background
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };

        mapView.Overlays.Add(backgroundOverlay);

        // Load the Frisco data to a layer
        var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Subdivisions.shp");
        // ReSharper disable once UseObjectOrCollectionInitializer
        _friscoBoundary = new ShapeFileFeatureLayer(filePath);
        // Convert the Frisco shape file from its native projection (srid:2276) to Spherical Mercator(srid:3857), to match the background
        _friscoBoundary.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style the data, and apply it from zoomlevel01 to zoomlevel20
        _friscoBoundary.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(16, GeoColors.Blue), GeoColors.DimGray, 2);
        _friscoBoundary.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add Frisco data to a LayerOverlay and add it to the map
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(_friscoBoundary);
        mapView.Overlays.Add(layerOverlay);

        mapView.IsRotationEnabled = true;

        // Set the map extent
        mapView.CenterPoint = new PointShape(-10777932, 3912260);
        mapView.MapScale = 100000;

        SetupButtonEvents();
        await mapView.RefreshAsync();
    }

    private void SetupButtonEvents()
    {
        SimultaneouslyDrawingCheckBox.CheckedChanged += (_, _) =>
            _animationSettings.Type = SimultaneouslyDrawingCheckBox.IsChecked
                ? MapAnimationType.DrawWithAnimation
                : MapAnimationType.DrawAfterAnimation;

        CompassButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
            await mapView.ZoomToExtentAsync(mapView.CenterPoint, mapView.MapScale, 0, _animationSettings));

        DefaultExtentButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
            await mapView.ZoomToExtentAsync(new PointShape(-10777932, 3912260), 100000, 0, _animationSettings));

        ZoomToScaleButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
            await mapView.ZoomToAsync(20000, _animationSettings));

        ZoomToLayerButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
            await mapView.ZoomToAsync(_friscoBoundary.GetBoundingBox(), _animationSettings));

        ZoomToFeatureButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
        {
            var feature = _friscoBoundary.FeatureSource.GetFeatureById("1", ReturningColumnsType.NoColumns);
            await mapView.ZoomToAsync(feature, _animationSettings);
        });

        CenterAtPointButton.Clicked += async (_, _) => await ExecuteWithoutCancellationException(async () =>
        {
            var pointInMercator = ProjectionConverter.Convert(4326, 3857, new PointShape(-96.82, 33.15));
            await mapView.CenterAtAsync(pointInMercator, _animationSettings);
        });
    }

    private static async Task ExecuteWithoutCancellationException(Func<Task> asyncFunc)
    {
        try
        {
            await asyncFunc();
        }
        catch (TaskCanceledException)
        {
            // do nothing when asyncFunc is cancelled. 
        }
    }
}
