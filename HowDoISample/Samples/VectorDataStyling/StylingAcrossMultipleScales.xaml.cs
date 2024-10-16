using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class StylingAcrossMultipleScales
{
    private bool _initialized;
    public StylingAcrossMultipleScales()
    {
        InitializeComponent();
    }

    private async void StylingAcrossMultipleScales_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));
        var streetsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Streets.shp"));
        var parksLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));

        // Project the layer's data to match the projection of the map
        hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        streetsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Add Styles to the layers
        StyleHotelsLayer(hotelsLayer);
        StyleStreetsLayer(streetsLayer);
        StyleParksLayer(parksLayer);

        // Add layers to a layerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.BackgroundColor = Colors.White;
        layerOverlay.Layers.Add(parksLayer);
        layerOverlay.Layers.Add(streetsLayer);
        layerOverlay.Layers.Add(hotelsLayer);

        // Add overlay to map
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 9000;
        MapView.CenterPoint = new PointShape(-10777473, 3909015);
        await MapView.RefreshAsync();
    }

    /// <summary>
    /// Adds a PointStyle and TextStyle to the Hotels Layer
    /// </summary>
    private static void StyleHotelsLayer(FeatureLayer hotelsLayer)
    {
        //********************
        // * Zoom Level 12-13 *
        // ********************/
        hotelsLayer.ZoomLevelSet.ZoomLevel12.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 4,
            GeoBrushes.DarkRed, new GeoPen(GeoBrushes.White, 2));

        hotelsLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level13;

        //********************
        // * Zoom Level 14-15 *
        // ********************/
        hotelsLayer.ZoomLevelSet.ZoomLevel14.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 6,
            GeoBrushes.DarkRed, new GeoPen(GeoBrushes.White, 2));

        hotelsLayer.ZoomLevelSet.ZoomLevel14.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level15;

        //********************
        // * Zoom Level 16-20 *
        // ********************/
        hotelsLayer.ZoomLevelSet.ZoomLevel16.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 10,
            GeoBrushes.DarkRed, new GeoPen(GeoBrushes.White, 2));
        hotelsLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = new TextStyle("NAME",
            new GeoFont("Segoe UI", 10, DrawingFontStyles.Bold), GeoBrushes.DarkRed)
        {
            YOffsetInPixel = 1,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            AllowLineCarriage = true
        };

        hotelsLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }

    /// <summary>
    /// Adds a LineStyle and TextStyle to the Streets Layer
    /// </summary>
    private static void StyleStreetsLayer(FeatureLayer streetsLayer)
    {
        //********************
        // * Zoom Level 12-13 *
        // ********************/
        streetsLayer.ZoomLevelSet.ZoomLevel12.DefaultLineStyle = new LineStyle(new GeoPen(GeoBrushes.LightGray, 1));

        streetsLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level13;

        //*****************
        // * Zoom Level 14 *
        // *****************/
        streetsLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = new LineStyle(new GeoPen(GeoBrushes.LightGray, 2),
            new GeoPen(GeoBrushes.White, 1));

        //*****************
        // * Zoom Level 15 *
        // *****************/
        streetsLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = new LineStyle(
            new GeoPen(GeoBrushes.LightGray, 4) { EndCap = DrawingLineCap.Round },
            new GeoPen(GeoBrushes.White, 2) { EndCap = DrawingLineCap.Round });
        streetsLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle = new TextStyle("FULL_NAME",
            new GeoFont("Segoe UI", 9, DrawingFontStyles.Bold), GeoBrushes.Black)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            GridSize = 20
        };

        //*****************
        // * Zoom Level 16 *
        // *****************/
        streetsLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = new LineStyle(
            new GeoPen(GeoBrushes.Gray, 5) { EndCap = DrawingLineCap.Round },
            new GeoPen(GeoBrushes.White, 4) { EndCap = DrawingLineCap.Round });
        streetsLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = new TextStyle("FULL_NAME",
            new GeoFont("Segoe UI", 9, DrawingFontStyles.Bold), GeoBrushes.Black)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            GridSize = 20
        };

        //*****************
        // * Zoom Level 17 *
        // *****************/
        streetsLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = new LineStyle(
            new GeoPen(GeoBrushes.Gray, 7) { EndCap = DrawingLineCap.Round },
            new GeoPen(GeoBrushes.White, 6) { EndCap = DrawingLineCap.Round });
        streetsLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = new TextStyle("FULL_NAME",
            new GeoFont("Segoe UI", 9, DrawingFontStyles.Bold), GeoBrushes.Black)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            GridSize = 20
        };

        //*****************
        // * Zoom Level 18 *
        // *****************/
        streetsLayer.ZoomLevelSet.ZoomLevel18.DefaultLineStyle = new LineStyle(
            new GeoPen(GeoBrushes.Gray, 9) { EndCap = DrawingLineCap.Round },
            new GeoPen(GeoBrushes.White, 8) { EndCap = DrawingLineCap.Round });
        streetsLayer.ZoomLevelSet.ZoomLevel18.DefaultTextStyle = new TextStyle("FULL_NAME",
            new GeoFont("Segoe UI", 10, DrawingFontStyles.Bold), GeoBrushes.Black)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            GridSize = 20
        };

        //********************
        // * Zoom Level 19-20 *
        // ********************/
        streetsLayer.ZoomLevelSet.ZoomLevel19.DefaultLineStyle = new LineStyle(
            new GeoPen(GeoBrushes.Gray, 13) { EndCap = DrawingLineCap.Round },
            new GeoPen(GeoBrushes.White, 12) { EndCap = DrawingLineCap.Round });
        streetsLayer.ZoomLevelSet.ZoomLevel19.DefaultTextStyle = new TextStyle("FULL_NAME",
            new GeoFont("Segoe UI", 10, DrawingFontStyles.Bold), GeoBrushes.Black)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            GridSize = 20
        };

        streetsLayer.ZoomLevelSet.ZoomLevel19.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }

    /// <summary>
    /// Adds an AreaStyle and TextStyle to the Parks Layer
    /// </summary>
    private static void StyleParksLayer(FeatureLayer parksLayer)
    {
        //********************
        // * Zoom Level 12-14 *
        // ********************/
        parksLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = new AreaStyle(GeoBrushes.PastelGreen);

        parksLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level14;

        //********************
        // * Zoom Level 15-20 *
        // ********************/
        parksLayer.ZoomLevelSet.ZoomLevel15.DefaultAreaStyle = new AreaStyle(GeoBrushes.PastelGreen);
        parksLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle = new TextStyle("NAME",
            new GeoFont("Segoe UI", 10, DrawingFontStyles.Bold), GeoBrushes.DarkGreen)
        {
            FittingPolygon = false,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            AllowLineCarriage = true,
            FittingPolygonFactor = 1
        };

        parksLayer.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}