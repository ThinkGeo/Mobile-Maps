using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class GetEnvelope
{
    private bool _initialized;
    public GetEnvelope()
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

        // Style the envelopeLayer
        var envelopeLayer = new InMemoryFeatureLayer();
        envelopeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
        envelopeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add cityLimits to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add("cityLimits", cityLimits);

        // Add envelopeLayer to the layerOverlay
        var envelopeOverlay = new LayerOverlay();
        envelopeOverlay.Layers.Add("envelopeLayer", envelopeLayer);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10778600, 3915260);
        MapView.MapScale = 240000;

        // Add LayerOverlay to Map
        MapView.Overlays.Add("layerOverlay", layerOverlay);
        MapView.Overlays.Add("envelopeOverlay", envelopeOverlay);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Gets the Envelope of the first feature in the cityLimits layer and adds them to the envelopeLayer to display on the
    ///     map
    /// </summary>
    private async void ShapeEnvelope_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var envelopeOverlay = (LayerOverlay)MapView.Overlays["envelopeOverlay"];

        var cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
        var envelopeLayer = (InMemoryFeatureLayer)envelopeOverlay.Layers["envelopeLayer"];

        // Query the cityLimits layer to get the first feature
        cityLimits.Open();
        var feature = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
        cityLimits.Close();

        // Get the bounding box (or envelope) of the feature
        var envelope = feature.GetBoundingBox();

        // Add the envelope shape into an InMemoryFeatureLayer to display the result.
        envelopeLayer.InternalFeatures.Clear();
        envelopeLayer.InternalFeatures.Add(new Feature(envelope));

        // Redraw the layerOverlay to see the envelope feature on the map
        await envelopeOverlay.RefreshAsync();
    }
}