﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Style = ThinkGeo.Core.Style;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display an ISOLine Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ISOLineLayerSample : ContentPage
    {
        public ISOLineLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the ISOLine layer to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create background world map with vector tile requested from ThinkGeo Cloud Service.
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            var isoLineOverlay = new LayerOverlay();
            mapView.Overlays.Add("isoLineOverlay", isoLineOverlay);

            // Load a csv file with the mosquito data that we will use for the iso line.
            var csvPointData = GetDataFromCSV(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Csv/Frisco_Mosquitos.csv"));

            // Create the layer based on the method GetDynamicIsoLineLayer and pass in the points we loaded above and add it to the map.
            //  We then set the drawing quality high so we get a crisp rendering.
            var isoLineLayer = GetDynamicIsoLineLayer(csvPointData);
            isoLineOverlay.Layers.Add("IsoLineLayer", isoLineLayer);
            isoLineOverlay.DrawingQuality = DrawingQuality.HighQuality;

            // Create a layer that so we can get the current extent below to set the maps extend
            // We wont use it after so later in the code we will just close it.
            var mosquitosLayer = new ShapeFileFeatureSource(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco_Mosquitos.shp"));
            mosquitosLayer.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Open the layer and set the map view current extent to the bounding box of the layer scaled up just a bit then close the layer
            mosquitosLayer.Open();
            mapView.CurrentExtent = mosquitosLayer.GetBoundingBox();
            mosquitosLayer.Close();

            // Refresh the map.
            mapView.Refresh();
        }

        private static Dictionary<PointShape, double> GetDataFromCSV(string csvFilePath)
        {
            // This code just reads the csv file into a dictionary of point shapes for the locations and mosquito population at those points.
            StreamReader streamReader = null;
            var csvDataPoints = new Dictionary<PointShape, double>();

            try
            {
                streamReader = new StreamReader(csvFilePath);
                var headline = streamReader.ReadLine();
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    var parts = line.Split(',');
                    csvDataPoints.Add(new PointShape(double.Parse(parts[0]), double.Parse(parts[1])),
                        double.Parse(parts[2]));
                }
            }
            finally
            {
                if (streamReader != null) streamReader.Close();
            }

            return csvDataPoints;
        }

        private DynamicIsoLineLayer GetDynamicIsoLineLayer(Dictionary<PointShape, double> csvPointData)
        {
            // We use this method to generate the values for the lines based on the data values and how many breaks we want.
            var isoLineLevels = IsoLineLayer.GetIsoLineLevels(csvPointData, 25);

            //Create the new dynamicIsoLineLayer
            var dynamicIsoLineLayer = new DynamicIsoLineLayer(csvPointData, isoLineLevels,
                new InverseDistanceWeightedGridInterpolationModel(), IsoLineType.LinesOnly);

            // Set the cell height and width dynamically based on the map view size
            //dynamicIsoLineLayer.CellHeightInPixel = (int)(mapView.ActualHeight / 80);
            //dynamicIsoLineLayer.CellWidthInPixel = (int)(mapView.ActualWidth / 80);

            //Create a series of colors from blue to red that we will use for the breaks based on the number of iso line levels we want.
            var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Blue, GeoColors.Red, isoLineLevels.Count,
                ColorWheelDirection.Clockwise);

            //Setup a class break style based on the isoline levels and the colors and add it to the iso line layer
            var classBreakStyle = new ClassBreakStyle(dynamicIsoLineLayer.DataValueColumnName);
            dynamicIsoLineLayer.CustomStyles.Add(classBreakStyle);

            // Create a collection of styles that we use we will use for the minimum value
            var firstStyles = new Collection<Style>();
            firstStyles.Add(new LineStyle(new GeoPen(colors[0], 3)));
            firstStyles.Add(new AreaStyle(new GeoPen(GeoColors.LightBlue, 3),
                new GeoSolidBrush(new GeoColor(150, colors[0]))));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, firstStyles));

            // Loop through all the colors we created as they will be class breaks
            for (var i = 0; i < colors.Count - 1; i++)
            {
                // Create the style collection for this break based on the colors we generated
                var styles = new Collection<Style>();
                styles.Add(new LineStyle(new GeoPen(colors[i + 1], 3)));
                styles.Add(new AreaStyle(new GeoPen(GeoColors.LightBlue, 3),
                    new GeoSolidBrush(new GeoColor(150, colors[i + 1]))));

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
            dynamicIsoLineLayer.CustomStyles.Add(textStyle);

            return dynamicIsoLineLayer;
        }

        #region Create Sample Data Code

        // ========================================================
        // Code for creating the grid file from a point shapefile
        // ========================================================
        //var mos = new ShapeFileFeatureSource(@"../../../data/Frisco_Mosquitos.shp");
        //mos.ProjectionConverter = new ProjectionConverter(2276, 3857);
        //mos.Open();
        //var features = mos.GetAllFeatures(ReturningColumnsType.AllColumns);

        //Dictionary<PointShape, double> points = new Dictionary<PointShape, double>();

        //StringBuilder builder = new StringBuilder();

        //foreach (var feature in features)
        //{
        //    if (feature.ColumnValues["DateCollec"] == "20190806")
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
        //File.WriteAllText(@"../../../data/Frisco_Mosquitos.csv", builder.ToString());

        #endregion
    }
}