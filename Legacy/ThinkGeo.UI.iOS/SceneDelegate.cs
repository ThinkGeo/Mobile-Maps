using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    // iOS 27 runs an app only in the scene lifecycle; the scene manifest in Info.plist names this class.
    [Register("SceneDelegate")]
    public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
    {
        [Export("window")]
        public UIWindow? Window { get; set; }

        [Export("scene:willConnectToSession:options:")]
        public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
        {
            if (scene is not UIWindowScene windowScene)
                return;

            Window = new UIWindow(windowScene);
            Window.RootViewController = new UINavigationController(new TableViewContoller());
            Window.MakeKeyAndVisible();
        }
    }
}
