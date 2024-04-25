using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class IsoLineLayer
{
    private bool _initialized;
    public IsoLineLayer()
    {
        InitializeComponent();
    }

    private async void ISOLineLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create background world map with vector tile requested from ThinkGeo Cloud Service.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var isoLineOverlay = new LayerOverlay
        {
            BackgroundColor = Colors.Transparent
        };
        MapView.Overlays.Add("isoLineOverlay", isoLineOverlay);

        // Load a csv file with the mosquito data that we will use for the iso line.
        var csvPointData = GetDataFromCsv(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Csv", "Frisco_Mosquitoes.csv"));

        // Create the layer based on the method GetDynamicIsoLineLayer and pass in the points we loaded above and add it to the map.
        //  We then set the drawing quality high, so we get a crisp rendering.
        var isoLineLayer = GetDynamicIsoLineLayer(csvPointData);
        isoLineOverlay.Layers.Add("IsoLineLayer", isoLineLayer);
        
        // Set the map scale and center point
        MapView.MapScale = 140_000;
        MapView.CenterPoint = new PointShape(-10778478, 3914602);
        await MapView.RefreshAsync();
    }

    private static Dictionary<PointShape, double> GetDataFromCsv(string csvFilePath)
    {
        // This code just reads the csv file into a dictionary of point shapes for the locations and mosquito population at those points.
        StreamReader streamReader = null;
        var csvDataPoints = new Dictionary<PointShape, double>();

        try
        {
            streamReader = new StreamReader(csvFilePath);
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();

                if (line == null) continue;
                var parts = line.Split(',');
                csvDataPoints.Add(new PointShape(double.Parse(parts[0]), double.Parse(parts[1])),
                    double.Parse(parts[2]));
            }
        }
        finally
        {
            streamReader?.Close();
        }

        return csvDataPoints;
    }

    private DynamicIsoLineLayer GetDynamicIsoLineLayer(Dictionary<PointShape, double> csvPointData)
    {
        // We use this method to generate the values for the lines based on the data values and how many breaks we want.
        var isoLineLevels = ThinkGeo.Core.IsoLineLayer.GetIsoLineLevels(csvPointData, 25);

        //Create the new dynamicIsoLineLayer
        var dynamicIsoLineLayer = new DynamicIsoLineLayer(csvPointData, isoLineLevels,
            new InverseDistanceWeightedGridInterpolationModel(), IsoLineType.LinesOnly)
        {
            // Set the cell height and width dynamically based on the map view size
            CellHeightInPixel = (int)(MapView.Height / 80),
            CellWidthInPixel = (int)(MapView.Width / 80)
        };

        //Create a series of colors from blue to red that we will use for the breaks based on the number of iso line levels we want.
        var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Blue, GeoColors.Red, isoLineLevels.Count,
            ColorWheelDirection.Clockwise);

        //Set up a class break style based on the isoline levels and the colors and add it to the iso line layer
        var classBreakStyle = new ClassBreakStyle(dynamicIsoLineLayer.DataValueColumnName);
        dynamicIsoLineLayer.CustomStyles.Add(classBreakStyle);

        // Create a collection of styles that we use we will use for the minimum value
        var firstStyles = new Collection<ThinkGeo.Core.Style>
        {
            new LineStyle(new GeoPen(colors[0], 3)),
            new AreaStyle(new GeoPen(GeoColors.LightBlue, 3),
                new GeoSolidBrush(new GeoColor(150, colors[0])))
        };
        classBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, firstStyles));

        // Loop through all the colors we created as they will be class breaks
        for (var i = 0; i < colors.Count - 1; i++)
        {
            // Create the style collection for this break based on the colors we generated
            var styles = new Collection<ThinkGeo.Core.Style>
            {
                new LineStyle(new GeoPen(colors[i + 1], 3)),
                new AreaStyle(new GeoPen(GeoColors.LightBlue, 3),
                    new GeoSolidBrush(new GeoColor(150, colors[i + 1])))
            };

            // Add the class break with the styles
            classBreakStyle.ClassBreaks.Add(new ClassBreak(isoLineLevels[i], styles));
        }

        //Create the text styles to label the lines and add it to the iso line layer
        var textStyle = TextStyle.CreateSimpleTextStyle(dynamicIsoLineLayer.DataValueColumnName, "Arial", 10,
            DrawingFontStyles.Bold, GeoColors.Black, 0, 0);
        textStyle.HaloPen = new GeoPen(GeoColors.White, 2);
        textStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
        textStyle.SplineType = SplineType.StandardSplining;
        textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
        textStyle.TextLineSegmentRatio = 9999999;
        textStyle.FittingLineInScreen = true;
        textStyle.SuppressPartialLabels = true;

        return dynamicIsoLineLayer;
    }

    #region Create Sample Data Code

    // ========================================================
    // Code for creating the grid file from a point shapefile
    // ========================================================
    //var mos = new ShapeFileFeatureSource(@"../../../data/Frisco_Mosquitoes.shp");
    //mos.ProjectionConverter = new ProjectionConverter(2276, 3857);
    //mos.Open();
    //var features = mos.GetAllFeatures(ReturningColumnsType.AllColumns);

    //Dictionary<PointShape, double> points = new Dictionary<PointShape, double>();

    //StringBuilder builder = new StringBuilder();

    //foreach (var feature in features)
    //{
    //    if (feature.ColumnValues["DateCollection"] == "20190806")
    //    {
    //        int male = 0;
    //        int female = 0;

    //        if (feature.ColumnValues["Male"] != "")
    //            male = int.Parse(feature.ColumnValues["Male"]);

    //        if (feature.ColumnValues["Female"] != "")
    //            female = int.Parse(feature.ColumnValues["Female"]);

    //        builder.AppendLine($"{feature.GetBoundingBox().GetCenterPoint().X},{feature.GetBoundingBox().GetCenterPoint().Y},{male + female}");
    //    }
    //}
    //File.WriteAllText(@"../../../data/Frisco_Mosquitoes.csv", builder.ToString());

    #endregion
}