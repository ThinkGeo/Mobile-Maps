// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ThinkGeoCloudVectorMapsSample_ForiOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBarItem customStyleTabBarItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBarItem darkStyleTabBarItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBarItem hybridStyleTabBarItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBarItem lightStyleTabBarItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBar tabBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (customStyleTabBarItem != null) {
                customStyleTabBarItem.Dispose ();
                customStyleTabBarItem = null;
            }

            if (darkStyleTabBarItem != null) {
                darkStyleTabBarItem.Dispose ();
                darkStyleTabBarItem = null;
            }

            if (hybridStyleTabBarItem != null) {
                hybridStyleTabBarItem.Dispose ();
                hybridStyleTabBarItem = null;
            }

            if (lightStyleTabBarItem != null) {
                lightStyleTabBarItem.Dispose ();
                lightStyleTabBarItem = null;
            }

            if (tabBar != null) {
                tabBar.Dispose ();
                tabBar = null;
            }
        }
    }
}