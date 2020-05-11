using CoreGraphics;
using System;
using UIKit;

namespace DrawEditFeatures
{
    public class SampleUIHelper
    {
        private static nfloat fullHeight;
        private static nfloat minHeight = 44;
        private static UIView instructionContainer;

        public static bool IsOnIPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static UIView InstructionContainer { get; set; }

        private static int DescriptionMarginLeft
        {
            get { return IsOnIPhone ? 10 : 20; }
        }

        public static void InitializeInstruction(UIView containerView, Func<bool, float> getInstructionHeight, Action<UIView> getInstructionContent = null)
        {
            bool isIphone = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;
            float instructionHeight = getInstructionHeight(isIphone);
            UIView instructionView = new UIView(new CGRect(0, containerView.Frame.Bottom - instructionHeight, containerView.Frame.Width, instructionHeight));
            containerView.Add(instructionView);

            instructionContainer = instructionView;
            instructionContainer.Alpha = 0.9f;
            instructionContainer.BackgroundColor = UIColor.FromRGBA(126, 124, 129, 255);
            fullHeight = instructionView.Frame.Height + minHeight;

            UIButton collapseButton = UIButton.FromType(UIButtonType.RoundedRect);
            collapseButton.Frame = new CGRect(new CGPoint(instructionView.Frame.Width - 60, 10), new CGSize(60, 30));
            collapseButton.SetImage(UIImage.FromBundle("more"), UIControlState.Normal);
            collapseButton.TintColor = UIColor.White;
            collapseButton.TouchUpInside += CollapseButton_TouchDown;

            UILabel instructionLable = new UILabel
            {
                BackgroundColor = instructionView.BackgroundColor,
                Text = "Instruction",
                Font = UIFont.FromName("Helvetica-Bold", 20),
                TextColor = UIColor.White,
                ShadowColor = UIColor.Gray,
                ShadowOffset = new CGSize(1, 1),
                Center = new CGPoint(instructionView.Frame.Width / 2, 20),
                Frame = new CGRect(0, 0, instructionView.Frame.Width, 44),
                TextAlignment = UITextAlignment.Center
            };

            ConstForDevice constaint;
            if (IsOnIPhone) constaint = new ConstForIPhone(instructionLable, instructionView);
            else constaint = new ConstForIPad(instructionLable, instructionView);

            UIView leftLineView = new UIView(new CGRect(new CGPoint(constaint.DescriptionMargin, 25), new CGSize(constaint.ContainerCenterX - constaint.InstructionWidthHalf - constaint.DescriptionMargin * 2, 1)))
            {
                BackgroundColor = UIColor.FromRGB(240, 240, 240)
            };

            UIView rightLineView = new UIView(new CGRect(new CGPoint(constaint.ContainerCenterX + constaint.InstructionWidthHalf + DescriptionMarginLeft, 25), new CGSize(constaint.ContainerCenterX - constaint.DescriptionMargin - constaint.InstructionWidthHalf - 60, 1)))
            {
                BackgroundColor = UIColor.FromRGB(240, 240, 240)
            };

            UIButton titleView = new UIButton();
            titleView.Layer.BorderWidth = 0;
            titleView.Layer.BorderColor = UIColor.Clear.CGColor;
            titleView.BackgroundColor = UIColor.Clear;
            titleView.SetTitle(string.Empty, UIControlState.Normal);
            titleView.Frame = new CGRect(0, 0, instructionView.Frame.Width, 44);
            titleView.TouchUpInside += CollapseButton_TouchDown;
            titleView.Add(instructionLable);
            titleView.Add(leftLineView);
            titleView.Add(rightLineView);
            titleView.Add(collapseButton);

            UILabel descriptionLableView = new UILabel(new CGRect(DescriptionMarginLeft, 44, instructionView.Frame.Width - DescriptionMarginLeft * 2, 20))
            {
                Text = "Draw and Edit Shapes.",
                TextColor = UIColor.White,
                ShadowColor = UIColor.Gray,
                ShadowOffset = new CGSize(1, 1),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                PreferredMaxLayoutWidth = instructionView.Frame.Width - DescriptionMarginLeft * 2
            };
            descriptionLableView.SizeToFit();

            instructionView.Add(titleView);
            instructionView.Add(descriptionLableView);

            if (getInstructionContent != null)
            {
                nfloat contentViewLeft = isIphone ? 10 : 20;
                nfloat contentViewTop = descriptionLableView.Frame.Bottom + 10;
                UIView contentView = new UIView(new CGRect(contentViewLeft, contentViewTop, instructionView.Frame.Width - 2 * contentViewLeft, instructionView.Frame.Height - (contentViewTop)));
                instructionView.Add(contentView);
                getInstructionContent(contentView);
            }
        }

        private static void CollapseButton_TouchDown(object sender, EventArgs e)
        {
            nfloat barHeight = fullHeight - instructionContainer.Frame.Height;
            UIView.Animate(.2, () =>
            {
                instructionContainer.Frame = new CGRect(new CGPoint(0, instructionContainer.Frame.Bottom - barHeight), new CGSize(instructionContainer.Frame.Width, barHeight));
            });
        }
    }
}