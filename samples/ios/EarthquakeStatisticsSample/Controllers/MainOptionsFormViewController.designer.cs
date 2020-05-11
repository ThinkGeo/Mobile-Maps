// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MapSuiteEarthquakeStatistics
{
    [Register ("MainOptionsFormViewController")]
    partial class MainOptionsFormViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView BaseMapTypeTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton QueryButton { get; set; }

        [Action ("CloseButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("QueryButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void QueryButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BaseMapTypeTableView != null) {
                BaseMapTypeTableView.Dispose ();
                BaseMapTypeTableView = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (QueryButton != null) {
                QueryButton.Dispose ();
                QueryButton = null;
            }
        }
    }
}