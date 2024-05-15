using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class BufferShape
{
    private bool _initialized;
    public BufferShape()
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

        var cityLimits = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "FriscoCityLimits.shp"));
        var bufferLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project cityLimits layer to Spherical Mercator to match the map projection
        cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style the bufferLayer
        bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        bufferLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits to a LayerOverlay
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add bufferLayer to the layerOverlay
        layerOverlay.Layers.Add("bufferLayer", bufferLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Buffers the first feature in the cityLimits layer and adds them to the bufferLayer to display on the map
    /// </summary>
    private async void BufferShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var bufferLayer = (InMemoryFeatureLayer)layerOverlay.Layers["bufferLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Buffer the first feature by 1000 meters
        var buffer = features[0]
            .Buffer(1000, GeographyUnit.Meter, DistanceUnit.Meter);

        // Add the buffer shape into an InMemoryFeatureLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
        bufferLayer.InternalFeatures.Clear();
        bufferLayer.InternalFeatures.Add(buffer);

        // Redraw the layerOverlay to see the buffered features on the map
        await layerOverlay.RefreshAsync();
    }
}