using UIKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteSiteSelection
{
    class ToolbarCandidates : Dictionary<string, UIBarButtonItem>
    {
        private static ToolbarCandidates instance;

        public event EventHandler<EventArgs> ToolBarButtonClick;

        public static ToolbarCandidates Instance
        {
            get { return instance ?? (instance = new ToolbarCandidates()); }
        }

        private ToolbarCandidates()
        {
            AddBarItem(SiteSelectionConstant.Pan, "pan", OnToolBarButtonClick);
            AddBarItem(SiteSelectionConstant.Pin, "pin", OnToolBarButtonClick);
            AddBarItem(SiteSelectionConstant.Clear, "recycle", OnToolBarButtonClick);
            AddBarItem(SiteSelectionConstant.Search, "search", OnToolBarButtonClick);
            AddBarItem(SiteSelectionConstant.Options, "settings", OnToolBarButtonClick);

            this[SiteSelectionConstant.FlexibleSpace] = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        }

        public IEnumerable<UIBarButtonItem> GetToolBarItems()
        {
            Collection<UIBarButtonItem> toolBarItems = new Collection<UIBarButtonItem>();

            toolBarItems.Add(Instance[SiteSelectionConstant.Pan]);
            toolBarItems.Add(Instance[SiteSelectionConstant.Pin]);
            toolBarItems.Add(Instance[SiteSelectionConstant.Clear]);
            toolBarItems.Add(Instance[SiteSelectionConstant.FlexibleSpace]);
            toolBarItems.Add(Instance[SiteSelectionConstant.Search]);
            toolBarItems.Add(Instance[SiteSelectionConstant.Options]);

            return toolBarItems;
        }

        private void AddBarItem(string title, string iconPath, EventHandler handler)
        {
            UIBarButtonItem item = new UIBarButtonItem(UIImage.FromBundle(iconPath), UIBarButtonItemStyle.Plain, handler);
            item.Title = title;
            this[title] = item;
        }

        private void OnToolBarButtonClick(object sender, EventArgs e)
        {
            ToolBarButtonClick?.Invoke(sender, e);
        }
    }
}