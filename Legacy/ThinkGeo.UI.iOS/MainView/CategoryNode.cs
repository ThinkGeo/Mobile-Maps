using System.Collections.Generic;
using System.Xml.Linq;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class CategoryNode
    {
        private string name;
        private bool expand;
        private List<SampleNode> child;

        public CategoryNode(XElement sampleElement)
        {
            name = sampleElement.Attribute("Name").Value;
            child = new List<SampleNode>();
            foreach (XElement element in sampleElement.Elements())
            {
                child.Add(new SampleNode(element));
            }
        }

        public string Name
        {
            get { return name; }
        }

        public bool Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        public List<SampleNode> Child
        {
            get { return child; }
        }
    }
}