using System.Collections.Generic;

namespace HowDoISample.Models
{
    public class MenuGroup : List<SampleMenuItem>
    {
        public string Title { get; set; }

        public bool IsExpanded { get; set; }
    }
}