using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;


namespace ThinkGeo.UI.iOS.HowDoI
{
    public class SampleInfo
    {
        public SampleInfo(string name, string className, string description, List<SampleInfo> children)
        {
            this.Name = name;
            this.Children = children;
            this.Visible = true;
            this.ClassName = className;
            this.Description = description;
        }

        public SampleInfo(XElement sampleElement)
        {
            Visible = true;
            Children = new List<SampleInfo>();
            Name = sampleElement.Attribute("Name").Value;
            foreach (XElement element in sampleElement.Elements())
            {
                Children.Add(new SampleInfo(element.Attribute("Name").Value,
                                            element.Attribute("Class").Value,
                                            element.Element("Description").Value,
                                            null));
            }
        }

        public List<SampleInfo> Children { get; }

        public string Name { get; }

        public bool Visible { get; set; }

        public string ClassName { get; }

        public string Description { get; }

        public string FullClassName
        {
            get
            {
                return "ThinkGeo.UI.iOS.HowDoI." + ClassName + "ViewController";
            }
        }
    }
}
