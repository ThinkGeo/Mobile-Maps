using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class ProjectionCloudServices
{
    private bool _initialized;
    private ProjectionCloudClient _projectionCloudClient;
    public ProjectionCloudServices()
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

        // Create a new feature layer to display the shapes we will be reprojected
        var reprojectedFeaturesLayer = new InMemoryFeatureLayer();

        // Add a point, line, and polygon style to the layer. These styles control how the shapes will be drawn
        reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple);
        reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.MediumPurple, 6, false);
        reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.MediumPurple), GeoColors.MediumPurple,
                2);

        // Apply these styles on all zoom levels. This ensures our shapes will be visible on all zoom levels
        reprojectedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layer to an overlay
        var reprojectedFeaturesOverlay = new LayerOverlay();
        reprojectedFeaturesOverlay.Layers.Add("Reprojected Features Layer", reprojectedFeaturesLayer);

        // Add the overlay to the map
        MapView.Overlays.Add("Reprojected Features Overlay", reprojectedFeaturesOverlay);

        // Set the map extent        
        MapView.CenterPoint = new PointShape(-10779600, 3915260);
        MapView.MapScale = 4000;

        // Initialize the ProjectionCloudClient with our ThinkGeo Cloud credentials
        _projectionCloudClient = new ProjectionCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Draw reprojected features on the map
    /// </summary>
    private async Task ClearMapAndAddFeatures(Collection<Feature> features)
    {
        // Get the layer we prepared from the MapView
        var reprojectedFeaturesOverlay = (LayerOverlay)MapView.Overlays["Reprojected Features Overlay"];
        var reprojectedFeatureLayer = (InMemoryFeatureLayer)reprojectedFeaturesOverlay.Layers["Reprojected Features Layer"];

        // Clear old features from the feature layer and add the newly reprojected features
        reprojectedFeatureLayer.InternalFeatures.Clear();

        foreach (var sphericalMercatorFeature in features)
            reprojectedFeatureLayer.InternalFeatures.Add(sphericalMercatorFeature);

        // Set the map extent to zoom into the feature and refresh the map
        reprojectedFeatureLayer.Open();
        //mapView.CurrentExtent = reprojectedFeatureLayer.GetBoundingBox();
        await MapView.ZoomToExtentAsync(reprojectedFeatureLayer.GetBoundingBox().GetCenterPoint(),
            4000, 0, new AnimationSettings());
        await reprojectedFeaturesOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Use the ProjectionCloudClient to re-project multiple different features
    /// </summary>
    private async void ReProjectMultipleFeatures_Click(object sender, EventArgs e)
    {
        // Create features based on the WKT in the text box in the UI
        var decimalDegreeFeatures = new Collection<Feature>();
        var wktStrings = new Collection<string>
        {
            "POINT(-96.834516 33.150083)",
            "LINESTRING(-96.83559 33.149,-96.835866046134 33.1508413556856,-96.835793626491 33.1508974965687,-96.8336008970734 33.1511063402186,-96.83356 33.15109,-96.83328 33.14922)",
            "POLYGON((-96.83582 33.1508,-96.83578 33.15046,-96.83353 33.15068,-96.83358 33.15102,-96.83582 33.1508))"
        };

        foreach (var wktString in wktStrings)
            try
            {
                var wktFeature = new Feature(wktString);
                decimalDegreeFeatures.Add(wktFeature);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        // Use the ProjectionCloudClient to convert between Decimal Degrees (4326) and Spherical Mercator (3857)
        var sphericalMercatorFeatures = await _projectionCloudClient.ProjectAsync(decimalDegreeFeatures, 4326, 3857);

        // Add the reprojected features to the map
        await ClearMapAndAddFeatures(sphericalMercatorFeatures);
    }
}