// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MapSuiteSiteSelection
{
    [Register ("MainOptionFormViewController")]
    partial class MainOptionFormViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView OptionsTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton QueryButton { get; set; }

        [Action ("CloseButtonTouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouchUpInside (UIKit.UIButton sender);

        [Action ("QueryButtonTouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void QueryButtonTouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (OptionsTableView != null) {
                OptionsTableView.Dispose ();
                OptionsTableView = null;
            }

            if (QueryButton != null) {
                QueryButton.Dispose ();
                QueryButton = null;
            }
        }
    }
}