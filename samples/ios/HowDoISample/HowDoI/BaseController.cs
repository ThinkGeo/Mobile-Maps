
using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
	public partial class BaseController : UIViewController
	{
		// provide access to the sidebar controller to all inheriting controllers
		protected SidebarNavigation.SidebarController SidebarController
		{
			get
			{
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.SidebarController;
			}
		}


		// provide access to the sidebar controller to all inheriting controllers
		protected NavController NavController
		{
			get
			{
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
			}
		}

		public BaseController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.NavigationItem.SetHidesBackButton(true, false);
			UIImage uiImage = UIImage.FromBundle("threelines");
		
			NavigationItem.SetLeftBarButtonItem(
				new UIBarButtonItem(uiImage
					, UIBarButtonItemStyle.Plain
					, (sender, args) => {
						SidebarController.ToggleMenu();
					}), true);
		}

		public void ShowToast(String message)
		{
			UIView residualView = this.View.ViewWithTag(1989);
			if (residualView != null)
				residualView.RemoveFromSuperview();

			var viewBack = new UIView(new CoreGraphics.CGRect(83, 0, 300, 100));
			viewBack.BackgroundColor = UIColor.Black;
			viewBack.Tag = 1989;
			UILabel lblMsg = new UILabel(new CoreGraphics.CGRect(0, 20, 300, 60));
			lblMsg.Lines = 2;
			lblMsg.Text = message;
			lblMsg.TextColor = UIColor.White;
			lblMsg.TextAlignment = UITextAlignment.Center;
			viewBack.Center = this.View.Center;
			viewBack.AddSubview(lblMsg);
			this.View.AddSubview(viewBack);
			//roundtheCorner(viewBack);
			UIView.BeginAnimations("Toast");
			UIView.SetAnimationDuration(3.0f);
			viewBack.Alpha = 0.0f;
			UIView.CommitAnimations();
		}

		public void ShowAlert(String message, string title)
		{
			var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			//Add Action
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			// Present Alert
			PresentViewController(okAlertController, true, null);
		}
	}
}

