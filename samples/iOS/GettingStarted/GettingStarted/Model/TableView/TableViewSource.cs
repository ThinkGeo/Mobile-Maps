using Foundation;
using System;
using System.Collections.ObjectModel;
using UIKit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the TableViewSource class.
    /// </summary>
    public class TableViewSource : UITableViewSource
    {
        public event EventHandler<TableViewRowClickEventArgs> RowClick;

        public Collection<SectionModel> Sections { get; private set; }

        public TableViewSource()
        {
            Sections = new Collection<SectionModel>();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            CellModel currentModel = Sections[indexPath.Section].Rows[indexPath.Row];

            UITableViewCell cell = tableView.DequeueReusableCell("cell") ??
                                   new UITableViewCell(UITableViewCellStyle.Subtitle, "Cell");

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = currentModel.Name;
            cell.TextLabel.Font = UIFont.FromName("Arial", 13);

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

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            RowClick?.Invoke(this, new TableViewRowClickEventArgs(tableView, indexPath));
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 30;
        }
    }
}