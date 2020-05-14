/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Globalization;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class IconStyleViewController : DetailViewController
    {
        public IconStyleViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);
            View.AddSubview(MapView);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer iconStyleFeatureLayer = new ShapeFileFeatureLayer("AppData/Vehicles.shp");
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add(iconStyleFeatureLayer);
            MapView.Overlays.Add(layerOverlay);

            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetIconStyle("AppData/Images/vehicle"));
            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            MapView.ZoomTo(new PointShape(-10772265, 3916827), MapView.ZoomLevelSet.ZoomLevel14.Scale);
        }

        private static ValueStyle GetIconStyle(string imagePath)
        {
            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "TYPE";

            GeoFont font = new GeoFont("Arial", 12, DrawingFontStyles.Bold);
            GeoSolidBrush brush = new GeoSolidBrush(GeoColor.StandardColors.Black);

            for (int i = 1; i <= 7; i++)
            {
                IconStyle iconStyle = new IconStyle(imagePath + i + ".png", "Type", font, brush);
                iconStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White);
                ValueItem valueItem = new ValueItem(i.ToString(CultureInfo.InvariantCulture), iconStyle);

                valueStyle.ValueItems.Add(valueItem);
            }

            return valueStyle;
        }
    }
}