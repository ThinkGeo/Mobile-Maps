using CoreGraphics;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ExtentAndScaleChangeEvents : BaseViewController
    {
        private UILabel labelExtent;
        private UILabel labelScale;

        public ExtentAndScaleChangeEvents()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtentChanged += MapView_CurrentExtentChanged;
            MapView.CurrentExtent = new RectangleShape(-141.5296875, 96.159375, -61.3734375, -19.153125);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(worldLayer);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 160 : 150, contentView =>
            {
                labelExtent = new UILabel(new CGRect(0, 0, contentView.Frame.Width, 30));
                labelExtent.TextColor = UIColor.White;
                labelExtent.ShadowColor = UIColor.Gray;
                labelExtent.ShadowOffset = new CGSize(1, 1);
                labelExtent.Lines = 0;
                labelExtent.LineBreakMode = UILineBreakMode.WordWrap;

                labelScale = new UILabel(new CGRect(0, 30, contentView.Frame.Width, 40));
                if (SampleUIHelper.IsOnIPhone)
                {
                    labelScale.Frame = new CGRect(0, 40, contentView.Frame.Width, 40);
                }

                labelScale.TextColor = UIColor.White;
                labelScale.ShadowColor = UIColor.Gray;
                labelScale.ShadowOffset = new CGSize(1, 1);

                contentView.AddSubviews(new UIView[] { labelExtent, labelScale });
            });
        }

        private void MapView_CurrentExtentChanged(object sender, CurrentExtentChangedMapViewEventArgs e)
        {
            PointShape upperLeftPoint = e.NewExtent.UpperLeftPoint;
            PointShape lowerRightPoint = e.NewExtent.LowerRightPoint;

            labelExtent.Text = string.Format("Map cureent extent: {0}, {1}, {2}, {3}", upperLeftPoint.X.ToString("n2"), upperLeftPoint.Y.ToString("n2"), lowerRightPoint.X.ToString("n2"), lowerRightPoint.Y.ToString("n2"));
            labelScale.Text = string.Format("Map cureent scale: {0}", MapUtil.GetScale(e.NewExtent, (float)MapView.Frame.Width, MapView.MapUnit).ToString("n4"));
            labelExtent.SizeToFit();
            labelScale.SizeToFit();
        }
    }
}