using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteEarthquakeStatistics
{
    internal class SectionModel
    {
        public string Title { get; set; }

        public Collection<RowModel> Rows { get; private set; }

        public float HeaderHeight { get; set; }

        public SectionModel(string title)
            : this(title, null)
        { }

        public SectionModel(string title, IEnumerable<RowModel> rows)
        {
            Title = title;
            Rows = new Collection<RowModel>();
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    Rows.Add(item);
                }
            }
        }
    }
}