using Android.Content;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class IconStyleSample : BaseSample
    {
        public IconStyleSample(Context context)
            : base(context)
        { }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-10786013, 3942186, -10758506, 3891508);

            ShapeFileFeatureLayer iconStyleFeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("Vehicles.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(iconStyleFeatureLayer);
            MapView.Overlays.Add(layerOverlay);

            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetIconStyle(SampleHelper.GetDataPath(@"Images/")));
            iconStyleFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        private static ValueStyle GetIconStyle(string imagePath)
        {
            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "TYPE";

            GeoFont font = new GeoFont("Arial", 9);
            GeoSolidBrush brush = new GeoSolidBrush(GeoColor.StandardColors.Black);

            valueStyle.ValueItems.Add(new ValueItem("1", new IconStyle(imagePath + "vehicle1.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("2", new IconStyle(imagePath + "vehicle2.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("3", new IconStyle(imagePath + "vehicle3.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("4", new IconStyle(imagePath + "vehicle4.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("5", new IconStyle(imagePath + "vehicle5.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("6", new IconStyle(imagePath + "vehicle6.png", "Type", font, brush)));
            valueStyle.ValueItems.Add(new ValueItem("7", new IconStyle(imagePath + "vehicle7.png", "Type", font, brush)));

            return valueStyle;
        }
    }
}