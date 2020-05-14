using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class AddVariousMapTools : BaseViewController
    {
        public AddVariousMapTools()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);
            layerOverlay.Layers.Add(worldLayer);

            MapView.Refresh();
        }

        private void ZoomToolSwichValueChanged(object sender, EventArgs e)
        {
            UISwitch zoomToolSwich = (UISwitch)sender;
            MapView.MapTools.ZoomMapTool.IsEnabled = zoomToolSwich.On;
        }

        private void GloblleSwichValueChanged(object sender, EventArgs e)
        {
            UISwitch zoomToolSwich = (UISwitch)sender;
            MapView.MapTools.ZoomMapTool.IsGlobeButtonVisiable = zoomToolSwich.On;
        }

        private void ScaleLineToolSwichValueChanged(object sender, EventArgs e)
        {
            UISwitch zoomToolSwich = (UISwitch)sender;
            MapView.MapTools.ScaleLine.IsEnabled = zoomToolSwich.On;
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 210 : 120, (contentView) =>
            {
                float globleLabelViewLeft = SampleUIHelper.IsOnIPhone ? 0 : 170;
                float globleLabelViewTop = SampleUIHelper.IsOnIPhone ? 35 : 0;
                float globeButtonSwichLeft = SampleUIHelper.IsOnIPhone ? 170 : 340;
                float globeButtonSwichTop = SampleUIHelper.IsOnIPhone ? 35 : 0;
                float scaleLineLabelViewLeft = SampleUIHelper.IsOnIPhone ? 0 : 400;
                float scaleLineLabelViewTop = SampleUIHelper.IsOnIPhone ? 70 : 0;
                float scaleLineToolSwichLeft = SampleUIHelper.IsOnIPhone ? 130 : 530;
                float scaleLineToolSwichTop = SampleUIHelper.IsOnIPhone ? 70 : 0;

                UILabel zoomToolLabelView = new UILabel(new CGRect(0, 0, 100, 30));
                zoomToolLabelView.Text = "Zoom tool:";
                zoomToolLabelView.TextColor = UIColor.White;
                zoomToolLabelView.ShadowColor = UIColor.Gray;
                zoomToolLabelView.ShadowOffset = new CGSize(1, 1);

                UISwitch zoomToolSwich = new UISwitch(new CGRect(100, 0, 50, 58));
                zoomToolSwich.On = true;
                zoomToolSwich.ValueChanged += ZoomToolSwichValueChanged;

                UILabel globleLabelView = new UILabel(new CGRect(globleLabelViewLeft, globleLabelViewTop, 170, 30));
                globleLabelView.Text = "Show globe button:";
                globleLabelView.TextColor = UIColor.White;
                globleLabelView.ShadowColor = UIColor.Gray;
                globleLabelView.ShadowOffset = new CGSize(1, 1);

                UISwitch globeButtonSwich = new UISwitch(new CGRect(globeButtonSwichLeft, globeButtonSwichTop, 50, 58));
                globeButtonSwich.On = false;
                globeButtonSwich.ValueChanged += GloblleSwichValueChanged;

                UILabel scaleLineLabelView = new UILabel(new CGRect(scaleLineLabelViewLeft, scaleLineLabelViewTop, 150, 30));
                scaleLineLabelView.Text = "ScaleLine tool:";
                scaleLineLabelView.TextColor = UIColor.White;
                scaleLineLabelView.ShadowColor = UIColor.Gray;
                scaleLineLabelView.ShadowOffset = new CGSize(1, 1);

                UISwitch scaleLineToolSwich = new UISwitch(new CGRect(scaleLineToolSwichLeft, scaleLineToolSwichTop, 50, 58));
                scaleLineToolSwich.On = false;
                scaleLineToolSwich.ValueChanged += ScaleLineToolSwichValueChanged;

                contentView.AddSubviews(new UIView[] { zoomToolLabelView, zoomToolSwich, globleLabelView, globeButtonSwich, scaleLineLabelView, scaleLineToolSwich });
            });
        }
    }
}