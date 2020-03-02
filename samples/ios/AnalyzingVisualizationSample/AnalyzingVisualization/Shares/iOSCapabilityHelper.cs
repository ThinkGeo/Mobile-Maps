using UIKit;

namespace AnalyzingVisualization
{
    internal static class iOSCapabilityHelper
    {
        public static bool IsOnIPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }
    }
}