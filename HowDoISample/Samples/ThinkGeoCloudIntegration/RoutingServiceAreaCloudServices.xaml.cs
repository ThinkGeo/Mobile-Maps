using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class RoutingServiceAreaCloudServices
{
    private bool _initialized;
    private RoutingCloudClient _routingCloudClient;
    private Collection<TimeSpan> _serviceAreaIntervals;
    public RoutingServiceAreaCloudServices()
	{
		InitializeComponent();
        MapView.SingleTap += MapView_SingleTap;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Set the map's unit of measurement to meters (Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a new feature layer to display the service areas
        var serviceAreasLayer = new InMemoryFeatureLayer();

        // Add a class break style to display the service areas
        // We will display a different color for 15, 30, 45, and 60 minute travel times
        var serviceAreasClassBreaks = new Collection<ClassBreak>
        {
            new(15,
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Green), GeoColors.Green)),
            new(30,
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Yellow), GeoColors.Yellow)),
            new(45,
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Orange), GeoColors.Orange)),
            new(60,
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Red), GeoColors.Red))
        };

        var serviceAreasClassBreakStyle = new ClassBreakStyle("TravelTimeFromCenterPoint",
            BreakValueInclusion.IncludeValue, serviceAreasClassBreaks);
        serviceAreasLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(serviceAreasClassBreakStyle);
        serviceAreasLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set up the legend adornment
        SetUpLegendAdornment(serviceAreasClassBreaks);

        // Add the layer to an overlay, and add the overlay to the mapview
        var serviceAreaOverlay = new LayerOverlay();
        serviceAreaOverlay.Layers.Add("Service Area Layer", serviceAreasLayer);
        MapView.Overlays.Add("Service Area Overlay", serviceAreaOverlay);

        // Add a simple marker overlay to display the center point of the service area
        var serviceAreaMarkerOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("Service Area Marker Overlay", serviceAreaMarkerOverlay);

        MapView.CenterPoint = new PointShape(-10777600, 3915260);
        MapView.MapScale = 1500000;
        
        // Create a new set of time spans for 15, 30, 45, 60 minutes. These will be used to create the class breaks for the routing service area request
        _serviceAreaIntervals =
            [
                new TimeSpan(0, 15, 0),
                new TimeSpan(0, 30, 0),
                new TimeSpan(0, 45, 0),
                new TimeSpan(1, 0, 0)
            ];

        // Initialize the RoutingCloudClient with our ThinkGeo Cloud Client credentials
        _routingCloudClient = new RoutingCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        // Run a sample query
        var samplePoint = new PointShape(-10777600, 3915260);
        GetAndDrawServiceArea(samplePoint);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Get the service area from a given point on the map
    /// </summary>
    private async Task<CloudRoutingGetServiceAreaResult> GetServiceArea(PointShape centerPoint)
    {
        // Set options for the service area request
        // We can control options like Travel Direction and Contour Granularity
        var options = new CloudRoutingGetServiceAreaOptions
        {
            DistanceUnit = DistanceUnit.Meter
        };

        // Set the srid for the query to 3857 (Spherical Mercator)
        const int srid = 3857;

        // Run the service area query
        // Pass in the service area intervals. These will be used as the service areas for the query (15, 30, 45 60 minutes)
        var getServiceAreaResult =
            await _routingCloudClient.GetServiceAreaAsync(centerPoint, srid, _serviceAreaIntervals, options);
        return getServiceAreaResult;
    }

    /// <summary>
    ///     Draw the ServiceArea polygons on the map
    /// </summary>
    private async Task DrawServiceArea(CloudRoutingGetServiceAreaResult result)
    {
        var serviceAreaResult = result.ServiceAreaResult;

        // Get the simple marker overlay from the map
        var serviceAreaMarkerOverlay = (SimpleMarkerOverlay)MapView.Overlays["Service Area Marker Overlay"];

        // Clear the previous markers
        serviceAreaMarkerOverlay.Children.Clear();

        // Add the service area center point marker to the map
        serviceAreaMarkerOverlay.Children.Add(
            CreateNewMarker(new PointShape(serviceAreaResult.Waypoint.Coordinate)));

        // Get the service area polygons layer from the map
        var serviceAreaOverlay = (LayerOverlay)MapView.Overlays["Service Area Overlay"];
        var serviceAreaLayer = (InMemoryFeatureLayer)serviceAreaOverlay.Layers["Service Area Layer"];

        // Clear the previous polygons
        serviceAreaLayer.InternalFeatures.Clear();

        // Add the new service area polygons to the map
        for (var i = 0; i < _serviceAreaIntervals.Count; i++)
        {
            // Add a 'TravelTimeFromCenterPoint' attribute for the class break style
            var columnValues = new Dictionary<string, string>
            {
                { "TravelTimeFromCenterPoint", _serviceAreaIntervals[i].TotalMinutes.ToString(CultureInfo.InvariantCulture) }
            };

            // Add each polygon to the feature layer
            var serviceAreaPolygon = serviceAreaResult.ServiceAreas[i];
            serviceAreaLayer.InternalFeatures.Add(new Feature(serviceAreaPolygon, columnValues));
        }

        await serviceAreaMarkerOverlay.RefreshAsync();
        await serviceAreaOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Draw the new point and service area on the map
    /// </summary>
    private async void GetAndDrawServiceArea(PointShape point)
    {
        // Show a loading graphic to let users know the request is running
        LoadingIndicator.IsRunning = true;
        LoadingLayout.IsVisible = true;

        // Run the service area query
        var getServiceAreaResult = await GetServiceArea(point);

        // Hide the loading graphic
        LoadingIndicator.IsRunning = false;
        LoadingLayout.IsVisible = false;

        // Handle an exception returned from the service
        if (getServiceAreaResult.Exception != null)
        {
            await DisplayAlert("Alert", getServiceAreaResult.Exception.Message, "Error");
            return;
        }

        // Draw the result on the map
        await DrawServiceArea(getServiceAreaResult);
    }

    private void SetUpLegendAdornment(Collection<ClassBreak> classBreaks)
    {
        //Create a legend adornment based on the service area class breaks
        var legend = new LegendAdornmentLayer
        {
            // Set up the legend adornment
            Title = new LegendItem
            {
                TextStyle = new TextStyle("Travel Times", new GeoFont("Verdana", 10, DrawingFontStyles.Bold),
                GeoBrushes.Black)
            },
            Location = AdornmentLocation.LowerRight
        };
        MapView.AdornmentOverlay.Layers.Add(legend);

        // Add a LegendItems to the legend adornment for each ClassBreak
        foreach (var classBreak in classBreaks)
        {
            var legendItem = new LegendItem
            {
                ImageStyle = classBreak.DefaultAreaStyle,
                TextStyle = new TextStyle($"<{classBreak.Value} minutes", new GeoFont("Verdana", 10),
                    GeoBrushes.Black)
            };
            legend.LegendItems.Add(legendItem);
        }
    }

    /// <summary>
    ///     Perform the service area query when a new point is drawn
    /// </summary>
    private void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);
        GetAndDrawServiceArea(pointInWorldCoordinate);
    }

    /// <summary>
    ///     Create a new map marker using preloaded image assets
    /// </summary>
    private static Marker CreateNewMarker(PointShape point)
    {
        return new ImageMarker
        {
            Position = point,
            ImagePath = "marker.png",
            TranslationY = -17,
            WidthRequest = 20,
            HeightRequest = 34
        };
    }
}