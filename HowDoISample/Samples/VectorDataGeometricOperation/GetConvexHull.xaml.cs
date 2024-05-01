using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class GetConvexHull
{
    private bool _initialized;
    public GetConvexHull()
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
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "FriscoCityLimits.shp"))
        {
            FeatureSource =
            {
                // Project cityLimits layer to Spherical Mercator to match the map projection
                ProjectionConverter = new ProjectionConverter(2276, 3857)
            }
        };

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style the convexHullLayer
        var convexHullLayer = new InMemoryFeatureLayer();
        convexHullLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        convexHullLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add convexHullLayer to the layerOverlay
        var convexHullOverlay = new LayerOverlay();
        convexHullOverlay.Layers.Add("convexHullLayer", convexHullLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);
        MapView.Overlays.Add("convexHullOverlay", convexHullOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Gets Convex Hull of the first feature in the cityLimits layer and adds them to the convexHullLayer to display on
    ///     the map
    /// </summary>
    private async void ShapeConvexHull_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var convexHullOverlay = (LayerOverlay)MapView.Overlays["convexHullOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var convexHullLayer = (InMemoryFeatureLayer)convexHullOverlay.Layers["convexHullLayer"];

        // Query the cityLimits layer to get the first feature
        cityLimits.Open();
        var feature = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
        cityLimits.Close();

        // Get the convex hull of the feature
        var convexHull = feature.GetConvexHull();

        // Add the convexHull into an InMemoryFeatureLayer to display the result.
        convexHullLayer.InternalFeatures.Clear();
        convexHullLayer.InternalFeatures.Add(convexHull);

        // Redraw the layerOverlay to see the convexHull feature on the map
        await convexHullOverlay.RefreshAsync();
    }
}