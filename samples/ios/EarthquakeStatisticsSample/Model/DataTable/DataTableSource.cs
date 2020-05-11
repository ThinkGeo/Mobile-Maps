using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    internal class DataTableSource : UITableViewSource
    {
        public Action<UITableView, NSIndexPath> RowClick;

        public Collection<SectionModel> Sections { get; private set; }

        public DataTableSource()
            : this(null)
        { }

        public DataTableSource(IEnumerable<SectionModel> rows)
        {
            Sections = new Collection<SectionModel>();
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    Sections.Add(item);
                }
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RowModel currentModel = Sections[indexPath.Section].Rows[indexPath.Row];

            UITableViewCell cell = tableView.DequeueReusableCell("cell") ??
                                   new UITableViewCell(UITableViewCellStyle.Subtitle, "Cell");

            switch (currentModel.CellAccessory)
            {
                case UITableViewCellAccessory.Checkmark:
                    cell.Accessory = currentModel.IsChecked ? currentModel.CellAccessory : UITableViewCellAccessory.None;
                    break;
                default:
                    cell.Accessory = currentModel.CellAccessory;
                    break;
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = currentModel.Name;
            cell.AccessoryView = currentModel.AccessoryView;

            if (currentModel.CustomUI != null)
            {
                currentModel.CustomUI.Frame = currentModel.CustomUIBounds;
                cell.Add(currentModel.CustomUI);
            }

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Sections.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Sections[(int)section].Rows.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return Sections[(int)section].Title;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            RowClick?.Invoke(tableView, indexPath);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            SectionModel currentSection = Sections[(int)section];
            if (currentSection.HeaderHeight > 0) return currentSection.HeaderHeight;
            return UITableView.AutomaticDimension;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            RowModel currentModel = Sections[indexPath.Section].Rows[indexPath.Row];
            if (currentModel.RowHeight > 0) return currentModel.RowHeight;
            return UITableView.AutomaticDimension;
        }
    }
}