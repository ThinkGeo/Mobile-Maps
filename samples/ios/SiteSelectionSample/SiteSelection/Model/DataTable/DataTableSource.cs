using System.Drawing;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteSiteSelection
{
    public class DataTableSource : UITableViewSource
    {
        public Action<UITableView, NSIndexPath> RowClick;

        public Collection<SectionModel> Sections { get; private set; }

        public DataTableSource()
        {
            Sections = new Collection<SectionModel>();
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
            else return UITableView.AutomaticDimension;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            RowModel currentModel = Sections[indexPath.Section].Rows[indexPath.Row];
            if (currentModel.RowHeight > 0) return currentModel.RowHeight;
            else return UITableView.AutomaticDimension;
        }
    }
}