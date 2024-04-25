using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class WorldMapsQueryCloudServices
{
    private bool _initialized;
    private MapsQueryCloudClient _mapsQueryCloudClient;
    public WorldMapsQueryCloudServices()
	{
		InitializeComponent();
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

        // Create a new feature layer to display the query shape used to perform the query
        var queryShapeFeatureLayer = new InMemoryFeatureLayer();

        // Add a point, line, and polygon style to the layer. These styles control how the query shape will be drawn
        queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            new PointStyle(PointSymbolType.Star, 20, GeoBrushes.Blue);
        queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.Blue);
        queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            new AreaStyle(GeoPens.Blue, new GeoSolidBrush(new GeoColor(10, GeoColors.Blue)));

        // Apply these styles on all zoom levels. This ensures our shapes will be visible on all zoom levels
        queryShapeFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a new feature layer to display the shapes returned by the query.
        var queriedFeaturesLayer = new InMemoryFeatureLayer();

        // Add a point, line, and polygon style to the layer. These styles control how the returned shapes will be drawn
        queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            new PointStyle(PointSymbolType.Star, 20, GeoBrushes.OrangeRed);
        queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(GeoPens.OrangeRed);
        queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(GeoPens.OrangeRed,
            new GeoSolidBrush(new GeoColor(10, GeoColors.OrangeRed)));
        queriedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the feature layers to an overlay, then add that overlay to the map
        var queriedFeaturesOverlay = new LayerOverlay();
        queriedFeaturesOverlay.Layers.Add("Queried Features Layer", queriedFeaturesLayer);
        queriedFeaturesOverlay.Layers.Add("Query Shape Layer", queryShapeFeatureLayer);
        MapView.Overlays.Add("Queried Features Overlay", queriedFeaturesOverlay);

        // Set the map extent to Frisco, TX
        MapView.CenterPoint = new PointShape(-10779600, 3915260);
        MapView.MapScale = 8000;

        // Add an event to handle new shapes that are drawn on the map
        //MapView.TrackOverlay.TrackEnded += OnShapeDrawn;
        MapView.TrackOverlay.TrackEnded += (_, args) =>
        {
            OnShapeDrawn(args.TrackShape);
        };

        // Initialize the MapsQueryCloudClient with our ThinkGeo Cloud credentials
        _mapsQueryCloudClient = new MapsQueryCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        // Create a sample shape and add it to the query shape layer
        var sampleShape = new RectangleShape(-10779877.70, 3915441.00, -10779248.97, 3915119.63);
        queryShapeFeatureLayer.InternalFeatures.Add(new Feature(sampleShape));
        // Run the world maps query

        MapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        await PerformWorldMapsQuery();

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Get features from the WorldMapsQuery service based on the UI parameters
    /// </summary>
    private async Task PerformWorldMapsQuery()
    {
        // Get the feature layers from the MapView
        var queriedFeaturesOverlay = (LayerOverlay)MapView.Overlays["Queried Features Overlay"];
        var queryShapeFeatureLayer = (InMemoryFeatureLayer)queriedFeaturesOverlay.Layers["Query Shape Layer"];
        var queriedFeaturesLayer = (InMemoryFeatureLayer)queriedFeaturesOverlay.Layers["Queried Features Layer"];

        // Show an error if trying to query with no query shape
        if (queryShapeFeatureLayer.InternalFeatures.Count == 0)
        {
            await DisplayAlert("Alert", "Please draw a shape to use for the query", "OK");
            return;
        }

        // Set the MapsQuery parameters based on the drawn query shape and the UI
        var queryShape = queryShapeFeatureLayer.InternalFeatures[0].GetShape();
        CloudMapsQueryResult result;

        // Perform the world maps query
        try
        {
            const int projectionInSrid = 3857;
            const string queryLayer = "buildings";
            result = await _mapsQueryCloudClient.GetFeaturesIntersectingAsync(queryLayer, queryShape, projectionInSrid,
                new CloudMapsQuerySpatialQueryOptions { MaxResults = 100 });

        }
        catch (Exception ex)
        {
            // Handle any errors returned from the maps query service
            if (ex is ArgumentException)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }

            await DisplayAlert("Alert", ex.Message, "OK");
            return;
        }


        if (result.Features.Count > 0)
        {
            // Add any features found by the query to the map
            foreach (var feature in result.Features) queriedFeaturesLayer.InternalFeatures.Add(feature);
        }
        else
        {
            await DisplayAlert("Alert", "No features found in the selected area", "OK");
        }
    }

    /// <summary>
    ///     Disable drawing mode and draw the new query shape on the map when finished drawing a shape
    /// </summary>
    private async void OnShapeDrawn(BaseShape drawnShape)
    {
        // Disable drawing mode and clear the drawing layer
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        ClearQueryShapes();

        // Get the query shape layer from the MapView
        var queriedFeaturesOverlay = (LayerOverlay)MapView.Overlays["Queried Features Overlay"];
        var queryShapeFeatureLayer = (InMemoryFeatureLayer)queriedFeaturesOverlay.Layers["Query Shape Layer"];

        // Add the newly drawn shape, then redraw the overlay
        queryShapeFeatureLayer.InternalFeatures.Add(new Feature(drawnShape));

        await PerformWorldMapsQuery();
        await queriedFeaturesOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Clear the query shapes from the map
    /// </summary>
    private void ClearQueryShapes()
    {
        // Get the query shape layer from the MapView
        var queriedFeaturesOverlay = (LayerOverlay)MapView.Overlays["Queried Features Overlay"];
        var queryShapeFeatureLayer = (InMemoryFeatureLayer)queriedFeaturesOverlay.Layers["Query Shape Layer"];
        var queriedFeaturesLayer = (InMemoryFeatureLayer)queriedFeaturesOverlay.Layers["Queried Features Layer"];

        // Clear the old query result and query shape from the map
        queriedFeaturesLayer.InternalFeatures.Clear();
        queryShapeFeatureLayer.InternalFeatures.Clear();
    }
}