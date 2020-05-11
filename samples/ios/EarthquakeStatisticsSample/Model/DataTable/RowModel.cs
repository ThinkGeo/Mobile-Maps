using System.Drawing;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    internal class RowModel
    {
        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public UIView CustomUI { get; set; }

        public RectangleF CustomUIBounds { get; set; }

        public float RowHeight { get; set; }

        public UIView AccessoryView { get; set; }

        public UITableViewCellAccessory CellAccessory { get; set; }

        public RowModel(string name)
            : this(name, null)
        { }

        public RowModel(string name, UIImageView accessoryView)
        {
            Name = name;
            AccessoryView = accessoryView;
        }
    }
}