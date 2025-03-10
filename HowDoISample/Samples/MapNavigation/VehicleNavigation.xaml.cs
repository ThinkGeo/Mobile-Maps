﻿using System.Collections.ObjectModel;
using System.Reflection;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapNavigation;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class VehicleNavigation
{
    private Collection<Vertex> _gpsPoints;
    private int _currentGpsPointIndex;
    private Marker _vehicleMarker;
    private readonly List<Vertex> _visitedVertices = [];
    private InMemoryFeatureLayer _visitedRoutesLayer;
    private bool _disposed;
    private bool _busy; // mark if the map is busy redrawing

    private bool _initialized;
    private CancellationTokenSource _cancellationTokenSource;
    private ThinkGeoRasterOverlay _backgroundOverlay;
        
    public VehicleNavigation()
    {
        InitializeComponent();
        Disappearing += UpdateVehicleLocation_Disappearing;
    }

    private void UpdateVehicleLocation_Disappearing(object sender, EventArgs e)
    {
        _disposed = true;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        _cancellationTokenSource = new CancellationTokenSource();
        // Add Cloud Maps as a background overlay
        _backgroundOverlay = new ThinkGeoRasterOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X2,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
        };
        MapView.Overlays.Add(_backgroundOverlay);
        MapView.EventView = new CustomEventView(); // Use the custom eventview which disables all the map events. 
        _gpsPoints = await InitGpsData();

        // Create the Layer for the Route
        var routeLayer = GetRouteLayerFromGpsPoints(_gpsPoints);
        _visitedRoutesLayer = new InMemoryFeatureLayer();
        _visitedRoutesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Green, 6, true);
        _visitedRoutesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var layerOverlay = new LayerGraphicsViewOverlay();
        layerOverlay.UpdateDataWhileTransforming = true;
        layerOverlay.Layers.Add(routeLayer);
        layerOverlay.Layers.Add(_visitedRoutesLayer);
        MapView.Overlays.Add(layerOverlay);

        // Create a marker overlay to show where the vehicle is
        var markerOverlay = new SimpleMarkerOverlay();
        // Create the marker of the vehicle
        _vehicleMarker = new ImageMarker
        {
            Position = new PointShape(_gpsPoints[0]),
            ImagePath = "vehicle_location.png",
            WidthRequest = 24,
            HeightRequest = 24
        };
        markerOverlay.Children.Add(_vehicleMarker);
        MapView.Overlays.Add(markerOverlay);

        MapView.CurrentExtentChangedInAnimation += MapViewOnCurrentExtentChangedInAnimation;

        AerialBackgroundCheckBox.CheckedChanged += AerialBackgroundCheckBoxOnCheckedChanged; 

        MapView.CenterPoint = new PointShape(_gpsPoints[0]);
        MapView.MapScale = 5000;
        await MapView.RefreshAsync();

        await ZoomToGpsPointsAsync();
    }

    private async Task ZoomToGpsPointsAsync()
    {
        for (_currentGpsPointIndex = 0; _currentGpsPointIndex < _gpsPoints.Count; _currentGpsPointIndex++)
        {
            while (_busy || _cancellationTokenSource.IsCancellationRequested)
                await Task.Delay(500); // delay zooming to GPS Points if the map is refreshing

            await ZoomToGpsPointAsync(_currentGpsPointIndex, _cancellationTokenSource.Token);
      
            if (_disposed)
                break;
        }
    }

    private void MapViewOnCurrentExtentChangedInAnimation(object sender, CurrentExtentChangedInAnimationMapViewEventArgs e)
    {
        if (_currentGpsPointIndex == 0)
            return;

        if (_visitedVertices.Count == 0)
            return;

        if (_currentGpsPointIndex >= _gpsPoints.Count)
            return;

        if (!MapUtil.IsSameDouble(e.FromResolution, e.ToResolution))
            return;

        var fromPoint = _gpsPoints[_currentGpsPointIndex - 1];
        var toPoint = _gpsPoints[_currentGpsPointIndex];

        var x = (toPoint.X - fromPoint.X) * e.Progress + fromPoint.X;
        var y = (toPoint.Y - fromPoint.Y) * e.Progress + fromPoint.Y;

        if (!MapUtil.IsSamePoint(_visitedVertices[^1], _gpsPoints[_currentGpsPointIndex - 1]))
        {
            _visitedVertices.RemoveAt(_visitedVertices.Count - 1);
        }
        UpdateVisitedRoutes(new Vertex(x, y));

        _vehicleMarker.Position = new PointShape(x, y);
    }

    private void UpdateVisitedRoutes(Vertex newVertex)
    {
        _visitedVertices.Add(newVertex);

        if (_visitedVertices.Count < 2)
            return;

        var lineShape = new LineShape(_visitedVertices);
        _visitedRoutesLayer.InternalFeatures.Clear();
        _visitedRoutesLayer.InternalFeatures.Add(new Feature(lineShape));
    }

    private async Task ZoomToGpsPointAsync(int gpsPointIndex, CancellationToken cancellationToken)
    {
        if (gpsPointIndex >= _gpsPoints.Count)
            return;

        var currentLocation = _gpsPoints[gpsPointIndex];
        var angle = GetRotationAngle(gpsPointIndex, _gpsPoints);

        var animationSettings = new AnimationSettings
        {
            Type = MapAnimationType.DrawWithAnimation,
            Length = 1000,
            Easing = Easing.Linear
        };

        var centerPoint = new PointShape(currentLocation);

        // Recenter the map to display the GPS location towards the bottom for improved visibility.
        centerPoint = MapUtil.OffsetPointWithScreenOffset(centerPoint, 0, 200, angle, MapView.MapScale, MapView.MapUnit);

        await MapView.ZoomToExtentAsync(centerPoint, MapView.MapScale, angle, animationSettings, OverlaysRenderSequenceType.Sequential, cancellationToken);
        UpdateVisitedRoutes(_gpsPoints[gpsPointIndex]);

        _vehicleMarker.Position = new PointShape(currentLocation.X, currentLocation.Y);
    }

    private static async Task<Collection<Vertex>> InitGpsData()
    {
        var gpsPoints = new Collection<Vertex>();

        // read the csv file from the embed resource. 
        var assembly = Assembly.GetExecutingAssembly();
        await using var stream = assembly.GetManifestResourceStream("HowDoISample.Data.Csv.vehicle-route.csv");
        if (stream == null) return null;

        // Convert GPS Points from Lat/Lon (srid:4326) to Spherical Mercator (Srid:3857), which is the projection of the base map
        var converter = new ProjectionConverter(4326, 3857);
        converter.Open();

        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var location = await reader.ReadLineAsync();
            if (location == null)
                continue;
            var posItems = location.Split(',');
            var lat = double.Parse(posItems[0]);
            var lon = double.Parse(posItems[1]);
            var vertexInSphericalMercator = converter.ConvertToExternalProjection(lon, lat);
            gpsPoints.Add(vertexInSphericalMercator);
        }
        converter.Close();
        return gpsPoints;
    }

    private static InMemoryFeatureLayer GetRouteLayerFromGpsPoints(Collection<Vertex> gpsPoints)
    {
        var lineShape = new LineShape();
        foreach (var vertex in gpsPoints)
        {
            lineShape.Vertices.Add(vertex);
        }

        // create the layers for the routes.
        var routeLayer = new InMemoryFeatureLayer();
        routeLayer.InternalFeatures.Add(new Feature(lineShape));
        routeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Yellow, 6, true);
        routeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        return routeLayer;
    }

    private static double GetRotationAngle(int currentIndex, IReadOnlyList<Vertex> gpsPoints)
    {
        Vertex currentLocation;
        Vertex nextLocation;

        if (currentIndex < gpsPoints.Count - 1)
        {
            currentLocation = gpsPoints[currentIndex];
            nextLocation = gpsPoints[currentIndex + 1];
        }
        else
        {
            currentLocation = gpsPoints[currentIndex - 1];
            nextLocation = gpsPoints[currentIndex];
        }

        double angle;
        if (nextLocation.X - currentLocation.X != 0)
        {
            var dx = nextLocation.X - currentLocation.X;
            var dy = nextLocation.Y - currentLocation.Y;

            angle = Math.Atan2(dx, dy) / Math.PI * 180; // get the angle in degrees 
            angle = -angle;
        }
        else
        {
            angle = nextLocation.Y - currentLocation.Y >= 0 ? 0 : 180;
        }
        return angle;
    }
    
    private async void AerialBackgroundCheckBoxOnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        while (_busy)
            await Task.Delay(500); // delay the operation is it's rendering 
        _busy = true;
        await RefreshCancellationTokenAsync();
        
        _backgroundOverlay.MapType = e.Value
            ? ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2
            : ThinkGeoCloudRasterMapsMapType.Light_V2_X2;
        await _backgroundOverlay.RefreshAsync();
        
        _busy = false;
    }
    
    private async void Button_OnClicked(object sender, EventArgs e)
    {
        while (_busy)
            await Task.Delay(500); // delay the operation is it's rendering 
        _busy = true;
        await RefreshCancellationTokenAsync();

        if (MapView.TiltAngle > 40)
            await MapView.TiltAsync(0, 1000, Easing.Default);
        else
            await MapView.TiltAsync(MapView.TiltAngle + 15, 1000, Easing.Default);
        _busy = false;
    }

    private async void ZoomInButton_OnClicked(object sender, EventArgs e)
    {
        while (_busy)
            await Task.Delay(500); // delay the operation is it's rendering 
        _busy = true;
        await RefreshCancellationTokenAsync();

        await MapView.ZoomInAsync(); // No cancellationToken passed in meaning this method cannot be canceled
        _busy = false;
    }
    
    private async void ZoomOutButton_OnClicked(object sender, EventArgs e)
    {
        while (_busy)
            await Task.Delay(500); // delay the operation is it's rendering 
        _busy = true;
        await RefreshCancellationTokenAsync();
            
        await MapView.ZoomOutAsync(); // No cancellationToken passed in meaning this method cannot be canceled
        _busy = false;
    }

    private async Task RefreshCancellationTokenAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }
}


/// <summary>
/// This customized class disables all the Touch events.
/// </summary>
class CustomEventView : EventView
{
    protected override TransformArguments TouchDownCore(TouchDownMapViewEventArgs e, IMapView mapView)
        => new TransformArguments();

    protected override TransformArguments TouchMoveCore(TouchMoveMapViewEventArgs e, IMapView mapView)
        => new TransformArguments();

    protected override TransformArguments TouchRotateCore(TouchRotateMapViewEventArgs e, IMapView mapView)
        => new TransformArguments();

    protected override TransformArguments TouchPointerDownCore(Pointer1DownMapViewEventArgs e, IMapView mapView)
        => new TransformArguments();

    protected override TransformArguments TouchPointerUpCore(Pointer1UpMapViewEventArgs e, IMapView mapView)
        => new TransformArguments();
}