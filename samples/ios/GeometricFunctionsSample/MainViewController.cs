using MonoTouch.Dialog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThinkGeo.Core;
using UIKit;

namespace GeometricFunctions
{
    public partial class MainViewController : UIViewController
    {
        private SliderViewController navigation;

        public MainViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            navigation = new SliderViewController();
            navigation.Position = FlyOutPosition.Left;
            navigation.View.Frame = UIScreen.MainScreen.Bounds;
            View.AddSubview(navigation.View);
            AddChildViewController(navigation);

            XDocument xDoc = XDocument.Load("AppData/GeometricFunctions.xml");
            Section section = new Section("Style List");
            if (xDoc.Root != null)
            {
                Collection<UIViewController> styleList = new Collection<UIViewController>();
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                foreach (var element in xDoc.Root.Elements())
                {
                    string name = element.Attribute("Name").Value;
                    string image = element.Attribute("Image").Value;
                    string className = element.Attribute("Class").Value;

                    UIImage icon = UIImage.FromBundle(image);
                    BadgeElement iconItem = new BadgeElement(icon, name);
                    section.Add(iconItem);

                    Type sampleType = currentAssembly.GetType("GeometricFunctions." + className);
                    DetailViewController rootController = (DetailViewController)Activator.CreateInstance(sampleType);

                    XElement geometrySourceElement = element.Element("GeometrySource");
                    foreach (var featureWktElement in geometrySourceElement.Elements())
                    {
                        if (!string.IsNullOrEmpty(featureWktElement.Value))
                        {
                            rootController.GeometrySource.Add(new Feature(featureWktElement.Value));
                        }
                    }

                    rootController.DetailButtonClick = navigation.ToggleMenu;
                    rootController.Title = name;
                    UINavigationController mainController = new UINavigationController(rootController);
                    styleList.Add(mainController);
                }
                navigation.ViewControllers = styleList.ToArray();
            }

            navigation.NavigationRoot = new RootElement("Style List") { section };
        }
    }
}