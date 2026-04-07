using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class CalculateArea
{
    private bool _initialized;
    public CalculateArea()
    {
        InitializeComponent();
        mapView.SingleTap += Map_SingleTap;
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        mapView.Overlays.Add(backgroundOverlay);

        mapView.MapTools.Add(new ZoomMapTool());

        // Create a feature layer to hold the Frisco Parks data
        var friscoParksLayer = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));

        // Project friscoParks layer to Spherical Mercator to match the map projection
        var projectionConverter = new ProjectionConverter(2276, 3857);
        friscoParksLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Style friscoParks layer
        friscoParksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
        friscoParksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var friscoParkOverlay = new LayerOverlay();
        friscoParkOverlay.Layers.Add("FriscoParksLayer", friscoParksLayer);
        mapView.Overlays.Add("FriscoParksOverlay", friscoParkOverlay);

        // Create a layer to hold features found by the selected area query
        var selectedAreaLayer = new InMemoryFeatureLayer();
        selectedAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
        selectedAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var selectedAreaOverlay = new LayerOverlay();
        selectedAreaOverlay.Layers.Add("SelectedAreaLayer", selectedAreaLayer);
        mapView.Overlays.Add("SelectedAreaOverlay", selectedAreaOverlay);

        // Set the map extent
        mapView.CenterPoint = new PointShape(-10777600, 3915260);
        mapView.MapScale = 40000;

        await mapView.RefreshAsync();
    }

    /// <summary>
    ///     Calculates the area of a feature selected on the map and displays it in the areaResult TextBox
    /// </summary>
    private async void Map_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var friscoParkOverlay = (LayerOverlay)mapView.Overlays["FriscoParksOverlay"];
        var friscoParksLayer = (ShapeFileFeatureLayer)friscoParkOverlay.Layers["FriscoParksLayer"];

        var selectedAreaOverlay = (LayerOverlay)mapView.Overlays["SelectedAreaOverlay"];
        var selectedAreaLayer = (InMemoryFeatureLayer)selectedAreaOverlay.Layers["SelectedAreaLayer"];

        // Query the friscoParks layer to get the first feature closest to the map tap event
        var pointInWorldCoordinate = mapView.ToWorldCoordinate(e.X, e.Y);
        var feature = friscoParksLayer.QueryTools.GetFeaturesNearestTo(pointInWorldCoordinate, GeographyUnit.Meter, 1,
            ReturningColumnsType.NoColumns).First();

        // Show the selected feature on the map
        selectedAreaLayer.InternalFeatures.Clear();
        selectedAreaLayer.InternalFeatures.Add(feature);
        await selectedAreaOverlay.RefreshAsync();

        // Get the area of the first feature
        var area = ((AreaBaseShape)feature.GetShape()).GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers);

        // Display the selectedArea's area in the areaResult TextBox
        AreaResult.Text = $"Area: {area:f3} sq km";
    }
}
