using Foundation;
using UIKit;
using System;
using System.Collections.Generic;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class TableSource : UITableViewSource
    {
        private List<CategoryNode> tableTree;
        private string cellIdentifier = "TableCell";

        public event EventHandler<SampleNode> TableSubNodeSelected;

        public TableSource(List<CategoryNode> nodes)
        {
            tableTree = nodes;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableTree[(int)section].Expand ? tableTree[(int)section].Child.Count + 1 : 1;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return tableTree.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);

            if (indexPath.Row == 0)
            {
                CategoryNode category = tableTree[indexPath.Section];
                cell.TextLabel.Text = string.Format("{0} ({1})", category.Name, category.Child.Count);

                if (tableTree[indexPath.Section].Expand)
                {
                    cell.AccessoryView = new UIImageView(UIImage.FromBundle("ArrowUp"));
                }
                else
                {
                    cell.AccessoryView = new UIImageView(UIImage.FromBundle("ArrowDown"));
                }
            }
            else if (tableTree[indexPath.Section].Expand)
            {
                SampleNode sample = tableTree[indexPath.Section].Child[indexPath.Row - 1];

                cell.TextLabel.Text = sample.Name;
                cell.DetailTextLabel.Text = sample.Description;
                cell.DetailTextLabel.Font = UIFont.FromName("Arial", 15);
                cell.DetailTextLabel.TextColor = UIColor.Gray;

                cell.ImageView.Image = UIImage.FromBundle("item");

                if (sample.Disabled)
                    cell.TextLabel.TextColor = UIColor.Gray;
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                CategoryNode category = tableTree[indexPath.Section];
                category.Expand = !category.Expand;
            }
            else
            {
                EventHandler<SampleNode> handler = TableSubNodeSelected;
                if (handler != null)
                {
                    handler(this, tableTree[indexPath.Section].Child[indexPath.Row - 1]);
                }
            }
            tableView.ReloadSections(new NSIndexSet((uint)indexPath.Section), UITableViewRowAnimation.Automatic);
            tableView.SelectRow(indexPath, true, UITableViewScrollPosition.Top);
        }

        //public override int IndentationLevel(UITableView tableView, NSIndexPath indexPath)
        //{
        //    return indexPath.Row == 0 ? 0 : 5;
        //}
    }
}