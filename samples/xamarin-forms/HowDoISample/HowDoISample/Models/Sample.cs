using System;
using System.Collections.Generic;
using System.Text;

namespace HowDoISample.Models
{
    class Sample
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }
    }

    class Category
    {
        public string Title { get; set; }

        public List<SampleMenuItem> Children { get; set; }
    }
}
