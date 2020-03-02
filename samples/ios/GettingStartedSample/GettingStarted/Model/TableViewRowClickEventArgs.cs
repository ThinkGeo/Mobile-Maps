using Foundation;
using System;
using UIKit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the TableViewRowClickEventArgs.
    /// </summary>
    public class TableViewRowClickEventArgs : EventArgs
    {
        public UITableView TableView { get; set; }

        public NSIndexPath IndexPath { get; set; }

        public TableViewRowClickEventArgs(UITableView tableView, NSIndexPath indexPath)
            : base()
        {
            TableView = tableView;
            IndexPath = indexPath;
        }
    }
}