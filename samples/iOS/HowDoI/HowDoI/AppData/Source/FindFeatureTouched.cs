using CoreGraphics;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class FindFeatureTouched : BaseViewController
    {
        public FindFeatureTouched()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            ThinkGeoCloudRasterMapsOverlay.TileCache = null;
            MapView.Overlays.Add("ThinkGeoCloudRasterMapsOverlay", ThinkGeoCloudRasterMapsOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileSnappingMode = UI.iOS.TileSnappingMode.Snapping;
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.TileWidth = 512;
            highlightOverlay.TileHeight = 512;
            highlightOverlay.TileSnappingMode = UI.iOS.TileSnappingMode.Snapping;
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            worldLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.MapSingleTap += MapView_MapSingleTap;
            MapView.Refresh();
        }

        private void MapView_MapSingleTap(object sender, UIGestureRecognizer e)
        {
            CGPoint location = e.LocationInView(MapView);
            PointShape position = MapUtil.ToWorldCoordinate(MapView.CurrentExtent, (float)location.X,
                (float)location.Y, (float)MapView.Frame.Width, (float)MapView.Frame.Height);

            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(position, new[] { "CNTRY_NAME" });

            highlightLayer.InternalFeatures.Clear();
            if (selectedFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
            }

            highlightLayer.Close();
            highlightOverlay.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}