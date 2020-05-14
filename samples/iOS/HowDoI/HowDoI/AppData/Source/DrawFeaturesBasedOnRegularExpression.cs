using CoreGraphics;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DrawFeaturesBasedOnRegularExpression : BaseViewController
    {
        public DrawFeaturesBasedOnRegularExpression()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-135.7, 83.6, 113.5, -53);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            RegexStyle regexStyle = new RegexStyle();
            regexStyle.ColumnName = "CNTRY_NAME";
            regexStyle.RegexItems.Add(new RegexItem(".*land", new AreaStyle(new GeoSolidBrush(GeoColors.LightGreen))));

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(regexStyle);
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(worldLayer);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 180 : 110, (contentView) =>
            {
                UILabel descriptionLabelView = new UILabel(new CGRect(0, 0, contentView.Frame.Width - 20, 30));

                descriptionLabelView.Text = "RegexStyle: ColumnName=\"CNTRY_NAME\" regularExpression=\".*land\",";
                descriptionLabelView.TextColor = UIColor.White;
                descriptionLabelView.ShadowColor = UIColor.Gray;
                descriptionLabelView.ShadowOffset = new CGSize(1, 1);
                descriptionLabelView.LineBreakMode = UILineBreakMode.WordWrap;
                descriptionLabelView.Lines = 0;
                descriptionLabelView.PreferredMaxLayoutWidth = contentView.Frame.Width - 20;
                descriptionLabelView.SizeToFit();

                contentView.AddSubviews(new UIView[] { descriptionLabelView });
            });
        }
    }
}