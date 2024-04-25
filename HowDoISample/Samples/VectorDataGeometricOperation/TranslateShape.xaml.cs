using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class TranslateShape
{
    private bool _initialized;
    public TranslateShape()
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
        var translatedLayer = new InMemoryFeatureLayer();
        var layerOverlay = new LayerOverlay();

        // Project cityLimits layer to Spherical Mercator to match the map projection
        cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Style cityLimits layer
        cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Style translatedLayer
        translatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        translatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits layer to a LayerOverlay
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add translatedLayer to the layerOverlay
        layerOverlay.Layers.Add("translatedLayer", translatedLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Translates the first feature in the cityLimits layer and displays the result on the map.
    /// </summary>
    private async void OffsetTranslateShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Translate the first feature's shape by the X and Y values on the UI in meters
        var translate = BaseShape.TranslateByOffset(features[0].GetShape(), Convert.ToDouble(TranslateX.Text),
            Convert.ToDouble(TranslateY.Text), GeographyUnit.Meter, DistanceUnit.Meter);

        // Add the translated shape into translatedLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
        // underlying data using BeginTransaction and CommitTransaction instead.
        translatedLayer.InternalFeatures.Clear();
        translatedLayer.InternalFeatures.Add(new Feature(translate));

        // Redraw the layerOverlay to see the translated feature on the map
        await layerOverlay.RefreshAsync();
    }

    private async void DegreeTranslateShape_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

        // Query the cityLimits layer to get all the features
        cityLimits.Open();
        var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
        cityLimits.Close();

        // Translate the first feature's shape by the X and Y values on the UI in meters
        var translate = BaseShape.TranslateByDegree(features[0].GetShape(),
            Convert.ToDouble(TranslateDistance.Text), Convert.ToDouble(TranslateAngle.Text), GeographyUnit.Meter,
            DistanceUnit.Meter);

        // Add the translated shape into translatedLayer to display the result.
        // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
        // underlying data using BeginTransaction and CommitTransaction instead.
        translatedLayer.InternalFeatures.Clear();
        translatedLayer.InternalFeatures.Add(new Feature(translate));

        // Redraw the layerOverlay to see the translated feature on the map
        await layerOverlay.RefreshAsync();
    }

}