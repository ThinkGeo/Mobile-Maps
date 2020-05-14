using CoreGraphics;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ChangeEditSettings : BaseViewController
    {
        private UISwitch swCanDrag;
        private UISwitch swCanResize;
        private UISwitch swCanRotate;
        private UISwitch swCanReShape;

        public ChangeEditSettings()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-79.303125, 76.471875, 0.853125000000006, -38.840625);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);
            MapView.Overlays.Add(layerOverlay);

            Feature feature = new Feature(new RectangleShape(-55.5723249724898, 15.7443857300058, -10.5026750275102, -7.6443857300058));
            MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 150 : 130, (contentView) =>
            {
                float canResizeLeft = SampleUIHelper.IsOnIPhone ? 0 : 280;
                float canResizeTop = SampleUIHelper.IsOnIPhone ? 35 : 0;
                float swCanResizeLeft = SampleUIHelper.IsOnIPhone ? 60 : 350;
                float swCanResizeTop = SampleUIHelper.IsOnIPhone ? 35 : 0;
                float canRotateLeft = SampleUIHelper.IsOnIPhone ? 120 : 420;
                float canRotateTop = SampleUIHelper.IsOnIPhone ? 35 : 0;
                float swCanRotateLeft = SampleUIHelper.IsOnIPhone ? 210 : 480;
                float swCanRotateTop = SampleUIHelper.IsOnIPhone ? 35 : 0;

                UILabel dragLabel = new UILabel(new CGRect(0, 0, 80, 30));
                dragLabel.Text = "Drag:";
                dragLabel.TextColor = UIColor.White;
                dragLabel.ShadowColor = UIColor.Gray;
                dragLabel.ShadowOffset = new CGSize(1, 1);

                swCanDrag = new UISwitch(new CGRect(60, 0, 100, 30));
                swCanDrag.On = false;
                swCanDrag.ValueChanged += sw_ValueChanged;

                UILabel reShapeLabel = new UILabel(new CGRect(120, 0, 100, 30));
                reShapeLabel.Text = "ReShape:";
                reShapeLabel.TextColor = UIColor.White;
                reShapeLabel.ShadowColor = UIColor.Gray;
                reShapeLabel.ShadowOffset = new CGSize(1, 1);

                swCanReShape = new UISwitch(new CGRect(210, 0, 100, 30));
                swCanReShape.On = false;
                swCanReShape.ValueChanged += sw_ValueChanged;

                UILabel canResize = new UILabel(new CGRect(canResizeLeft, canResizeTop, 100, 30));
                canResize.Text = "Resize:";
                canResize.TextColor = UIColor.White;
                canResize.ShadowColor = UIColor.Gray;
                canResize.ShadowOffset = new CGSize(1, 1);

                swCanResize = new UISwitch(new CGRect(swCanResizeLeft, swCanResizeTop, 100, 30));
                swCanResize.On = false;
                swCanResize.ValueChanged += sw_ValueChanged;

                UILabel canRotate = new UILabel(new CGRect(canRotateLeft, canRotateTop, 100, 30));
                canRotate.Text = "Rotate:";
                canRotate.TextColor = UIColor.White;
                canRotate.ShadowColor = UIColor.Gray;
                canRotate.ShadowOffset = new CGSize(1, 1);

                swCanRotate = new UISwitch(new CGRect(swCanRotateLeft, swCanRotateTop, 100, 25));
                swCanRotate.On = false;
                swCanRotate.ValueChanged += sw_ValueChanged;

                contentView.AddSubviews(new UIView[] { dragLabel, reShapeLabel, canResize, canRotate, swCanDrag, swCanReShape, swCanResize, swCanRotate });
            });
        }

        private void sw_ValueChanged(object sender, System.EventArgs e)
        {
            MapView.EditOverlay.CanReshape = swCanReShape.On;
            MapView.EditOverlay.CanResize = swCanResize.On;
            MapView.EditOverlay.CanRotate = swCanRotate.On;
            MapView.EditOverlay.CanDrag = swCanDrag.On;

            MapView.EditOverlay.CalculateAllControlPoints();
            MapView.Refresh();
        }
    }
}