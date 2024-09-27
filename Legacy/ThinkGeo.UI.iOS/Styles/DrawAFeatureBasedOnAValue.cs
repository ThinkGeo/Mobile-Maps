using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DrawAFeatureBasedOnAValue : BaseViewController
    {
        public DrawAFeatureBasedOnAValue()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-135.7, 83.6, 113.5, -53);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "CNTRY_NAME";
            valueStyle.ValueItems.Add(new ValueItem("United States", new AreaStyle(new GeoSolidBrush(GeoColors.LightGreen))));

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA20", worldLayer);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 80, null);
        }
    }
}