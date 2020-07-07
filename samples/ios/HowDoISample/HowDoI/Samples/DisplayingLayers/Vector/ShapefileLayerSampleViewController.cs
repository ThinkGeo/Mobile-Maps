using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public partial class ShapefileLayerSampleViewController : BaseController
    {
        public ShapefileLayerSampleViewController() : base("ShapefileLayerSampleViewController", null)
        {
            Title = "";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            MapView mapView = new MapView(View.Frame);
            mapView.MapTools.ZoomMapTool.IsEnabled = false;
            View.Add(mapView);

            // Set the Map Unit to Meter, the Shapefile’s unit of measure.
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~");
            // Add a ThinkGeoCloudMapsOverlay.
            mapView.Overlays.Add("ThinkGeoCloudMapsOverlay", thinkGeoCloudMapsOverlay);

            // Create a new Layer and pass the path to a Shapefile into its constructor. 
            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/cntry02.shp");

            // Set the worldLayer with a preset Style, as AreaStyles.Country1 has YellowGreen background and black border, our worldLayer will have the same render style.  
            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
            areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 118, 138, 69), 1);
            areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;

            // This setting will apply from ZoomLevel01 to ZoomLevel20, which means the map will be rendered in the same style, no matter how far we zoom in or out. 
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureLayer capitalLayer = new ShapeFileFeatureLayer("AppData/capital.shp");
            // Similarly, we use the presetPointStyle for cities.     
            PointStyle pointStyle = new PointStyle();
            pointStyle.SymbolType = PointSymbolType.Square;
            pointStyle.FillBrush = new GeoSolidBrush(GeoColors.White);
            pointStyle.OutlinePen = new GeoPen(GeoColors.Black, 1);
            pointStyle.SymbolSize = 6;

            PointStyle stackStyle = new PointStyle();
            stackStyle.SymbolType = PointSymbolType.Square;
            stackStyle.FillBrush = new GeoSolidBrush(GeoColors.Maroon);
            stackStyle.OutlinePen = new GeoPen(GeoColors.Transparent, 0);
            stackStyle.SymbolSize = 2;

            pointStyle.CustomPointStyles.Add(stackStyle);
            // We can customize our own Style. Here we pass in a color and a size.
            capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.White, 7, GeoColors.Brown);
            // The Style we set here is available from ZoomLevel01 to ZoomLevel05. That means if we zoom in a bit more, it will no longer be visible.
            capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;
            capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultPointStyle = pointStyle;
            // This setting also applies from ZoomLevel01 to ZoomLevel20, so city symbols will be rendered in the same style, no matter how far we zoom in or out. 
            capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // We create a new Layer for labeling the capitals.
            ShapeFileFeatureLayer capitalLabelLayer = new ShapeFileFeatureLayer("AppData/capital.shp");
            // We use the preset TextStyle. Here we pass in "CITY_NAME", the name of the field containing the values we want to label the map with.
            GeoFont font = new GeoFont("Arial", 9, DrawingFontStyles.Bold);
            GeoSolidBrush txtBrush = new GeoSolidBrush(GeoColors.Maroon);
            TextStyle textStyle = new TextStyle("CITY_NAME", font, txtBrush);
            textStyle.XOffsetInPixel = 0;
            textStyle.YOffsetInPixel = -6;
            // We can customize our own TextStyle. Here we pass in the font, size, style and color.
            capitalLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("CITY_NAME", "Arial", 8, DrawingFontStyles.Italic, GeoColors.Black, 3, 3);
            // The TextStyle we set here is available from ZoomLevel01 to ZoomLevel05. 
            capitalLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;
            capitalLabelLayer.ZoomLevelSet.ZoomLevel06.DefaultTextStyle = textStyle;
            capitalLabelLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            // Since the map is drawn with tiles, the label needs to draw on the margin to make sure the text is complete after joining the tiles together.
            // Create a new Layer Overlay to hold the layer we just created
            LayerOverlay layerOverlay = new LayerOverlay();

            // Add the shapefile layer to the layer overlay
            layerOverlay.Layers.Add(worldLayer);
            layerOverlay.Layers.Add(capitalLayer);
            layerOverlay.Layers.Add(capitalLabelLayer);

            // Add the layerOverlay to map.
            mapView.Overlays.Add(layerOverlay);

            // Set a proper extent for the map. The extent is the geographical area you want it to display.
            mapView.CurrentExtent = new RectangleShape(-11917925, 6094804, -3300683, 370987);

            // We now need to call the Refresh() method of the Map view so that the Map can redraw based on the data that has been provided.
            mapView.Refresh();

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

