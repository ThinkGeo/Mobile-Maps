using System.Collections.ObjectModel;

namespace GettingStartedSample
{
    public class SectionModel
    {
        public Collection<CellModel> Rows { get; private set; }

        public SectionModel()
        {
            Rows = new Collection<CellModel>();
        }
    }
}