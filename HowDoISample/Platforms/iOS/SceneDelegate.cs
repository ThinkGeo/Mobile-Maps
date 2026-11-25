using Foundation;

namespace HowDoISample;

// iOS 27 runs an app only in the scene lifecycle; the scene manifest in Info.plist
// names this class. Without it the process launches, the App constructor runs and
// then nothing happens at all - no window, no page, a black screen and no log.
[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
}
