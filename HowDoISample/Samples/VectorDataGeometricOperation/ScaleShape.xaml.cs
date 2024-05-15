using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class ScaleShape
{
    private bool _initialized;
    private double _scaleFactor = 1;

    public ScaleShape()
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

        ShapeFileFeatureLayer.BuildIndexFile(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "FriscoCityLimits.shp"));

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
        var scaledLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project cityLimits layer to Spherical Mercator to match the map projection
        cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style scaledLayer
        scaledLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        scaledLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits layer to a LayerOverlay
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add scaledLayer to the layerOverlay
        layerOverlay.Layers.Add("scaledLayer", scaledLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Scales the first feature in the cityLimits layer and displays the result on the map.
    /// </summary>
    private async void ScaleShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var scaledLayer = (InMemoryFeatureLayer)layerOverlay.Layers["scaledLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Scale the feature by 1.1
        _scaleFactor *= 1.1;
        var scale = BaseShape.ScaleTo(features[0].GetShape(), _scaleFactor);

        // Add the scaled shape into scaledLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
        // underlying data using BeginTransaction and CommitTransaction instead.
        scaledLayer.InternalFeatures.Clear();
        scaledLayer.InternalFeatures.Add(new Feature(scale));

        // Redraw the layerOverlay to see the scaled feature on the map
        await layerOverlay.RefreshAsync();
    }
}