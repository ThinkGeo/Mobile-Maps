using UIKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteEarthquakeStatistics
{
    internal class EarthquakeToolBar : Dictionary<string, UIBarButtonItem>
    {
        private static EarthquakeToolBar instance;

        public event EventHandler<EventArgs> ToolBarButtonClick;

        public static EarthquakeToolBar Instance
        {
            get { return instance ?? (instance = new EarthquakeToolBar()); }
        }

        private EarthquakeToolBar()
        {
            // Tool bar buttons
            AddBarItem(EarthquakeConstant.Cursor, "pan", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Polygon, "polygon", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Rectangle, "rectangle", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Clear, "recycle", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Search, "search", OnToolBarButtonClick);
            AddBarItem(EarthquakeConstant.Options, "settings", OnToolBarButtonClick);

            this[EarthquakeConstant.FlexibleSpace] = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        }

        public IEnumerable<UIBarButtonItem> GetToolBarItems()
        {
            Collection<UIBarButtonItem> toolBarItems = new Collection<UIBarButtonItem>
            {
                Instance[EarthquakeConstant.Cursor],
                Instance[EarthquakeConstant.Polygon],
                Instance[EarthquakeConstant.Rectangle],
                Instance[EarthquakeConstant.Clear],
                Instance[EarthquakeConstant.FlexibleSpace],
                Instance[EarthquakeConstant.Search],
                Instance[EarthquakeConstant.Options]
            };

            return toolBarItems;
        }

        private void AddBarItem(string title, string iconPath, EventHandler handler)
        {
            UIBarButtonItem item = new UIBarButtonItem(UIImage.FromBundle(iconPath), UIBarButtonItemStyle.Bordered, handler)
            {
                Title = title
            };
            this[title] = item;
        }

        private void OnToolBarButtonClick(object sender, EventArgs e)
        {
            ToolBarButtonClick?.Invoke(sender, e);
        }
    }
}