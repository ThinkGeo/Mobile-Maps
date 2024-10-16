using CoreGraphics;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ZoomInToAFeatureTouched : BaseViewController
    {
        public ZoomInToAFeatureTouched()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            MapView.SingleTap += MapViewOnSingleTap;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            highlightOverlay.TileType = TileType.SingleTile;
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            MapView.Refresh();
        }

        private void MapViewOnSingleTap(object? sender, TouchMapViewEventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            var worldLocation = e.PointInWorldCoordinate;
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(worldLocation, new[] { "CNTRY_NAME" });

            highlightLayer.InternalFeatures.Clear();
            if (selectedFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
                MapView.ZoomTo(selectedFeatures[0].GetBoundingBox());
            }

            highlightOverlay.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}