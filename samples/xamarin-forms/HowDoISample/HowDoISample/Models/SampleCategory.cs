using System.Collections.Generic;

namespace HowDoISample.Models
{
    internal class SampleCategory
    {
        public string Title { get; set; }

        public List<SampleMenuItem> Children { get; set; }
    }
}