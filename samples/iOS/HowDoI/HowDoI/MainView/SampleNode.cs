using UIKit;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class SampleNode
    {
        private string name;
        private string className;
        private string description;
        private bool disabled;
        private bool displayPicture;
        private string information;

        public UIViewController ClassInstance
        {
            get { return Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType("ThinkGeo.UI.iOS.HowDoI." + className)) as UIViewController; }
        }

        public SampleNode(XElement sampleElement)
        {
            name = sampleElement.Attribute("Name").Value;
            className = sampleElement.Attribute("Class").Value;
            description = sampleElement.Element("Description").Value;

            if (sampleElement.Attribute("Disabled") != null)
                bool.TryParse(sampleElement.Attribute("Disabled").Value, out disabled);

            if (sampleElement.Attribute("DisplayPicture") != null)
                bool.TryParse(sampleElement.Attribute("DisplayPicture").Value, out displayPicture);

            if (sampleElement.Attribute("Information") != null)
                information = sampleElement.Attribute("Information").Value;
        }

        public string Name
        {
            get { return name; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        public bool DisplayPicture
        {
            get { return displayPicture; }
            set { displayPicture = value; }
        }

        public string Infomation
        {
            get { return information; }
        }
    }
}