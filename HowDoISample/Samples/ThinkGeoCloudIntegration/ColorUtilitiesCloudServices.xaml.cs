using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.ThinkGeoCloudIntegration;

public partial class ColorUtilitiesCloudServices
{
    private bool _initialized;
    private ColorCloudClient _colorCloudClient;
    public ColorUtilitiesCloudServices()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Set the map's unit of measurement to meters (Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a new ShapeFileFeatureLayer using a shapefile containing Frisco Census data
        var housingUnitsLayer = new ShapeFileFeatureLayer(Path.Combine(
            FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco 2010 Census Housing Units.shp"));
        housingUnitsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create a new ProjectionConverter to convert between Texas North Central (2276) and Spherical Mercator (3857)
        var projectionConverter = new ProjectionConverter(2276, 3857);
        housingUnitsLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Create a new overlay and add the census feature layer
        var housingUnitsOverlay = new LayerOverlay();
        housingUnitsOverlay.Layers.Add("Frisco Housing Units", housingUnitsLayer);
        MapView.Overlays.Add("Frisco Housing Units Overlay", housingUnitsOverlay);

        // Create a legend adornment to display class breaks
        var legend = new LegendAdornmentLayer
        {
            // Set up the legend adornment
            Title = new LegendItem
            {
                TextStyle = new TextStyle("Housing Unit Counts", new GeoFont("Verdana", 10, DrawingFontStyles.Bold),
                GeoBrushes.Black)
            },
            Location = AdornmentLocation.LowerRight
        };
        MapView.AdornmentOverlay.Layers.Add("Legend", legend);

        // Get the extent of the features from the housing units shapefile, and set the map extent.
        MapView.CenterPoint = new PointShape(-10774523, 3909181);
        MapView.MapScale = 800_000;
        // Initialize the ColorCloudClient using our ThinkGeo Cloud credentials
        _colorCloudClient = new ColorCloudClient(SampleKeys.ClientId2, SampleKeys.ClientSecret2);

        // Set the initial color scheme for the housing units layer
        var colors = await GetColorsFromCloud();
        // If colors were successfully generated, update the map
        if (colors.Count > 0) await UpdateHousingUnitsLayerColors(colors);

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Make a request to the ThinkGeo Cloud for a new set of colors
    /// </summary>
    private async Task<Collection<GeoColor>> GetColorsFromCloud()
    {
        // Set the number of colors we want to generate
        const int numberOfColors = 3;

        // Create a new collection to hold the colors generated
        var colors = new Collection<GeoColor>();

        // Generate colors based on the selected 'color type'
        if (RdoHue.IsChecked)
        {
            // Get a family of colors with the same hue and sequential variances in lightness and saturation
            colors = await GetColorsByHue(numberOfColors);
        }
        else if (RdoQuality.IsChecked)
        {
            // Get a family of colors based on analogous hues
            colors = await GetQualityColors(numberOfColors);
        }
        else if (RdoContrasting.IsChecked)
        {
            // Get a family of colors based on complementary hues
            colors = await GetContrastingColors(numberOfColors);
        }
        // Add more conditions for other radio buttons as needed

        return colors;
    }

    /// <summary>
    ///     Update the colors for the housing units layers
    /// </summary>
    private async Task UpdateHousingUnitsLayerColors(IReadOnlyList<GeoColor> colors)
    {
        // Get the housing units layer from the MapView
        var housingUnitsOverlay = (LayerOverlay)MapView.Overlays["Frisco Housing Units Overlay"];
        var housingUnitsLayer = (ShapeFileFeatureLayer)housingUnitsOverlay.Layers["Frisco Housing Units"];

        // Clear the previous style from the housing units layer
        housingUnitsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();

        // Create a new ClassBreakStyle to showcase the color family generated
        var classBreakStyle = new ClassBreakStyle();
        var classBreaks = new Collection<ClassBreak>();

        // Different features will be styled differently based on the 'H_UNITS' attribute of the features
        classBreakStyle.ColumnName = "H_UNITS";
        double[] classBreaksIntervals = [0, 1000, 2000, 3000, 4000, 5000];
        for (var i = 0; i < colors.Count; i++)
        {
            // Create a differently colored area style for housing units counts of 0, 1000, 2000, etc
            var areaStyle = new AreaStyle(new GeoSolidBrush(colors[colors.Count - i - 1]));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(classBreaksIntervals[i], areaStyle));
            classBreaks.Add(new ClassBreak(classBreaksIntervals[i], areaStyle));
        }

        // Add the ClassBreakStyle to the housing units layer
        housingUnitsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
        housingUnitsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        await GenerateNewLegendItems(classBreaks);

        // Refresh the overlay to redraw the features
        await housingUnitsOverlay.RefreshAsync();
    }

    private async Task GenerateNewLegendItems(Collection<ClassBreak> classBreaks)
    {
        //// Clear the previous legend adornment
        var legend = (LegendAdornmentLayer)MapView.AdornmentOverlay.Layers["Legend"];

        legend.LegendItems.Clear();
        // Add a LegendItems to the legend adornment for each ClassBreak
        foreach (var classBreak in classBreaks)
        {
            var legendItem = new LegendItem
            {
                ImageStyle = classBreak.DefaultAreaStyle,
                TextStyle = new TextStyle($">{classBreak.Value} units", new GeoFont("Verdana", 10),
                    GeoBrushes.Black)
            };
            legend.LegendItems.Add(legendItem);
        }

        await MapView.AdornmentOverlay.RefreshAsync();
    }

    /// <summary>
    ///     Use the ColorCloudClient APIs to generate a set of colors based on the input parameters, and apply the new color
    ///     scheme to a feature layer
    /// </summary>
    private async void GenerateColors_Click(object sender, EventArgs e)
    {
        // Get a new set of colors from the ThinkGeo Cloud
        var colors = await GetColorsFromCloud();

        // If colors were successfully generated, update the map
        if (colors.Count > 0) await UpdateHousingUnitsLayerColors(colors);
    }

    /// <summary>
    ///     Get a family of colors with the same hue and sequential variances in lightness and saturation
    /// </summary>
    private async Task<Collection<GeoColor>> GetColorsByHue(int numberOfColors)
    {
        // Generate colors based on the parameters selected in the UI
        if (RdoRandomColor.IsChecked)
            // Use a random base color
            return await _colorCloudClient.GetColorsInHueFamilyAsync(numberOfColors);
        else
            // Use a default color for the base color
            return await _colorCloudClient.GetColorsInHueFamilyAsync(GetGeoColorFromDefaultColors(),
                numberOfColors);
    }

    /// <summary>
    ///     Get a family of colors with qualitative variances in hue, but similar lightness and saturation
    /// </summary>
    private async Task<Collection<GeoColor>> GetQualityColors(int numberOfColors)
    {
        if (RdoRandomColor.IsChecked)
            // Use a random base color
            return await _colorCloudClient.GetColorsInQualityFamilyAsync(numberOfColors);
        else
            // Use a default color for the base color
            return await _colorCloudClient.GetColorsInQualityFamilyAsync(GetGeoColorFromDefaultColors(),
                numberOfColors);
    }

    /// <summary>
    ///     Get a family of colors based on contrasting hues
    /// </summary>
    private async Task<Collection<GeoColor>> GetContrastingColors(int numberOfColors)
    {
        var contrastingColors = new Collection<GeoColor>();
        Dictionary<GeoColor, Collection<GeoColor>> colorsDictionary;

        // Generate colors based on the parameters selected in the UI
        if (RdoRandomColor.IsChecked)
            // Use a random base color
            colorsDictionary = await _colorCloudClient.GetColorsInContrastingFamilyAsync(numberOfColors);
        else
            // Use a default color for the base color
            colorsDictionary = await _colorCloudClient.GetColorsInContrastingFamilyAsync(GetGeoColorFromDefaultColors(),
                    numberOfColors);

        // Some color generation APIs use multiple base colors based on the original input color
        // These APIs return a dictionary where the 'keys' are the base colors and the 'values' are the colors generated from that base
        // For this sample we will simply utilize all the colors generated
        foreach (var color in colorsDictionary.Values.SelectMany(colors => colors))
            contrastingColors.Add(color);

        return contrastingColors;
    }

    /// <summary>
    ///     Helper function to get a GeoColor based on the selected Default Color in the UI
    /// </summary>
    private GeoColor GetGeoColorFromDefaultColors()
    {
        var color = GeoColors.White;

        if (RdoRedColor.IsChecked)
        {
            color = GeoColors.Red;
        }
        else if (RdoGreenColor.IsChecked)
        {
            color = GeoColors.Green;
        }
        else if (RdoBlueColor.IsChecked)
        {
            color = GeoColors.Blue;
        }

        return color;
    }

}