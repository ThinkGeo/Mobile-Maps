using System.Collections.Generic;

namespace HowDoISample.Models
{
    internal class Sample
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }
    }

    internal class SampleCategory
    {
        public string Title { get; set; }

        public List<SampleMenuItem> Children { get; set; }
    }
}