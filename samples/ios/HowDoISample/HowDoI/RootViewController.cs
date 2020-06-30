using System;
using System.Drawing;
using Foundation;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
	public partial class RootViewController : UIViewController
	{
		// the sidebar controller for the app
		public SidebarNavigation.SidebarController SidebarController { get; private set; }

		// the navigation controller
		public NavController NavController { get; private set; }
		LabelWithBorder instructionDescription;

		public RootViewController() : base(null, null)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		
			// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController();

			// set the default sample
			NavController.PushViewController(new DisplayASimpleMapViewController(), false);
			SidebarController = new SidebarNavigation.SidebarController(this, NavController, new SideMenuController());
			SidebarController.MenuWidth = 2 * (int)(View.GetFrame().Width / 3);
			SidebarController.ReopenOnRotate = false;
			SidebarController.MenuLocation = SidebarNavigation.MenuLocations.Left;

			// InstructionView
			var instructionView = new UIView();
			instructionView.ViewForBaselineLayout.Frame = new CoreGraphics.CGRect(0, this.View.Frame.Size.Height - 80, this.View.Frame.Size.Width, 80);
			instructionView.BackgroundColor = UIColor.LightGray;
			this.View.AddSubview(instructionView);

            // Add Line
            var lineInstructionView = new UIView();
            lineInstructionView.ViewForBaselineLayout.Frame = new CoreGraphics.CGRect(0, instructionView.Frame.Size.Height - 70, this.View.Frame.Size.Width, 2);
            lineInstructionView.BackgroundColor = UIColor.White;
            instructionView.AddSubview(lineInstructionView);

            // Add title label
            var instructionTitle = new UILabel();
			instructionTitle.Text = " Instruction";
			instructionTitle.ViewForBaselineLayout.Frame = new CoreGraphics.CGRect((this.View.Frame.Size.Width / 2) - 45, instructionView.Frame.Size.Height - 78, 90, 20);
			instructionTitle.TextColor = UIColor.Black;
			instructionTitle.Font.WithSize(48);
			instructionTitle.BackgroundColor = UIColor.LightGray;
			instructionView.AddSubview(instructionTitle);

			// Add description label
			instructionDescription = new LabelWithBorder();
			instructionDescription.Text = "This sample shows how to display a simple map.";
			instructionDescription.ViewForBaselineLayout.Frame = new CoreGraphics.CGRect(0 , instructionView.Frame.Size.Height - 60, this.View.Frame.Size.Width, 0);
			instructionDescription.TextColor = UIColor.Black;
			instructionDescription.TextAlignment = UITextAlignment.Justified;
			instructionDescription.Font.WithSize(32);
			ChangeLabelHeigthWithText(instructionDescription);
			instructionView.AddSubview(instructionDescription);
			
		}



		void ChangeLabelHeigthWithText(UILabel label, float maxHeight = 60f)
		{
			float width = (float)label.Frame.Width;
			SizeF size = (SizeF)((NSString)label.Text).StringSize(label.Font, constrainedToSize: new SizeF(width, maxHeight),
					lineBreakMode: UILineBreakMode.WordWrap);
			var labelFrame = label.Frame;
			labelFrame.Size = new SizeF(width, size.Height);
			label.Frame = labelFrame;
		}

		public void UpdateInstructionDescription(string description)
		{
			instructionDescription.Text = description;
			ChangeLabelHeigthWithText(instructionDescription);
		}
	}

	public class LabelWithBorder: UILabel
	{
		private UIEdgeInsets EdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
		private UIEdgeInsets InverseEdgeInsets = new UIEdgeInsets(-5, -5, -5, -5);

		public LabelWithBorder() : base()
		{
		}

		public override CoreGraphics.CGRect TextRectForBounds(CoreGraphics.CGRect bounds, nint numberOfLines)
		{
			var textRect = base.TextRectForBounds(EdgeInsets.InsetRect(bounds), numberOfLines);
			return InverseEdgeInsets.InsetRect(textRect);
		}

		public override void DrawText(CoreGraphics.CGRect rect)
		{
			base.DrawText(EdgeInsets.InsetRect(rect));
		}
	}
}

