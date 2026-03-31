using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class CalculateLength
{
    private bool _initialized;
    public CalculateLength()
    {
        InitializeComponent();
        Map.SingleTap += Map_SingleTap;
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        Map.Overlays.Add(backgroundOverlay);

        Map.MapTools.Add(new ZoomMapTool());

        var friscoTrailsLayer = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hike_Bike.shp"));

        // Project friscoTrails layer to Spherical Mercator to match the map projection
        var projectionConverter = new ProjectionConverter(2276, 3857);
        friscoTrailsLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the friscoTrails layer
        friscoTrailsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        friscoTrailsLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Orange, 2, false);

        var friscoTrailsOverlay = new LayerOverlay();
        friscoTrailsOverlay.Layers.Add("FriscoTrailsLayer", friscoTrailsLayer);
        Map.Overlays.Add("FriscoTrailsOverlay", friscoTrailsOverlay);

        // Create a layer to hold the selectedLineLayer found by the spatial query
        var selectedLineLayer = new InMemoryFeatureLayer();
        selectedLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
        selectedLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var selectedLineOverlay = new LayerOverlay();
        selectedLineOverlay.Layers.Add("SelectedLineLayer", selectedLineLayer);
        Map.Overlays.Add("SelectedLineOverlay", selectedLineOverlay);

        // Set the map extent
        Map.CenterPoint = new PointShape(-10777600, 3915260);
        Map.MapScale = 200000;

        // Add LayerOverlay to Map

        await Map.RefreshAsync();
    }

    /// <summary>
    ///     Calculates the length of a line selected on the map and displays it in the lengthResult TextBox
    /// </summary>
    private async void Map_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var friscoTrailsOverlay = (LayerOverlay)Map.Overlays["FriscoTrailsOverlay"];
        var friscoTrailsLayer = (ShapeFileFeatureLayer)friscoTrailsOverlay.Layers["FriscoTrailsLayer"];

        var selectedLineOverlay = (LayerOverlay)Map.Overlays["SelectedLineOverlay"];
        var selectedLineLayer = (InMemoryFeatureLayer)selectedLineOverlay.Layers["SelectedLineLayer"];

        // Query the friscoTrails layer to get the first feature closest to the map tap event
        var pointInWorldCoordinate = Map.ToWorldCoordinate(e.X, e.Y);
        var feature = friscoTrailsLayer.QueryTools.GetFeaturesNearestTo(pointInWorldCoordinate, GeographyUnit.Meter, 1,
            ReturningColumnsType.NoColumns).First();

        // Show the selected feature on the map
        selectedLineLayer.InternalFeatures.Clear();
        selectedLineLayer.InternalFeatures.Add(feature);
        await selectedLineOverlay.RefreshAsync();

        // Get the length of the first feature
        var length = ((LineBaseShape)feature.GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

        // Display the selectedLine's length in the lengthResult TextBox
        LengthResult.Text = $"Length: {length:f3} km";
    }
}
