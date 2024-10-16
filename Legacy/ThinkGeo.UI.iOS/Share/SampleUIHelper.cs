using CoreGraphics;
using UIKit;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class SampleUIHelper
    {
        private static nfloat fullHeight;
        private static nfloat minHeight = 44;
        private static UIView instructionContainer;

        public static void InitializeInstruction(MapView mapView, UIView containerView, Type sampleType, Func<bool, float> getInstructionHeight, Action<UIView> getInstructionContent = null)
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
            collapseButton.SetImage(UIImage.FromBundle("More"), UIControlState.Normal);
            collapseButton.TintColor = UIColor.White;
            collapseButton.TouchUpInside += CollapseButton_TouchDown;

            UILabel instructionLable = new UILabel();
            instructionLable.BackgroundColor = instructionView.BackgroundColor;
            instructionLable.Text = "Instruction";
            instructionLable.Font = UIFont.FromName("Helvetica-Bold", 20);
            instructionLable.TextColor = UIColor.White;
            instructionLable.ShadowColor = UIColor.Gray;
            instructionLable.ShadowOffset = new CGSize(1, 1);
            instructionLable.Center = new CGPoint(instructionView.Frame.Width / 2, 20);
            instructionLable.Frame = new CGRect(0, 0, instructionView.Frame.Width, 44);
            instructionLable.TextAlignment = UITextAlignment.Center;

            ConstForDevice constaint = null;
            if (IsOnIPhone) constaint = new ConstForIPhone(instructionLable, instructionView);
            else constaint = new ConstForIPad(instructionLable, instructionView);

            UIView leftLineView = new UIView(new CGRect(new CGPoint(constaint.DescriptionMargin, 25), new CGSize(constaint.ContainerCenterX - constaint.InstructionWidthHalf - constaint.DescriptionMargin * 2, 1)));
            leftLineView.BackgroundColor = UIColor.FromRGB(240, 240, 240);

            UIView rightLineView = new UIView(new CGRect(new CGPoint(constaint.ContainerCenterX + constaint.InstructionWidthHalf + DescriptionMarginLeft, 25), new CGSize(constaint.ContainerCenterX - constaint.DescriptionMargin - constaint.InstructionWidthHalf - 60, 1)));
            rightLineView.BackgroundColor = UIColor.FromRGB(240, 240, 240);

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

            UILabel descriptionLableView = new UILabel(new CGRect(DescriptionMarginLeft, 44, instructionView.Frame.Width - DescriptionMarginLeft * 2, 20));
            descriptionLableView.Text = GetSampleDescription(sampleType.Name);
            descriptionLableView.TextColor = UIColor.White;
            descriptionLableView.ShadowColor = UIColor.Gray;
            descriptionLableView.ShadowOffset = new CGSize(1, 1);
            descriptionLableView.LineBreakMode = UILineBreakMode.WordWrap;
            descriptionLableView.Lines = 0;
            descriptionLableView.PreferredMaxLayoutWidth = instructionView.Frame.Width - DescriptionMarginLeft * 2;
            descriptionLableView.SizeToFit();

            instructionView.Add(titleView);
            instructionView.Add(descriptionLableView);

            if (getInstructionContent != null)
            {
                float contentViewLeft = isIphone ? 10 : 20;
                nfloat contentViewTop = descriptionLableView.Frame.Bottom + 10;
                UIView contentView = new UIView(new CGRect(contentViewLeft, contentViewTop, instructionView.Frame.Width - 2 * contentViewLeft, instructionView.Frame.Height - (contentViewTop)));
                instructionView.Add(contentView);
                getInstructionContent(contentView);
            }
        }

        public static bool IsOnIPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static UIView InstructionContainer
        {
            get { return SampleUIHelper.instructionContainer; }
            set { SampleUIHelper.instructionContainer = value; }
        }

        private static string GetSampleDescription(string typeName)
        {
            XDocument xDoc = XDocument.Load("AppData/SampleList-iOS.xml");
            string description = string.Empty;

            foreach (XElement element in xDoc.Root.Elements())
            {
                foreach (var child in element.Elements())
                {
                    if (typeName == child.Attribute("Class").Value)
                    {
                        description = child.Element("Description").Value;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(description)) break;
            }

            return description;
        }

        private static void CollapseButton_TouchDown(object sender, EventArgs e)
        {
            nfloat barHeight = fullHeight - instructionContainer.Frame.Height;
            UIView.Animate(.2, () =>
            {
                instructionContainer.Frame = new CGRect(new CGPoint(0, instructionContainer.Frame.Bottom - barHeight), new CGSize(instructionContainer.Frame.Width, barHeight));
            });
        }

        private static int DescriptionMarginLeft
        {
            get
            {
                return IsOnIPhone ? 10 : 20;
            }
        }

        private SizeF MeasureText(UILabel label)
        {
            return (SizeF)UIStringDrawing.StringSize(label.Text, label.Font);
        }

        private class ConstForDevice
        {
            public ConstForDevice(UILabel label, UIView container)
            {
                SetInstructionSize(label);
                SetContainerCenter(container);
            }

            public nfloat ContainerCenterX { get; set; }

            public nfloat ContainerCenterY { get; set; }

            public float DescriptionMargin { get; set; }

            public float InstructionWidth { get; set; }

            public float InstructionWidthHalf { get; set; }

            public float InstructionHeight { get; set; }

            public float InstructionHeightHalf { get; set; }

            private void SetInstructionSize(UILabel label)
            {
                SizeF size = (SizeF)UIStringDrawing.StringSize(label.Text, label.Font);
                InstructionWidth = size.Width;
                InstructionHeight = size.Height;
                InstructionWidthHalf = size.Width * .5f;
                InstructionHeightHalf = size.Height * .5f;
            }

            private void SetContainerCenter(UIView container)
            {
                ContainerCenterX = container.Center.X;
                ContainerCenterY = container.Center.Y;
            }
        }

        private class ConstForIPhone : ConstForDevice
        {
            public ConstForIPhone(UILabel label, UIView container)
                : base(label, container)
            {
                DescriptionMargin = 10;
            }
        }

        private class ConstForIPad : ConstForDevice
        {
            public ConstForIPad(UILabel label, UIView container)
                : base(label, container)
            {
                DescriptionMargin = 20;
            }
        }
    }
}