using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class UnionShapes
{
    private bool _initialized;
    public UnionShapes()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

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

        var dividedCityLimits = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "FriscoCityLimitsDivided.shp"));
        var unionLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project dividedCityLimits layer to Spherical Mercator to match the map projection
        dividedCityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style dividedCityLimits layer
        dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(128, GeoColors.LightOrange), GeoColors.DimGray, 2);
        dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("NAME",
            "Segoe UI", 12, DrawingFontStyles.Bold, GeoColors.Black, GeoColors.White, 2);
        dividedCityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style unionLayer
        unionLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray, 2);
        unionLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add dividedCityLimits to a LayerOverlay
        layerOverlay.Layers.Add("dividedCityLimits", dividedCityLimits);

        // Add unionLayer to the layerOverlay
        layerOverlay.Layers.Add("unionLayer", unionLayer);

        //Set the maps current extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Unions all the features in the dividedCityLimits layer and displays the results on the map
    /// </summary>
    private async void UnionShapes_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var dividedCityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["dividedCityLimits"];
        var unionLayer = (InMemoryFeatureLayer)layerOverlay.Layers["unionLayer"];

        // Query the dividedCityLimits layer to get all the features
        dividedCityLimits.Open();
        var features = dividedCityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        dividedCityLimits.Close();

        // Union all the features into a single Multipolygon Shape
        var union = AreaBaseShape.Union(features);

        // Add the union shape into unionLayer to display the result.
        // If this were to be a permanent change to the dividedCityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
        unionLayer.InternalFeatures.Clear();
        unionLayer.InternalFeatures.Add(new Feature(union));

        // Hide the dividedCityLimits layer
        dividedCityLimits.IsVisible = false;

        // Redraw the layerOverlay to see the union features on the map
        await layerOverlay.RefreshAsync();
    }
}