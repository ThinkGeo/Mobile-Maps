/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Globalization;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;

namespace AnalyzingVisualization
{
    public class IconStyleViewController : DetailViewController
    {
        public IconStyleViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            View.AddSubview(MapView);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            thinkGeoCloudMapsOverlay.TileCache = new FileRasterTileCache("./cache", "raster_light");
            thinkGeoCloudMapsOverlay.VectorTileCache = new FileVectorTileCache("./cache", "vector");
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            ShapeFileFeatureLayer iconStyleFeatureLayer = new ShapeFileFeatureLayer("AppData/Vehicles.shp");
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add(iconStyleFeatureLayer);
            MapView.Overlays.Add(layerOverlay);

            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetIconStyle("AppData/Images/vehicle"));
            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            MapView.ZoomTo(new PointShape(-10772265, 3916827), MapView.ZoomLevelSet.ZoomLevel14.Scale);
        }

        private static ValueStyle GetIconStyle(string imagePath)
        {
            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "TYPE";

            GeoFont font = new GeoFont("Arial", 12, DrawingFontStyles.Bold);
            GeoSolidBrush brush = new GeoSolidBrush(GeoColors.Black);

            for (int i = 1; i <= 7; i++)
            {
                IconStyle iconStyle = new IconStyle(imagePath + i + ".png", "Type", font, brush);
                iconStyle.HaloPen = new GeoPen(GeoColors.White);
                ValueItem valueItem = new ValueItem(i.ToString(CultureInfo.InvariantCulture), iconStyle);

                valueStyle.ValueItems.Add(valueItem);
            }

            return valueStyle;
        }
    }
}