using System.Collections.Generic;

namespace HowDoISample.Models
{
    public class SampleMenuItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }

    public class MenuGroup : List<SampleMenuItem>
    {
        public string Title { get; set; }

        public bool IsExpanded { get; set; }
    }
}