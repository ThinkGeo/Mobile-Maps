using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataGeometricOperation;

public partial class GetLineOnALine
{
    private bool _initialized;
    public GetLineOnALine()
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

        // Add the rail line feature to the railway layer
        var railway = new InMemoryFeatureLayer();
        railway.InternalFeatures.Add(new Feature(
            "LineString (-10776730 3925750, -10778989 3915278, -10781766 3909228, -10782065 3907458, -10781867 3905465)"));

        // Style railway layer
        railway.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
        railway.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add railway to the layerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add("railway", railway);
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        // Style the subLineLayer
        var subLineLayer = new InMemoryFeatureLayer();
        subLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Green, 4, false);
        subLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add subLineLayer to the layerOverlay
        var subLineOverlay = new LayerOverlay();
        subLineOverlay.Layers.Add("subLineLayer", subLineLayer);
        MapView.Overlays.Add("subLineOverlay", subLineOverlay);

        // Set the map extent
        MapView.CenterPoint = new PointShape(-10777600, 3915260);
        MapView.MapScale = 140000;

        // Add LayerOverlay to Map
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Get a subLine of a line and displays it on the map
    /// </summary>
    private async void GetSubLine_OnClick(object sender, EventArgs e)
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var subLineOverlay = (LayerOverlay)MapView.Overlays["subLineOverlay"];

        var railway = (InMemoryFeatureLayer)layerOverlay.Layers["railway"];
        var subLineLayer = (InMemoryFeatureLayer)subLineOverlay.Layers["subLineLayer"];

        // Query the railway layer to get all the features
        railway.Open();
        var feature = railway.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
        railway.Close();

        // Get the subLine, starting from 1000-meter point,and the length is 10000 meters 
        var subLine = ((LineShape)feature.GetShape()).GetLineOnALine(StartingPoint.FirstPoint,
            1000, 10000, GeographyUnit.Meter,
            DistanceUnit.Meter);

        // Add the subLine into an InMemoryFeatureLayer to display the result.
        subLineLayer.InternalFeatures.Clear();
        subLineLayer.InternalFeatures.Add(new Feature(subLine));

        // Redraw the layerOverlay to see the subLine on the map
        await subLineOverlay.RefreshAsync();
    }
}