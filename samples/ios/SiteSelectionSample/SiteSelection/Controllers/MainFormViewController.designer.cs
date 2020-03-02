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
    [Register ("MainFormViewController")]
    partial class MainFormViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIToolbar OperationToolbar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView QueryResultTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView QueryResultView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (OperationToolbar != null) {
                OperationToolbar.Dispose ();
                OperationToolbar = null;
            }

            if (QueryResultTableView != null) {
                QueryResultTableView.Dispose ();
                QueryResultTableView = null;
            }

            if (QueryResultView != null) {
                QueryResultView.Dispose ();
                QueryResultView = null;
            }
        }
    }
}