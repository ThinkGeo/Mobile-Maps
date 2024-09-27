using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    internal class LoadAHeatMapLayer : BaseViewController
    {
        public LoadAHeatMapLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;

            ShapeFileFeatureLayer heatLayer = new ShapeFileFeatureLayer("AppData/SampleData/cities_e.shp");
            heatLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 75, DistanceUnit.Kilometer));
            heatLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(heatLayer);

            heatLayer.Open();
            MapView.CurrentExtent = heatLayer.GetBoundingBox();
            heatLayer.Close();

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}