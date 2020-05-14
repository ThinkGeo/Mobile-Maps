using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;

namespace RoutingSample_iOS
{
    class DirectionTableViewDataSource : UITableViewDataSource
    {
        private static string cellIdentifier = "cellIdentifier";
        private List<DirectionDataItem> directionDataList;

        public DirectionTableViewDataSource(List<DirectionDataItem> directions)
        {
            directionDataList = directions;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            if(cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
            }

            cell.TextLabel.Font = UIFont.SystemFontOfSize(10);
            cell.TextLabel.TextColor = UIColor.FromRGB(51, 51, 51);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, cell.Frame.Width, 30);

            var cellWidth = cell.Frame.Width / 3;
            var roadNameView = new UITextView(new CGRect(5, 5, cellWidth + 15, cell.Frame.Height));
            roadNameView.Text = directionDataList[indexPath.Row].RoadName;
            roadNameView.Font = UIFont.SystemFontOfSize(10);
            var directionView = new UITextView(new CGRect(20 + cellWidth, 5, cellWidth, cell.Frame.Height));
            directionView.Text = directionDataList[indexPath.Row].Direction;
            directionView.Font = UIFont.SystemFontOfSize(10);
            var lengthView = new UITextView(new CGRect(20 + 2 * cellWidth, 5, cellWidth - 15, cell.Frame.Height));
            lengthView.Text = directionDataList[indexPath.Row].Length.ToString();
            lengthView.Font = UIFont.SystemFontOfSize(10);

            cell.AddSubview(roadNameView);
            cell.AddSubview(directionView);
            cell.AddSubview(lengthView);

            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return directionDataList.Count;
        }
    }
}
