using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateLineStyle
{
    private bool _initialized;
    public CreateLineStyle()
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
        var thinkGeoCloudVectorMapsOverlay = new ThinkGeoRasterOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
        };
        MapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

        // Create a layer with line data
        var friscoRailroad = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Railroad", "Railroad.shp"));
        var layerOverlay = new LayerOverlay();

        // Project the layer's data to match the projection of the map
        friscoRailroad.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Create a line style
        var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));
        // Add the line style to the collection of custom styles for ZoomLevel 1.
        friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
        friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layer to a layer overlay
        layerOverlay.Layers.Add("Railroad", friscoRailroad);

        // Add the overlay to the map
        MapView.Overlays.Add("overlay", layerOverlay);

        // Set the map scale and center point
        //mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel18.Scale;
        MapView.MapScale = 2500;
        MapView.CenterPoint = new PointShape(-10779392, 3914375);
        await MapView.RefreshAsync();
    }

    private async void rbLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MapView == null || MapView.Overlays.Count <= 0) return;

        if (sender is RadioButton { IsChecked: false })
            return;

        var layerOverlay = (LayerOverlay)MapView.Overlays["overlay"];
        var friscoRailroad = (ShapeFileFeatureLayer)layerOverlay.Layers["Railroad"];

        // Create a line style
        var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));

        // Add the line style to the collection of custom styles for ZoomLevel 1.
        friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
        friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Refresh the layerOverlay to show the new style
        await layerOverlay.RefreshAsync();
    }

    private async void rbDashedLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MapView.Overlays.Count <= 0) return;

        if (sender is RadioButton { IsChecked: false })
            return;

        var layerOverlay = (LayerOverlay)MapView.Overlays["overlay"];
        var friscoRailroad = (ShapeFileFeatureLayer)layerOverlay.Layers["Railroad"];

        var lineStyle = new LineStyle(
            new GeoPen(GeoColors.Black, 6),
            new GeoPen(GeoColors.White, 4)
            {
                DashStyle = LineDashStyle.Custom,
                DashPattern = { 3f, 3f },
                StartCap = DrawingLineCap.Flat,
                EndCap = DrawingLineCap.Flat
            }
        );

        // Add the line style to the collection of custom styles for ZoomLevel 1.
        friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
        // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
        friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Refresh the layerOverlay to show the new style
        await layerOverlay.RefreshAsync();
    }
}