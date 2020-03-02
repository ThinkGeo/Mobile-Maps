using Android.Content;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class CustomStyleSample : BaseSample
    {
        public CustomStyleSample(Context context)
            : base(context)
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = new RectangleShape(-13886070, 5660597, -8906057, 2382985);

            /*===========================================
               Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
               a Client ID and Secret. These were sent to you via email when you signed up
               with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
            ===========================================*/
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = (ThinkGeoCloudRasterMapsOverlay)MapView.Overlays["WMK"];
            string baseFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string cachePathFilename = Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
            thinkGeoCloudMapsOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "SphericalMercator");

            LineStyle lineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 50, 0, 249), 4, false);

            //Cold Front
            CustomGeoImageLineStyle coldFrontLineStyle = GetCustomLineStyle(lineStyle, 19, CustomGeoImageLineStyle.SymbolSide.Right,
                SampleHelper.GetDataPath(@"CustomStyles/offset_circle_red_bl.png"), SampleHelper.GetDataPath(@"CustomStyles/offset_triangle_blue_revert.png"));

            InMemoryFeatureLayer inMemoryFeatureLayerColdFront = new InMemoryFeatureLayer();
            inMemoryFeatureLayerColdFront.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(coldFrontLineStyle);
            inMemoryFeatureLayerColdFront.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryFeatureLayerColdFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/ColdFront2.txt"))));
            inMemoryFeatureLayerColdFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/ColdFront3.txt"))));

            //Warm Front
            InMemoryFeatureLayer inMemoryFeatureLayerWarmFront = new InMemoryFeatureLayer();
            CustomGeoImageLineStyle warmFrontLineStyle = new CustomGeoImageLineStyle(lineStyle, new GeoImage(SampleHelper.GetDataPath(@"CustomStyles/offset_circle_blue.png")),
                                                                                                    30, CustomGeoImageLineStyle.SymbolSide.Right);
            inMemoryFeatureLayerWarmFront.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(warmFrontLineStyle);
            inMemoryFeatureLayerWarmFront.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/WarmFront5.txt"))));
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/WarmFront6.txt"))));
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/WarmFront7.txt"))));
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/WarmFront8.txt"))));

            //Occluded Front
            InMemoryFeatureLayer inMemoryFeatureLayerOccludedFront = new InMemoryFeatureLayer();
            CustomGeoImageLineStyle occludedFrontLineStyle = GetCustomLineStyle(lineStyle, 45, CustomGeoImageLineStyle.SymbolSide.Right,
                SampleHelper.GetDataPath(@"CustomStyles/offset_triangle_and_circle_blue.png"), SampleHelper.GetDataPath(@"CustomStyles/offset_triangle_and_circle_red.png"));
            inMemoryFeatureLayerOccludedFront.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(occludedFrontLineStyle);
            inMemoryFeatureLayerOccludedFront.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/OccludedFront9.txt"))));
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/OccludedFront10.txt"))));
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(File.ReadAllText(SampleHelper.GetDataPath(@"CustomStyles/OccludedFront11.txt"))));

            PressureValueStyle pressureValueStyle = new PressureValueStyle();
            pressureValueStyle.ColumnName = "Pressure";

            InMemoryFeatureLayer pressureFeatureLayer = new InMemoryFeatureLayer();
            pressureFeatureLayer.Open();
            pressureFeatureLayer.Columns.Add(new FeatureSourceColumn("Pressure"));
            pressureFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pressureValueStyle);
            pressureFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            pressureFeatureLayer.InternalFeatures.Add(new Feature(MapView.CurrentExtent.GetCenterPoint()));

            Random random = new Random();
            string[] pressures = { "H", "L" };
            for (int i = 0; i < 20; i++)
            {
                Feature pressurePoint = new Feature(random.Next(-13886070, -8906057), random.Next(3382985, 6660597));
                pressurePoint.ColumnValues["Pressure"] = pressures[random.Next(0, 2)];
                pressureFeatureLayer.InternalFeatures.Add(pressurePoint);
            }

            WindPointStyle windStyle1 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#0AF8F8"));
            WindPointStyle windStyle2 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#0FF5B0"));
            WindPointStyle windStyle3 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#F7F70D"));
            WindPointStyle windStyle4 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#FBE306"));

            ClassBreakStyle windClassBreakStyle = new ClassBreakStyle();
            windClassBreakStyle.ColumnName = "TEXT";
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(10, new Collection<Style> { windStyle1 }));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(20, new Collection<Style> { windStyle2 }));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(30, new Collection<Style> { windStyle3 }));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(40, new Collection<Style> { windStyle4 }));

            InMemoryFeatureLayer windFeatureLayer = new InMemoryFeatureLayer();
            windFeatureLayer.Open();
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("TEXT"));
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("ANGLE"));
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("LEVEL"));
            windFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(windClassBreakStyle);
            windFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            for (int i = 0; i < 20; i++)
            {
                Feature windFeature = new Feature(random.Next(-13886070, -8906057), random.Next(3382985, 6660597));
                windFeature.ColumnValues["TEXT"] = random.Next(10, 40).ToString(CultureInfo.InvariantCulture);
                windFeature.ColumnValues["ANGLE"] = random.Next(0, 360).ToString(CultureInfo.InvariantCulture);
                windFeature.ColumnValues["LEVEL"] = random.Next(0, 5).ToString(CultureInfo.InvariantCulture);
                windFeatureLayer.InternalFeatures.Add(windFeature);
            }

            LayerOverlay dynamicOverlay = new LayerOverlay();
            dynamicOverlay.Layers.Add(inMemoryFeatureLayerColdFront);
            dynamicOverlay.Layers.Add(inMemoryFeatureLayerWarmFront);
            dynamicOverlay.Layers.Add(inMemoryFeatureLayerOccludedFront);
            dynamicOverlay.Layers.Add(pressureFeatureLayer);
            dynamicOverlay.Layers.Add(windFeatureLayer);
            MapView.Overlays.Add(dynamicOverlay);
        }

        private static CustomGeoImageLineStyle GetCustomLineStyle(LineStyle lineStyle, int space, CustomGeoImageLineStyle.SymbolSide symbolSide, params string[] imagePaths)
        {
            return new CustomGeoImageLineStyle(lineStyle, imagePaths.Select(p => new GeoImage(p)), space, symbolSide);
        }
    }
}