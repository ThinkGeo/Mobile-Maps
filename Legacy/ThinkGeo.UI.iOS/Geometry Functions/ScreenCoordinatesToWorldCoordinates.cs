using CoreGraphics;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ScreenCoordinatesToWorldCoordinates : BaseViewController
    {
        private UILabel screenPositionLable;
        private UILabel worldPositionLable;

        public ScreenCoordinatesToWorldCoordinates()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            MapView.SingleTap += MapViewOnSingleTap;
            
            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(worldLayer);

            MapView.Refresh();
        }

        private void MapViewOnSingleTap(object? sender, TouchMapViewEventArgs e)
        {
            var location = e.PointInScreenCoordinate;
            var worldPosition = e.PointInWorldCoordinate;
            
            screenPositionLable.Text = string.Format("Screen Position:({0:N4},{1:N4})", location.X, location.Y);
            worldPositionLable.Text = string.Format("World Position: ({0:N4},{1:N4})", worldPosition.X, worldPosition.Y);
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 160, contentView =>
            {
                screenPositionLable = new UILabel(new CGRect(0, 0, 400, 30));

                screenPositionLable.Text = "Screen Position:";
                screenPositionLable.TextColor = UIColor.White;
                screenPositionLable.ShadowColor = UIColor.Gray;
                screenPositionLable.ShadowOffset = new CGSize(1, 1);

                worldPositionLable = new UILabel(new CGRect(0, 30, 400, 30));
                worldPositionLable.Text = "World Position:";
                worldPositionLable.TextColor = UIColor.White;
                worldPositionLable.ShadowColor = UIColor.Gray;
                worldPositionLable.ShadowOffset = new CGSize(1, 1);

                contentView.AddSubviews(new UIView[] { screenPositionLable, worldPositionLable });
            });
        }
    }
}