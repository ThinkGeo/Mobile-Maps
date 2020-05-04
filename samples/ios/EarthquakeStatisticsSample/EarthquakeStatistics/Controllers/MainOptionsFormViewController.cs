using Foundation;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public partial class MainOptionsFormViewController : UIViewController
    {
        private DataTableSource baseMapTypeSource;
        private ShapeFileFeatureLayer earthquakeHeatLayer;
        private ShapeFileFeatureLayer earthquakePointLayer;
        private IsoLineLayer earthquakeIsoLineLayer;
        private UILabel baseMapLabel;

        public Action QueryEarthquakeResult;
        public Action<string> OptionRowClick;

        public MainOptionsFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeOptionsSource();
            InitializeComponent();
        }

        public override void ViewDidAppear(bool animated)
        {
            baseMapLabel.Text = Global.BaseMapTypeString ?? "ThinkGeo Cloud";
        }

        private void InitializeOptionsSource()
        {
            earthquakePointLayer = (ShapeFileFeatureLayer)Global.HighLightOverlay.Layers["EarthquakePointLayer"];
            earthquakeHeatLayer = (ShapeFileFeatureLayer)Global.HighLightOverlay.Layers["EarthquakeHeatLayer"];
            earthquakeIsoLineLayer = (IsoLineLayer)Global.HighLightOverlay.Layers["EarthquakeIsoLineLayer"];

            int rowWidth = 400;
            int sliderWith = 190;
            if (Global.UserInterfaceIdiomIsPhone)
            {
                rowWidth = 300;
                sliderWith = 90;
            }

            baseMapTypeSource = new DataTableSource();
            baseMapTypeSource.RowClick += RowClick;

            RectangleF rowFrame = new RectangleF(10, 5, rowWidth, 30);

            Collection<RowModel> baseMapRows = new Collection<RowModel>();
            RowModel baseMapModel = new RowModel("Base Map");
            baseMapLabel = new UILabel(new RectangleF(5, 5, rowWidth * 0.6f, 40))
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.Black,
                UserInteractionEnabled = false
            };

            baseMapModel.CellAccessory = UITableViewCellAccessory.DisclosureIndicator;
            baseMapModel.CustomUI = baseMapLabel;
            baseMapModel.CustomUIBounds = new RectangleF(rowWidth * 0.35f, 10, rowWidth * 0.6f, 30);
            baseMapModel.RowHeight = 50;
            baseMapRows.Add(baseMapModel);
            baseMapTypeSource.Sections.Add(new SectionModel("Base Map Type", baseMapRows) { HeaderHeight = 30 });

            Collection<RowModel> displayTypeRows = new Collection<RowModel>();
            RowModel displayTypeModel = new RowModel(string.Empty);
            UISegmentedControl displayTypeSegment = new UISegmentedControl();
            displayTypeSegment.InsertSegment("Point Type", 1, true);
            displayTypeSegment.InsertSegment("Heat Style", 2, true);
            displayTypeSegment.InsertSegment("IsoLine Style", 3, true);
            displayTypeSegment.ValueChanged += DisplayTypeSegmentValueChanged;
            displayTypeSegment.SelectedSegment = 0;
            displayTypeModel.CustomUI = displayTypeSegment;
            displayTypeModel.CustomUIBounds = new RectangleF(10, 10, rowWidth, 30);
            displayTypeModel.RowHeight = 50;

            displayTypeRows.Add(displayTypeModel);
            baseMapTypeSource.Sections.Add(new SectionModel("Display Type", displayTypeRows) { HeaderHeight = 30 });

            Collection<RowModel> queryOperation = new Collection<RowModel>();
            RowModel magnitude = new RowModel(string.Empty);
            RowModel depth = new RowModel(string.Empty);
            RowModel date = new RowModel(string.Empty);

            RectangleF silderFrame = new RectangleF(150, 0, sliderWith, 30);

            RangeSliderView magnitudeView = new RangeSliderView(new UIRangeSlider(
                silderFrame,
                Global.QueryConfiguration.LowerMagnitude,
                Global.QueryConfiguration.UpperMagnitude,
                Global.QueryConfiguration.LowerMagnitude,
                Global.QueryConfiguration.UpperMagnitude)
            { Name = "Magnitude:" })
            {
                Frame = rowFrame
            };
            magnitude.CustomUI = magnitudeView;
            magnitude.CustomUIBounds = rowFrame;
            magnitude.RowHeight = 45;

            RangeSliderView depthView = new RangeSliderView(new UIRangeSlider(
                silderFrame,
                Global.QueryConfiguration.LowerDepth,
                Global.QueryConfiguration.UpperDepth,
                Global.QueryConfiguration.LowerDepth,
                Global.QueryConfiguration.UpperDepth)
            { Name = "Depth(KM):" })
            {
                Frame = rowFrame
            };
            depth.CustomUI = depthView;
            depth.CustomUIBounds = rowFrame;
            depth.RowHeight = 45;

            RangeSliderView dateView = new RangeSliderView(new UIRangeSlider(
                silderFrame,
                Global.QueryConfiguration.LowerYear,
                Global.QueryConfiguration.UpperYear,
                Global.QueryConfiguration.LowerYear,
                Global.QueryConfiguration.UpperYear)
            { Name = "Date(Year):" })
            {
                Frame = rowFrame
            };
            date.CustomUI = dateView;
            date.CustomUIBounds = rowFrame;
            date.RowHeight = 45;

            queryOperation.Add(magnitude);
            queryOperation.Add(depth);
            queryOperation.Add(date);
            baseMapTypeSource.Sections.Add(new SectionModel("Query Operation", queryOperation) { HeaderHeight = 30 });
            BaseMapTypeTableView.Source = baseMapTypeSource;
        }

        private void InitializeComponent()
        {
            //QueryButton.Layer.CornerRadius = 5;
        }

        private void RowClick(UITableView tableView, NSIndexPath indexPath)
        {
            if (OptionRowClick != null)
            {
                DataTableSource source = (DataTableSource)tableView.Source;
                string itemName = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;

                OptionRowClick(itemName);
            }
        }

        private void DisplayTypeSegmentValueChanged(object sender, EventArgs e)
        {
            UISegmentedControl displayTypeSegment = (UISegmentedControl)sender;

            earthquakePointLayer.IsVisible = false;
            earthquakeHeatLayer.IsVisible = false;
            earthquakeIsoLineLayer.IsVisible = false;

            switch (displayTypeSegment.SelectedSegment)
            {
                case 0:
                    earthquakePointLayer.IsVisible = true;
                    break;
                case 1:
                    earthquakeHeatLayer.IsVisible = true;
                    break;
                case 2:
                    earthquakeIsoLineLayer.IsVisible = true;
                    break;
            }

            Global.HighLightOverlay.Refresh();
        }

        partial void CloseButton_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }

        partial void QueryButton_TouchUpInside(UIButton sender)
        {
            if (QueryEarthquakeResult != null)
            {
                if (Global.UserInterfaceIdiomIsPhone)
                    DismissViewController(true, () => QueryEarthquakeResult());
                else
                    QueryEarthquakeResult();
            }
        }
    }
}