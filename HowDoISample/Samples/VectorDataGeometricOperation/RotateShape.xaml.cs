using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class RotateShape
{
    private bool _initialized;
    private float _degrees;

    public RotateShape()
    {
        InitializeComponent();
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

        var cityLimits = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "FriscoCityLimits.shp"));
        var rotatedLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project cityLimits layer to Spherical Mercator to match the map projection
        cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style rotatedLayer
        rotatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        rotatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits layer to a LayerOverlay
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add rotatedLayer to the layerOverlay
        layerOverlay.Layers.Add("rotatedLayer", rotatedLayer);

        // Set the map extent 
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Rotates the first feature in the cityLimits layer and displays the result on the map.
    /// </summary>
    private async void RotateShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var rotatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["rotatedLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Rotate the feature by 45 degrees
        _degrees += 45;
        var rotate = BaseShape.Rotate(features[0], features[0].GetShape().GetCenterPoint(), _degrees);

        // Add the rotated shape into rotatedLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
        rotatedLayer.InternalFeatures.Clear();
        rotatedLayer.InternalFeatures.Add(new Feature(rotate));

        // Redraw the layerOverlay to see the rotated feature on the map
        await layerOverlay.RefreshAsync();
    }
}