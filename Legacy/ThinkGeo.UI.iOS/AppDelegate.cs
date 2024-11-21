using Foundation;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        private UIWindow window;
        private UINavigationController navigationController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            UIViewController mainController = new TableViewContoller();
            navigationController = new UINavigationController(mainController);
            window.RootViewController = navigationController;
            window.MakeKeyAndVisible();
            return true;
        }
    }
}