using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class CalculateCenterPoint
{
    private bool _initialized;
    public CalculateCenterPoint()
	{
		InitializeComponent();
        MapView.SingleTap += MapView_SingleTap;
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

        // Create a feature layer to hold the Census Housing data
        var censusHousingLayer = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco 2010 Census Housing Units.shp"));

        // Project censusHousing layer to Spherical Mercator to match the map projection
        var projectionConverter = new ProjectionConverter(2276, 3857);
        censusHousingLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the censusHousing layer
        censusHousingLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        censusHousingLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);

        var censusHousingOverlay = new LayerOverlay();
        censusHousingOverlay.Layers.Add("CensusHousingLayer", censusHousingLayer);
        MapView.Overlays.Add("CensusHousingOverlay", censusHousingOverlay);

        // Create a layer to hold the centerPointLayer and Style it
        var centerPointLayer = new InMemoryFeatureLayer();
        centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 12, GeoColors.White, 4);
        centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.Black, 2);
        centerPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var centerPointOverlay = new LayerOverlay();
        centerPointOverlay.Layers.Add("CenterPointLayer", centerPointLayer);
        MapView.Overlays.Add("CenterPointOverlay", centerPointOverlay);

        // Set the map extent to the censusHousing layer bounding box
        MapView.CenterPoint = new PointShape(-10777600, 3920260);
        MapView.MapScale = 800000;              

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Calculates the center point of a feature
    /// </summary>
    /// <param name="feature"> The target feature to calculate its center point</param>
    private async Task CalculateCenterPointFunction(Feature feature)
    {
        var centerPointOverlay = (LayerOverlay)MapView.Overlays["CenterPointOverlay"];
        var centerPointLayer = (InMemoryFeatureLayer)centerPointOverlay.Layers["CenterPointLayer"];

        // Get the CenterPoint of the selected feature
        var centerPoint = CentroidCenter.IsChecked 
            ? feature.GetShape().GetCenterPoint() // Accurate, but can be relatively slower on extremely complex shapes
            : feature.GetBoundingBox().GetCenterPoint(); // BoundingBox method. Less accurate, but much faster

        // Show the centerPoint on the map        
        centerPointLayer.InternalFeatures.Clear();
        centerPointLayer.InternalFeatures.Add("selectedFeature", feature);
        centerPointLayer.InternalFeatures.Add("centerPoint", new Feature(centerPoint));

        // Refresh the overlay to show the results
        await centerPointOverlay.RefreshAsync();
    }

    // Gets the closest feature from the tap event and calculates the center point
    private async void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var censusHousingOverlay = (LayerOverlay)MapView.Overlays["CensusHousingOverlay"];
        var censusHousingLayer = (ShapeFileFeatureLayer)censusHousingOverlay.Layers["CensusHousingLayer"];
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);

        // Query the censusHousing layer to get the first feature closest to the map tap event
        var feature = censusHousingLayer.QueryTools.GetFeaturesNearestTo(pointInWorldCoordinate, 
            GeographyUnit.Meter,1,ReturningColumnsType.NoColumns).First();

        await CalculateCenterPointFunction(feature);
    }

    // RadioButton checked event that will recalculate the center point so long as a feature was already selected
    private async void RadioButton_Checked(object sender, EventArgs e)
    {
        var centerPointOverlay = (LayerOverlay)MapView.Overlays["CenterPointOverlay"];
        var centerPointLayer = (InMemoryFeatureLayer)centerPointOverlay.Layers["CenterPointLayer"];

        // Recalculate the center point if a feature has already been selected
        if (centerPointLayer.InternalFeatures.Contains("selectedFeature"))
            await CalculateCenterPointFunction(centerPointLayer.InternalFeatures["selectedFeature"]);
    }
}