using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class MenuTableSource: UITableViewSource
    {
        SampleInfo[] sampleInfos;
        string CellIdentifier = "TableCell";
        private bool[] _isSectionOpen;
        private EventHandler _headerButtonCommand;
        private SideMenuController _sideMenuController;

        public MenuTableSource(SampleInfo[] items, UITableView tableView, SideMenuController sideMenuController)
        {
            sampleInfos = items;
            _isSectionOpen = new bool[sampleInfos.Length];
            _sideMenuController = sideMenuController;
            _headerButtonCommand = (sender, e) =>
            {
                var button = sender as UIButton;
                var section = button.Tag;
                _isSectionOpen[(int)section] = !_isSectionOpen[(int)section];
                tableView.ReloadData();

                // Animate the section cells
                var paths = new NSIndexPath[RowsInSection(tableView, section)];
                for (int i = 0; i < paths.Length; i++)
                {
                    paths[i] = NSIndexPath.FromItemSection(i, section);
                }

                tableView.ReloadRows(paths, UITableViewRowAnimation.Automatic);
            };
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return sampleInfos.Length;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 36;
        }

        public override nfloat EstimatedHeightForHeader(UITableView tableView, nint section)
        {
            return 36;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _isSectionOpen[(int)section] ? sampleInfos[(int)section].Children.Count : 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            string name = sampleInfos[indexPath.Section].Children[indexPath.Row].Name;

            //if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            }
            cell.TextLabel.Text = "   " + name;
            cell.TextLabel.Font = UIFont.SystemFontOfSize(15.0f);
            return cell;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView header =  new UIView(new RectangleF(0, 0, 320, 36));
            header.BackgroundColor = UIColor.White;
            var headerButton = CreateHeaderButton(header.Bounds, section,
                $"  {sampleInfos[(int)section].Name}({sampleInfos[(int)section].Children.Count})");
            header.AddSubview(headerButton);
            return header;
        }

        private UIButton CreateHeaderButton(CGRect frame, nint tag, string label)
        {
            var button = new UIButton(frame);
            button.Tag = tag;
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            button.SetTitle(label, UIControlState.Normal);
            button.Font = button.Font.WithSize(16f);

            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            button.TouchUpInside += _headerButtonCommand;
            return button;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            _sideMenuController.LaunchSampleByClassName(sampleInfos[indexPath.Section].Children[indexPath.Row].FullClassName, sampleInfos[indexPath.Section].Children[indexPath.Row].Name, sampleInfos[indexPath.Section].Children[indexPath.Row].Description);
            //var cell = tableView.CellAt(indexPath) as TableViewCell;
        }

    }
}