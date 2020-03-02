using Foundation;
using UIKit;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace MapSuiteSiteSelection
{
    public partial class MainOptionFormViewController : UIViewController
    {
        private DataTableSource optionsSource;
        private RowModel typeFilterModel;
        private UIButton unitTypeButton;
        private UILabel baseMapLabel;

        public Action QueryEarthquakeResult;
        public Action<string> OptionRowClick;

        public MainOptionFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Site Selection Setting";
            InitializeOptionsSource();
        }

        public override void ViewDidAppear(bool animated)
        {
            typeFilterModel.Name = Global.FilterConfiguration.QueryColumnName + "/" + Global.FilterConfiguration.QueryColumnValue;
            unitTypeButton.SetTitle(Global.FilterConfiguration.BufferDistanceUnit + "s", UIControlState.Normal);
            baseMapLabel.Text = Global.BaseMapTypeString ?? "ThinkGeo Cloud Maps Light";
            OptionsTableView.ReloadData();
        }

        private void InitializeOptionsSource()
        {
            int rowWidth = 400;
            if (Global.UserInterfaceIdiomIsPhone)
            {
                rowWidth = 300;
            }

            optionsSource = new DataTableSource();
            optionsSource.RowClick += RowClick;

            Collection<RowModel> baseMapRows = new Collection<RowModel>();
            RowModel baseMapModel = new RowModel("Base Map");
            baseMapLabel = new UILabel(new RectangleF(5, 5, rowWidth * 0.6f, 40));
            baseMapLabel.TextAlignment = UITextAlignment.Right;
            baseMapLabel.TextColor = UIColor.Black;
            baseMapLabel.UserInteractionEnabled = false;

            baseMapModel.CellAccessory = UITableViewCellAccessory.DisclosureIndicator;
            baseMapModel.CustomUI = baseMapLabel;
            baseMapModel.CustomUIBounds = new RectangleF(rowWidth * 0.35f, 10, rowWidth * 0.6f, 30);
            baseMapModel.RowHeight = 50;
            baseMapRows.Add(baseMapModel);
            optionsSource.Sections.Add(new SectionModel("Base Map Type", baseMapRows) { HeaderHeight = 30 });

            Collection<RowModel> typeFilterRows = new Collection<RowModel>();
            typeFilterModel = new RowModel(Global.FilterConfiguration.QueryColumnName + "/" + Global.FilterConfiguration.QueryColumnValue);
            typeFilterModel.RowHeight = 50;
            typeFilterModel.CellAccessory = UITableViewCellAccessory.DisclosureIndicator;
            typeFilterRows.Add(typeFilterModel);
            optionsSource.Sections.Add(new SectionModel("Filter By Type", typeFilterRows) { HeaderHeight = 30 });

            Collection<RowModel> areaFilterRows = new Collection<RowModel>();
            RowModel areaFilterModel = new RowModel("UnitType");
            UITextView textView = new UITextView(new RectangleF(0, 0, rowWidth * 0.7f, 40));
            textView.Font = UIFont.FromName("Arial", 16);
            textView.Text = "20";
            textView.Changed += DistanceChanged;
            unitTypeButton = new UIButton(new RectangleF(rowWidth * 0.65f, 0, rowWidth * 0.3f, 40));
            unitTypeButton.TitleLabel.TextAlignment = UITextAlignment.Right;
            unitTypeButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            unitTypeButton.SetTitle("Miles", UIControlState.Normal);
            unitTypeButton.TouchUpInside += UnitTypeButtonTouchUpInside;

            UIView areaView = new UIView(new RectangleF(10, 5, rowWidth, 50));
            areaView.Add(textView);
            areaView.Add(unitTypeButton);

            areaFilterModel.CellAccessory = UITableViewCellAccessory.DisclosureIndicator;
            areaFilterModel.CustomUI = areaView;
            areaFilterModel.CustomUIBounds = new RectangleF(10, 5, rowWidth - 10, 30);
            areaFilterModel.RowHeight = 50;
            areaFilterRows.Add(areaFilterModel);
            optionsSource.Sections.Add(new SectionModel("Buffered Distance", areaFilterRows) { HeaderHeight = 30 });

            OptionsTableView.Source = optionsSource;
        }

        private void UnitTypeButtonTouchUpInside(object sender, EventArgs e)
        {
            OptionRowClick?.Invoke("UnitType");
        }

        private void DistanceChanged(object sender, EventArgs e)
        {
            UITextView textView = (UITextView)sender;
            if (double.TryParse(textView.Text, out double result) == true)
            {
                Global.FilterConfiguration.BufferValue = result;
            }
            else
            {
                textView.Text = "20";
            }
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

        partial void CloseButtonTouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }

        partial void QueryButtonTouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
            if (QueryEarthquakeResult != null)
            {
                QueryEarthquakeResult();
            }
        }
    }
}
