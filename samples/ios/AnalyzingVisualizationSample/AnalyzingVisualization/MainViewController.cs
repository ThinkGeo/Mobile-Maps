using MonoTouch.Dialog;
using UIKit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AnalyzingVisualization
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SliderViewController navigation = new SliderViewController();
            navigation.Position = FlyOutPosition.Left;
            navigation.View.Frame = UIScreen.MainScreen.Bounds;
            View.AddSubview(navigation.View);
            AddChildViewController(navigation);

            XDocument xDoc = XDocument.Load("AppData/SampleList.xml");
            Section section = new Section("Style List");
            if (xDoc.Root != null)
            {
                Collection<UIViewController> styleList = new Collection<UIViewController>();
                foreach (var element in xDoc.Root.Elements())
                {
                    string image = element.Attribute("Image").Value;
                    string className = element.Attribute("Class").Value;
                    string name = element.Attribute("Name").Value;

                    UIImage icon = UIImage.FromBundle(image);
                    BadgeElement iconItem = new BadgeElement(icon, name);
                    section.Add(iconItem);

                    DetailViewController rootController = (DetailViewController)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType("AnalyzingVisualization." + className));
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