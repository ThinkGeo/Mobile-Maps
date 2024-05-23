using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class CalculateShortestLineBetweenShapes
{
    private bool _initialized;
    public CalculateShortestLineBetweenShapes()
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

        var friscoParks = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));
        var stadiumLayer = new InMemoryFeatureLayer();
        var shortestLineLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project friscoParks layer to Spherical Mercator to match the map projection
        friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style friscoParks layer
        friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
        friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style stadiumLayer
        stadiumLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 16, GeoColors.White, 4);
        stadiumLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style shortestLineLayer
        shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
        shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add friscoParks layer to a LayerOverlay
        layerOverlay.Layers.Add("friscoParks", friscoParks);

        // Add stadiumLayer layer to a LayerOverlay
        layerOverlay.Layers.Add("stadiumLayer", stadiumLayer);

        // Add shortestLineLayer to the layerOverlay
        var shortestLineOverlay = new LayerOverlay();
        shortestLineOverlay.Layers.Add("shortestLineLayer", shortestLineLayer);
        MapView.Overlays.Add("shortestLineOverlay", shortestLineOverlay);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 50000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        // Add Toyota Stadium feature to stadiumLayer
        var stadium = new Feature(new PointShape(-10779651.500992451, 3915933.0023557912));
        stadiumLayer.InternalFeatures.Add(stadium);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Calculates the shortest line from the selected park to the stadium and displays its length and shows the line on
    ///     the map
    /// </summary>
    private async void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var shortestLineOverlay = (LayerOverlay)MapView.Overlays["shortestLineOverlay"];

        var friscoParks = (ShapeFileFeatureLayer)layerOverlay.Layers["friscoParks"];
        var stadiumLayer = (InMemoryFeatureLayer)layerOverlay.Layers["stadiumLayer"];
        var shortestLineLayer = (InMemoryFeatureLayer)shortestLineOverlay.Layers["shortestLineLayer"];

        // Query the friscoParks layer to get the first feature closest to the map tap event
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);
        var park = friscoParks.QueryTools.GetFeaturesNearestTo(pointInWorldCoordinate, GeographyUnit.Meter, 1,
            ReturningColumnsType.NoColumns).First();

        // Get the stadium feature from the stadiumLayer
        var stadium = stadiumLayer.InternalFeatures[0];

        // Get the shortest line from the selected park to the stadium
        var shortestLine = park.GetShape().GetShortestLineTo(stadium, GeographyUnit.Meter);

        // Show the shortestLine on the map
        shortestLineLayer.InternalFeatures.Clear();
        shortestLineLayer.InternalFeatures.Add(new Feature(shortestLine));
        await shortestLineOverlay.RefreshAsync();

        // Get the area of the first feature
        var length = shortestLine.GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

        // Display the shortestLine's length in the distanceResult TextBox
        DistanceResult.Text = $"Distance: {length:f3} km";
    }
}