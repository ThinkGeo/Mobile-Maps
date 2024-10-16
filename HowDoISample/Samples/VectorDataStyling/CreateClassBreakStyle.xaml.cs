using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateClassBreakStyle
{
    private bool _initialized;
    public CreateClassBreakStyle()
    {
        InitializeComponent();
    }

    private async void CreateClassBreakStyle_OnSizeChanged(object sender, EventArgs e)
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

        var housingUnitsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco 2010 Census Housing Units.shp"));
        var legend = new LegendAdornmentLayer
        {
            // Set up the legend adornment
            Title = new LegendItem
            {
                TextStyle = new TextStyle("Housing Units", new GeoFont("Verdana", 10, DrawingFontStyles.Bold),
                    GeoBrushes.Black)
            },
            Location = AdornmentLocation.LowerRight
        };

        MapView.AdornmentOverlay.Layers.Add(legend);

        // Project the layer's data to match the projection of the map
        housingUnitsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        AddClassBreakStyle(housingUnitsLayer, legend);

        // Add housingUnitsLayer to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(housingUnitsLayer);

        // Add layerOverlay to the mapView
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 800000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }

    /// <summary>
    /// Adds a ClassBreakStyle to the housingUnitsLayer that changes colors based on the numerical value of the H_UNITS
    /// column as they fall within the range of a ClassBreak
    /// </summary>
    private static void AddClassBreakStyle(FeatureLayer layer, LegendAdornmentLayer legend)
    {
        // Create the ClassBreakStyle based on the H_UNITS numerical column
        var housingUnitsStyle = new ClassBreakStyle("H_UNITS");

        var classBreakIntervals = new double[] { 0, 1000, 2000, 3000, 4000, 5000 };
        var colors = GeoColor.GetColorsInHueFamily(GeoColors.Red, classBreakIntervals.Length).Reverse().ToList();

        // Create ClassBreaks for each of the classBreakIntervals
        for (var i = 0; i < classBreakIntervals.Length; i++)
        {
            // Create the classBreak using one of the intervals and colors defined above
            var classBreak = new ClassBreak(classBreakIntervals[i],
                AreaStyle.CreateSimpleAreaStyle(new GeoColor(192, colors[i]), GeoColors.White));

            // Add the classBreak to the housingUnitsStyle ClassBreaks collection
            housingUnitsStyle.ClassBreaks.Add(classBreak);

            // Add a LegendItem to the legend adornment to represent the classBreak
            var legendItem = new LegendItem
            {
                ImageStyle = classBreak.DefaultAreaStyle,
                TextStyle = new TextStyle($">{classBreak.Value} units", new GeoFont("Verdana", 10),
                    GeoBrushes.Black)
            };
            legend.LegendItems.Add(legendItem);
        }

        // Add and apply the ClassBreakStyle to the housingUnitsLayer
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(housingUnitsStyle);
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}