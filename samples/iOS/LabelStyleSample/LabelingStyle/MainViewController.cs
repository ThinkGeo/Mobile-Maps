using MonoTouch.Dialog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UIKit;

namespace LabelingStyle
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

            XDocument xDoc = XDocument.Load("AppData/StyleList.xml");
            Section section = new Section("Layer List");
            if (xDoc.Root != null)
            {
                Collection<UIViewController> styleList = new Collection<UIViewController>();
                foreach (var element in xDoc.Root.Elements())
                {
                    string image = element.Attribute("Image").Value;
                    string className = element.Attribute("Class").Value;
                    string name = element.Attribute("Name").Value;

                    section.Add(new BadgeElement(UIImage.FromBundle(image), name));

                    DetailViewController rootController = (DetailViewController)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType("LabelingStyle." + className), navigation);
                    rootController.Title = name;
                    UINavigationController mainController = new UINavigationController(rootController);
                    styleList.Add(mainController);
                }
                navigation.ViewControllers = styleList.ToArray();
            }

            navigation.NavigationRoot = new RootElement("Layer List") { section };
        }
    }
}