using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateTextStyle
{
    private bool _initialized;
    public CreateTextStyle()
    {
        InitializeComponent();
    }

    private ShapeFileFeatureLayer _hotelsLabelLayer;
    private ShapeFileFeatureLayer _streetsLabelLayer;
    private ShapeFileFeatureLayer _parksLabelLayer;
    private LayerNonRotationGraphicsViewOverlay _dynamicLabelOverlay;

    private async void CreateTextStyle_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;
        MapView.IsRotationEnabled = true;

        // Set the map scale and center point
        MapView.MapScale = 9000;
        MapView.CenterPoint = new PointShape(-10777289.95123455, 3908744.4225903796);

        MapView.Overlays.Clear();

        await LoadDynamicLabels();
        await MapView.RefreshAsync();

        SetupEvents();
    }

    private void SetupEvents()
    {
        DynamicLabelCheckBox.CheckedChanged += async (_, args) =>
        {
            LabelOptionsCheckBox.IsVisible = args.Value;
            MapView.Overlays.Clear();

            if (!args.Value)
            {
                LoadStaticLabels();
            }
            else
            {
                await LoadDynamicLabels();

                _hotelsLabelLayer.IsVisible = LabelHotelsCheckBox.IsChecked;
                _parksLabelLayer.IsVisible = LabelParksCheckBox.IsChecked;
                _streetsLabelLayer.IsVisible = LabelStreetsCheckBox.IsChecked;

            }
            await MapView.RefreshAsync();
        };

        LabelHotelsCheckBox.CheckedChanged += async (_, args) =>
        {
            _hotelsLabelLayer.IsVisible = args.Value;
            await _dynamicLabelOverlay.RefreshAsync();
        };
        LabelStreetsCheckBox.CheckedChanged += async (_, args) =>
        {
            _streetsLabelLayer.IsVisible = args.Value;
            await _dynamicLabelOverlay.RefreshAsync();
        };
        LabelParksCheckBox.CheckedChanged += async (_, args) =>
        {
            _parksLabelLayer.IsVisible = args.Value;
            await _dynamicLabelOverlay.RefreshAsync();
        };

        CompassButton.Clicked += async (_, _) =>
            await MapView.ZoomToExtentAsync(MapView.CenterPoint, MapView.MapScale, 0);
    }

    private void LoadStaticLabels()
    {
        // Add layers to a layerOverlay
        var layerOverlay = new LayerOverlay();
        LoadLayers(layerOverlay, false);
        // Add overlay to map
        MapView.Overlays.Add(layerOverlay);
    }

    private async Task LoadDynamicLabels()
    {
        // Add layers to a layerOverlay
        var layerOverlay = new LayerOverlay();
        LoadLayers(layerOverlay, true);
        // Add overlay to map
        MapView.Overlays.Add(layerOverlay);

        _dynamicLabelOverlay = new LayerNonRotationGraphicsViewOverlay();
        await _dynamicLabelOverlay.OpenAsync(MapView);
        LoadLayers(_dynamicLabelOverlay);
        // Add overlay to map
        MapView.Overlays.Add(_dynamicLabelOverlay);
    }

    private static void LoadLayers(LayerOverlay layerOverlay, bool shapeOnly)
    {
        var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));
        var streetsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Streets.shp"));
        var parksLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));

        // Project the layer's data to match the projection of the map
        hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        streetsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        if (shapeOnly)
        {
            StyleStreetsLayer_ShapeOnly(streetsLayer);
            StyleParksLayer_ShapeOnly(parksLayer);
        }
        else
        {
            StyleHotelsLayer(hotelsLayer);
            StyleStreetsLayer(streetsLayer);
            StyleParksLayer(parksLayer);
        }

        layerOverlay.Background = Colors.White;
        layerOverlay.Layers.Add(parksLayer);
        layerOverlay.Layers.Add(streetsLayer);
        layerOverlay.Layers.Add(hotelsLayer);
    }

    private void LoadLayers(LayerNonRotationGraphicsViewOverlay labelOverlay)
    {
        _hotelsLabelLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));
        _streetsLabelLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Streets.shp"));
        _parksLabelLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));

        // Project the layer's data to match the projection of the map
        _hotelsLabelLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        _streetsLabelLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        _parksLabelLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        StyleHotelsLayer(_hotelsLabelLayer);
        StyleStreetsLayer_LabelOnly(_streetsLabelLayer);
        StyleParksLayer_LabelOnly(_parksLabelLayer);

        labelOverlay.Layers.Add(_parksLabelLayer);
        labelOverlay.Layers.Add(_streetsLabelLayer);
        labelOverlay.Layers.Add(_hotelsLabelLayer);
    }

    /// <summary>
    /// Adds a PointStyle and TextStyle to the Hotels Layer
    /// </summary>
    private static void StyleHotelsLayer(FeatureLayer hotelsLayer)
    {
        var textStyle = new TextStyle("NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold),
            GeoBrushes.DarkRed)
        {
            TextPlacement = TextPlacement.Center,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            AllowLineCarriage = true,
            YOffsetInPixel = -10
        };
        var pointStyle = new PointStyle(PointSymbolType.Circle, 4, GeoBrushes.Brown,
            new GeoPen(GeoBrushes.DarkRed, 2));

        hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }

    /// <summary>
    /// Adds a LineStyle and TextStyle to the Streets Layer
    /// </summary>
    private static void StyleStreetsLayer(FeatureLayer streetsLayer)
    {
        StyleStreetsLayer_LabelOnly(streetsLayer);
        StyleStreetsLayer_ShapeOnly(streetsLayer);
    }
    private static void StyleStreetsLayer_LabelOnly(FeatureLayer streetsLayer)
    {
        var textStyle = new TextStyle("FULL_NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold),
            GeoBrushes.MidnightBlue)
        {
            SplineType = SplineType.StandardSplining,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel
        };
        streetsLayer.ZoomLevelSet.ZoomLevel16.CustomStyles.Add(textStyle);
        streetsLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }

    private static void StyleStreetsLayer_ShapeOnly(FeatureLayer streetsLayer)
    {
        var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));

        streetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(lineStyle);
        streetsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }

    /// <summary>
    /// Adds an AreaStyle and TextStyle to the Parks Layer
    /// </summary>
    private static void StyleParksLayer(FeatureLayer parksLayer)
    {
        StyleParksLayer_ShapeOnly(parksLayer);
        StyleParksLayer_LabelOnly(parksLayer);
    }

    private static void StyleParksLayer_ShapeOnly(FeatureLayer parksLayer)
    {
        var areaStyle = new AreaStyle(GeoPens.DimGray, GeoBrushes.PastelGreen);

        parksLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(areaStyle);
        parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
    private static void StyleParksLayer_LabelOnly(FeatureLayer parksLayer)
    {
        var textStyle = new TextStyle("NAME", new GeoFont("Segoe UI", 12, DrawingFontStyles.Bold),
            GeoBrushes.DarkGreen)
        {
            FittingPolygon = false,
            HaloPen = new GeoPen(GeoBrushes.White, 2),
            DrawingLevel = DrawingLevel.LabelLevel,
            AllowLineCarriage = true
        };

        parksLayer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(textStyle);
        parksLayer.ZoomLevelSet.ZoomLevel14.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}