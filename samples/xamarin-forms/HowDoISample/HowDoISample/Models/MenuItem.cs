using System;
using System.Collections.Generic;
using System.Text;

namespace HowDoISample.Models
{
    class SampleMenuItem
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }

    class MenuGroup : List<SampleMenuItem>
    {
        public string Title { get; set; }

        public bool IsExpanded { get; set; }
    }
}
