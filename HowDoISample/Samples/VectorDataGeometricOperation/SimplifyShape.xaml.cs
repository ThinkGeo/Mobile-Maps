using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class SimplifyShape 
{
    private bool _initialized;
    public SimplifyShape()
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
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile","FriscoCityLimits.shp"));
        var simplifyLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project cityLimits layer to Spherical Mercator to match the map projection
        cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style simplifyLayer
        simplifyLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        simplifyLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits layer to a LayerOverlay
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add simplifyLayer to the layerOverlay
        layerOverlay.Layers.Add("simplifyLayer", simplifyLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Simplifies the first feature in the cityLimits layer and displays the results on the map.
    /// </summary>
    private async void SimplifyShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var simplifyLayer = (InMemoryFeatureLayer)layerOverlay.Layers["simplifyLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Simplify the first feature using the Douglas Peucker method
        var simplify = AreaBaseShape.Simplify(features[0].GetShape() as AreaBaseShape,
            Convert.ToInt32(Tolerance.Text), SimplificationType.DouglasPeucker);

        // Add the simplified shape into simplifyLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
        simplifyLayer.InternalFeatures.Clear();
        simplifyLayer.InternalFeatures.Add(new Feature(simplify));

        // Redraw the layerOverlay to see the simplified feature on the map
        await layerOverlay.RefreshAsync();
    }
}