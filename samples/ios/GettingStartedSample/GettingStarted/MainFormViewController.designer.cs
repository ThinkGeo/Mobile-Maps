// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace GettingStartedSample
{
    [Register ("MainFormViewController")]
    partial class MainFormViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ModalDailogCloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ModalDailogView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ModalOptionsTableView { get; set; }

        [Action ("ModalDailogCloseButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ModalDailogCloseButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ModalDailogCloseButton != null) {
                ModalDailogCloseButton.Dispose ();
                ModalDailogCloseButton = null;
            }

            if (ModalDailogView != null) {
                ModalDailogView.Dispose ();
                ModalDailogView = null;
            }

            if (ModalOptionsTableView != null) {
                ModalOptionsTableView.Dispose ();
                ModalOptionsTableView = null;
            }
        }
    }
}