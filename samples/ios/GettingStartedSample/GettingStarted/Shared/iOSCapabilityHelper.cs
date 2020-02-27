using CoreGraphics;
using CoreLocation;
using System;
using UIKit;

namespace GettingStartedSample
{
    internal static class IOSCapabilityHelper
    {
        private static Version currentSystemVersion;

        public static Version CurrentSystemVersion
        {
            get
            {
                return currentSystemVersion ?? (currentSystemVersion = Version.Parse(UIDevice.CurrentDevice.SystemVersion));
            }
        }

        public static bool IsOnIPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static void SetViewLayout(UIView mainView)
        {
            if (CurrentSystemVersion.Major < 8)
            {
                UIInterfaceOrientation statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                if (statusBarOrientation == UIInterfaceOrientation.LandscapeLeft || statusBarOrientation == UIInterfaceOrientation.LandscapeRight)
                {
                    mainView.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width);
                }
                else
                {
                    mainView.Frame = UIScreen.MainScreen.Bounds;
                }
            }
        }

        public static void SetGpsLocationManagerCapability(CLLocationManager locationManager)
        {
            if (CurrentSystemVersion.Major >= 8)
            {
                // Add "NSLocationWhenInUseUsageDescription" and "NSLocationAlwaysUsageDescription" key into plist file.
                locationManager.RequestAlwaysAuthorization();
                locationManager.RequestWhenInUseAuthorization();
            }
        }
    }
}