/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;

namespace AnalyzingVisualization
{
    public class CustomStyleViewContoller : DetailViewController
    {
        public CustomStyleViewContoller()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-13886070, 6660597, -8906057, 3382985);

            LineStyle lineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 50, 0, 249), 4, false);

            //Cold Front
            CustomGeoImageLineStyle coldFrontLineStyle = GetCustomLineStyle(lineStyle, 19, "offset_circle_red_bl.png", "offset_triangle_blue_revert.png");
            InMemoryFeatureLayer coldFrontLineLayer = new InMemoryFeatureLayer();
            coldFrontLineLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(coldFrontLineStyle);
            coldFrontLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            coldFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/ColdFront2.txt")));
            coldFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/ColdFront3.txt")));

            //Warm Front
            CustomGeoImageLineStyle warmFrontLineStyle = GetCustomLineStyle(lineStyle, 30, "offset_circle_blue.png");
            InMemoryFeatureLayer warmFrontLineLayer = new InMemoryFeatureLayer();
            warmFrontLineLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(warmFrontLineStyle);
            warmFrontLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            warmFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/WarmFront5.txt")));
            warmFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/WarmFront6.txt")));
            warmFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/WarmFront7.txt")));
            warmFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/WarmFront8.txt")));

            //Occluded Front
            CustomGeoImageLineStyle occludedFrontLineStyle = GetCustomLineStyle(lineStyle, 45, "offset_triangle_and_circle_blue.png", "offset_triangle_and_circle_red.png");
            InMemoryFeatureLayer occludedFrontLineLayer = new InMemoryFeatureLayer();
            occludedFrontLineLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(occludedFrontLineStyle);
            occludedFrontLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            occludedFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/OccludedFront9.txt")));
            occludedFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/OccludedFront10.txt")));
            occludedFrontLineLayer.InternalFeatures.Add(new Feature(File.ReadAllText(@"AppData/CustomStyles/OccludedFront11.txt")));

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

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            LayerOverlay dynamicOverlay = new LayerOverlay();
            dynamicOverlay.TileType = TileType.SingleTile;
            dynamicOverlay.Layers.Add(coldFrontLineLayer);
            dynamicOverlay.Layers.Add(warmFrontLineLayer);
            dynamicOverlay.Layers.Add(occludedFrontLineLayer);
            dynamicOverlay.Layers.Add(pressureFeatureLayer);
            dynamicOverlay.Layers.Add(windFeatureLayer);
            MapView.Overlays.Add(dynamicOverlay);

            MapView.Refresh();
        }

        private static CustomGeoImageLineStyle GetCustomLineStyle(LineStyle lineStyle, int space, params string[] imagePaths)
        {
            return new CustomGeoImageLineStyle(lineStyle, imagePaths.Select(p => new GeoImage(Path.Combine("AppData/CustomStyles", p))), space, CustomGeoImageLineStyle.SymbolSide.Right);
        }
    }
}