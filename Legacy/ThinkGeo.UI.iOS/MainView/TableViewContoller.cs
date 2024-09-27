using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class TableViewContoller : UITableViewController
    {
        private WebViewController webViewController;
        private static UIViewController currentViewController;

        public TableViewContoller()
            : base()
        {
            webViewController = new WebViewController();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "iOS How Do I Samples";
            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            XDocument xDoc = XDocument.Load("AppData/SampleList-iOS.xml");
            List<CategoryNode> nodes = new List<CategoryNode>();

            if (xDoc.Root != null)
                nodes.AddRange(xDoc.Root.Elements().Select(element => new CategoryNode(element)));

            TableSource tableSource = new TableSource(nodes);
            tableSource.TableSubNodeSelected += TableSubNodeSelected;
            TableView.Source = tableSource;
        }

        private void TableSubNodeSelected(object sender, SampleNode node)
        {
            if (node.Disabled) return;

            UIBarButtonItem sampleSourceItem = new UIBarButtonItem("View Source", UIBarButtonItemStyle.Plain, (s, e) =>
            {
                webViewController.SampleName = node.ClassName;
                NavigationController.PushViewController(webViewController, true);
            });

            if (currentViewController != null)
            {
                foreach (UIView sampleView in currentViewController.View.Subviews)
                {
                    if (sampleView != null) sampleView.Dispose();
                }
                currentViewController.Dispose();
                GC.SuppressFinalize(currentViewController);
            }

            UIViewController viewController = node.ClassInstance;
            viewController.NavigationItem.SetRightBarButtonItem(sampleSourceItem, true);
            viewController.Title = node.Name;
            currentViewController = viewController;
            NavigationController.PushViewController(currentViewController, true);
        }
    }
}