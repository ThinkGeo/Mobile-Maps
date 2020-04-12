/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreAnimation;
using CoreGraphics;
using System.Linq;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace AnalyzingVisualization
{
    public class FilterStyleViewController : DetailViewController
    {
        private UIButton btnApply;
        private UILabel lblColumn;
        private UITextField inputText;
        private InstructionView instructionView;
        private FilterStyle filterStyle;
        private SimpleFilterConditionType currentSimpleFilterConditionType;
        private ShapeFileFeatureLayer statesLayer;

        public FilterStyleViewController()
        {
            SettingContainerHeight = 120;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeSettingView();
            ApplyFilterStyle();
        }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512);

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            string thinkgeoCloudClientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
            string thinkgeoCloudClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(thinkgeoCloudClientKey, thinkgeoCloudClientSecret);
            MapView.Overlays.Add(thinkGeoCloudMapsOverlay);

            statesLayer = new ShapeFileFeatureLayer("AppData/states.shp");
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.Layers.Add(statesLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            GeoColor strokeColor = GeoColor.FromHtml("#333333");
            GeoColor fillColor = GeoColor.FromArgb(130, GeoColor.FromHtml("#ffb74c"));

            filterStyle = new FilterStyle();
            filterStyle.Conditions.Add(new FilterCondition("Population", ">2967297"));
            filterStyle.Styles.Add(AreaStyle.CreateSimpleAreaStyle(fillColor, strokeColor));
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(filterStyle);
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            MapView.ZoomTo(new PointShape(-10777397, 3821690), MapView.ZoomLevelSet.ZoomLevel05.Scale);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            CALayer attributionLayer = MapView.EventView.Layer.Sublayers.FirstOrDefault(l => l.Name.Equals("AttributionLayer"));
            attributionLayer.Frame = new CGRect(0, -SettingContainerHeight, attributionLayer.Frame.Width, attributionLayer.Frame.Height);
        }

        private void InitializeSettingView()
        {
            SettingButton.Enabled = true;
            instructionView = new InstructionView(RefreshInstructionView, InstructionExpandStateChanged);
            SettingButtonClick = InstructionExpandStateChanged;
            instructionView.TranslatesAutoresizingMaskIntoConstraints = false;

            NSLayoutConstraint[] instructionConstraints =
            {
                NSLayoutConstraint.Create(instructionView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, SettingContainerView, NSLayoutAttribute.Left, 1, 0),
                NSLayoutConstraint.Create(instructionView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, SettingContainerView, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(instructionView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, SettingContainerView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(instructionView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, SettingContainerView, NSLayoutAttribute.Bottom, 1, 0)
            };

            SettingContainerView.Add(instructionView);
            SettingContainerView.AddConstraints(instructionConstraints);
            View.BringSubviewToFront(SettingContainerView);
        }

        private void RefreshInstructionView(UIView contentView)
        {
            contentView.ClipsToBounds = true;
            FilterConditionDefaultValues filterConditionDefaultValues = new FilterConditionDefaultValues();

            lblColumn = new UILabel();
            lblColumn.Text = filterConditionDefaultValues.FirstOrDefault().Value.Item1;
            if (iOSCapabilityHelper.IsOnIPhone) lblColumn.Font = UIFont.FromName("Arial", 13);
            lblColumn.TextColor = UIColor.White;
            lblColumn.TranslatesAutoresizingMaskIntoConstraints = false;

            inputText = new UITextField();
            inputText.Text = filterConditionDefaultValues.FirstOrDefault().Value.Item2;
            inputText.TextAlignment = UITextAlignment.Center;
            inputText.TextColor = UIColor.White;
            if (iOSCapabilityHelper.IsOnIPhone) inputText.Font = UIFont.FromName("Arial", 13);
            inputText.TranslatesAutoresizingMaskIntoConstraints = false;

            UIPickerView filterConditionTypePicker = new UIPickerView();
            filterConditionTypePicker.Model = new FilterStyleConditionModel
            {
                RowSelected = s =>
                {
                    currentSimpleFilterConditionType = s;
                    lblColumn.Text = filterConditionDefaultValues.SingleOrDefault(t => t.Key.Equals(s)).Value.Item1;
                    inputText.Text = filterConditionDefaultValues.SingleOrDefault(t => t.Key.Equals(s)).Value.Item2;
                    inputText.Hidden = s.ToString().Contains("Empty");
                }
            };
            filterConditionTypePicker.ShowSelectionIndicator = true;
            filterConditionTypePicker.TranslatesAutoresizingMaskIntoConstraints = false;

            btnApply = new UIButton(UIButtonType.RoundedRect);
            btnApply.BackgroundColor = UIColor.DarkGray;
            btnApply.TintColor = UIColor.White;
            btnApply.SetTitle("Apply", UIControlState.Normal);
            btnApply.TouchUpInside += (sender, args) => ApplyFilterStyle();
            if (iOSCapabilityHelper.IsOnIPhone) btnApply.Font = UIFont.FromName("Arial", 13);
            btnApply.TranslatesAutoresizingMaskIntoConstraints = false;

            int columnWidth = 120;
            int filterConditionTypePickerWidth = 180;
            int btnApplyLeft = 80;
            int btnApplyTop = 10;

            if (iOSCapabilityHelper.IsOnIPhone)
            {
                columnWidth = 95;
                filterConditionTypePickerWidth = 137;
                btnApplyLeft = 10;
                btnApplyTop = 40;
            }

            NSLayoutConstraint[] settingViewConstraints =
            {
                NSLayoutConstraint.Create(lblColumn, NSLayoutAttribute.Left, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Left, 1, 0),
                NSLayoutConstraint.Create(lblColumn, NSLayoutAttribute.Top, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Top, 1, 10),
                NSLayoutConstraint.Create(lblColumn, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, columnWidth),
                NSLayoutConstraint.Create(lblColumn, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30),

                NSLayoutConstraint.Create(filterConditionTypePicker, NSLayoutAttribute.Left, NSLayoutRelation.Equal, lblColumn, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(filterConditionTypePicker, NSLayoutAttribute.Top, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Top, 1, 10),
                NSLayoutConstraint.Create(filterConditionTypePicker, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, filterConditionTypePickerWidth),
                NSLayoutConstraint.Create(filterConditionTypePicker, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30),

                NSLayoutConstraint.Create(inputText, NSLayoutAttribute.Left, NSLayoutRelation.Equal, filterConditionTypePicker, NSLayoutAttribute.Right, 1, 1),
                NSLayoutConstraint.Create(inputText, NSLayoutAttribute.Top, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Top, 1, 10),
                NSLayoutConstraint.Create(inputText, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 70),
                NSLayoutConstraint.Create(inputText, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30),

                NSLayoutConstraint.Create(btnApply, NSLayoutAttribute.Left, NSLayoutRelation.Equal, filterConditionTypePicker, NSLayoutAttribute.Right, 1,btnApplyLeft),
                NSLayoutConstraint.Create(btnApply, NSLayoutAttribute.Top, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Top, 1, btnApplyTop),
                NSLayoutConstraint.Create(btnApply, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 50),
                NSLayoutConstraint.Create(btnApply, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30)
            };

            contentView.Add(lblColumn);
            contentView.Add(filterConditionTypePicker);
            contentView.Add(inputText);
            contentView.Add(btnApply);
            contentView.AddConstraints(settingViewConstraints);
        }

        private void InstructionExpandStateChanged()
        {
            float y = SettingContainerHeight;
            CALayer attributionLayer = MapView.EventView.Layer.Sublayers.FirstOrDefault(l => l.Name.Equals("AttributionLayer"));
            if (SettingContainerView.Hidden)
            {
                SettingContainerView.AnimatedShow(AnimateType.Up);
            }
            else
            {
                SettingContainerView.AnimatedHide(AnimateType.Down);
                y = 0;
            }
            attributionLayer.Frame = new CGRect(0, -y, attributionLayer.Frame.Width, attributionLayer.Frame.Height);
        }

        private void ApplyFilterStyle()
        {
            filterStyle.Conditions.Clear();
            SimpleFilterCondition newCondition = new SimpleFilterCondition(lblColumn.Text, currentSimpleFilterConditionType, inputText.Text);
            filterStyle.Conditions.Add(newCondition);

            MapView.Overlays["LayerOverlay"].Refresh();
        }
    }
}