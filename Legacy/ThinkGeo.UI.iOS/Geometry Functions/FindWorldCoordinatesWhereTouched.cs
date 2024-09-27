using CoreGraphics;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class FindWorldCoordinatesWhereTouched : BaseViewController
    {
        private UILabel longitudeLabelView;
        private UILabel latitudeLabelView;

        public FindWorldCoordinatesWhereTouched()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.SingleTap += MapViewOnSingleTap; 

            // ThinkGeoCloudRasterMapsOverlay worldMapKitOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            // MapView.Overlays.Add("WorldMapKitOverlay", worldMapKitOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            worldLayer.Open();
            MapView.CurrentExtent = worldLayer.GetBoundingBox();
            worldLayer.Close();

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        private void MapViewOnSingleTap(object? sender, TouchMapViewEventArgs e)
        {
            var position = e.PointInWorldCoordinate;
            
            longitudeLabelView.Text = string.Format("Longitude : {0:N4}", position.X);
            latitudeLabelView.Text = string.Format("Latitude : {0:N4}", position.Y);

            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            LayerOverlay highlightOverlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(position, new[] { "CNTRY_NAME" });

            highlightLayer.InternalFeatures.Clear();
            if (selectedFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
            }

            highlightOverlay.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 170 : 150, contentView =>
            {
                longitudeLabelView = new UILabel(new CGRect(0, 0, 400, 30));
                latitudeLabelView = new UILabel(new CGRect(0, 30, 400, 30));

                longitudeLabelView.Text = "Longitude:";
                longitudeLabelView.TextColor = UIColor.White;
                longitudeLabelView.ShadowColor = UIColor.Gray;
                longitudeLabelView.ShadowOffset = new CGSize(1, 1);

                latitudeLabelView.Text = "Latitude:";
                latitudeLabelView.TextColor = UIColor.White;
                latitudeLabelView.ShadowColor = UIColor.Gray;
                latitudeLabelView.ShadowOffset = new CGSize(1, 1);

                contentView.AddSubviews(new UIView[] { longitudeLabelView, latitudeLabelView });
            });
        }
    }
}