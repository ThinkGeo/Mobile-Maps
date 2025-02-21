using ThinkGeo.Core;
using ThinkGeo.UI.Maui;
using Timer = System.Timers.Timer;

namespace HowDoISample.MarkersAndPopups;

public partial class UsingMarkers
{
    private bool _initialized;
    private SimpleMarkerOverlay _markerOverlay;
    private PopupOverlay _popupOverlay;
    private readonly Timer _timer;
    private double _baseScale;

    public UsingMarkers()
    {
        InitializeComponent();
        _random = new Random(System.DateTime.Now.Microsecond);

        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.Elapsed += (sender, args) => UpdateMarkerStatus();
        MapView.CurrentExtentChanged += MapView_CurrentExtentChanged;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
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

        MapView.MapTools.Add(new ZoomMapTool());

        // Set the map extent        
        MapView.CenterPoint = new PointShape(-10777032, 3908560);
        MapView.MapScale = 10000;
        _baseScale = 10000;

        await AddHotelMarkersAsync();

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
        _timer.Start();
    }

    private void UpdateMarkerStatus()
    {
        foreach (var marker in _markerOverlay)
        {
            if (marker is not HotelMarker hotelMarker)
                continue;

            hotelMarker.StatusCode = GetRandomStatus();
        }
    }

    private void MapView_CurrentExtentChanged(object sender, CurrentExtentChangedMapViewEventArgs e)
    {
        if (!e.IsMapScaleChanged)
            return;

        var minScale = 0.5;
        var maxScale = 2;

        var currentScale = _baseScale / e.NewScale;
        currentScale = currentScale < minScale ? minScale : currentScale;
        currentScale = currentScale > maxScale ? maxScale : currentScale;

        foreach (var marker in _markerOverlay)
        {
            if (marker is not HotelMarker hotelMarker)
                continue;

            hotelMarker.Scale = currentScale;

            if (DeviceInfo.Platform != DevicePlatform.Android)
            {
                // iOS, WinUI, MacCatalyst
                hotelMarker.TranslationY = -hotelMarker.WidthRequest * 0.4 * hotelMarker.Scale;
            }
        }
    }

    private readonly Random _random;
    private int GetRandomStatus()
    {
        return _random.Next(1, 5);
    }

    private async Task AddHotelMarkersAsync()
    {
        var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile",
                "Hotels.shp"));
        hotelsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        var layerOverlay = new LayerGraphicsViewOverlay();
        layerOverlay.Layers.Add(hotelsLayer);
        MapView.Overlays.Add(layerOverlay);

        _markerOverlay = new SimpleMarkerOverlay();
        _popupOverlay = new PopupOverlay();

        // Open the layer so that we can begin querying
        hotelsLayer.Open();
        // Query all the hotel features
        var hotelFeatures = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

        // Add each hotel feature to the popupOverlay
        foreach (var feature in hotelFeatures)
        {
            var marker = new HotelMarker();
            marker.Position = feature.GetShape() as PointShape;
            marker.TranslationY = -marker.WidthRequest * 0.4;

            marker.Tapped += (sender, args) =>
            {
                var popup = new Popup
                {
                    Position = marker.Position,
                    Text = "Marker Tapped"
                };

                _popupOverlay.Children.Clear();
                _popupOverlay.Children.Add(popup);
            };
            _markerOverlay.Add(marker);
        }

        // Close the hotel layer
        hotelsLayer.Close();

        // Add the popupOverlay to the map and refresh
        MapView.Overlays.Add(_markerOverlay);
        MapView.Overlays.Add(_popupOverlay);

        await MapView.RefreshAsync();
    }
}
