using CoreGraphics;
using System;
using UIKit;

namespace AnalyzingVisualization
{
    internal class InstructionView : UIView
    {
        private Action InstructionViewTopChanged;
        private Action<UIView> ResetChildrenLayout;

        public InstructionView(Action<UIView> initializeChildrenContent, Action instructionViewTopChanged)
        {
            ResetChildrenLayout = initializeChildrenContent;
            InstructionViewTopChanged = instructionViewTopChanged;

            InitializeComponentLayout();
        }

        private static int DescriptionMarginLeft
        {
            get { return iOSCapabilityHelper.IsOnIPhone ? 10 : 20; }
        }

        private void CollapseButtonTouchDown(object sender, EventArgs e)
        {
            if (InstructionViewTopChanged != null) InstructionViewTopChanged();
        }

        private void InitializeComponentLayout()
        {
            float titleHeight = 40;

            Alpha = 0.9f;
            BackgroundColor = UIColor.FromRGBA(126, 124, 129, 255);

            UIView topLine = new UIView(new CGRect(0, 0, Frame.Width, 2));
            topLine.BackgroundColor = UIColor.DarkGray;
            topLine.Layer.CornerRadius = 4;
            topLine.Layer.ShadowRadius = 3;
            topLine.Layer.ShadowOpacity = .8f;
            topLine.Layer.ShadowColor = UIColor.Black.CGColor;
            topLine.Layer.ShadowOffset = new CGSize(1, 0);
            topLine.TranslatesAutoresizingMaskIntoConstraints = false;
            Add(topLine);

            UIButton titleView = new UIButton();
            titleView.BackgroundColor = UIColor.FromRGBA(126, 124, 129, 255);
            titleView.TouchUpInside += CollapseButtonTouchDown;
            titleView.TranslatesAutoresizingMaskIntoConstraints = false;
            Add(titleView);

            UILabel instructionLable = new UILabel();
            instructionLable.BackgroundColor = BackgroundColor;
            instructionLable.Text = "Setting";
            instructionLable.Font = UIFont.FromName("Helvetica-Bold", 20);
            instructionLable.TextColor = UIColor.White;
            instructionLable.ShadowColor = UIColor.Gray;
            instructionLable.ShadowOffset = new CGSize(1, 1);
            instructionLable.TextAlignment = UITextAlignment.Left;
            instructionLable.TranslatesAutoresizingMaskIntoConstraints = false;
            titleView.Add(instructionLable);

            UIButton collapseButton = UIButton.FromType(UIButtonType.RoundedRect);
            collapseButton.SetImage(UIImage.FromBundle("dotdotdot"), UIControlState.Normal);
            collapseButton.TintColor = UIColor.White;
            collapseButton.TouchUpInside += CollapseButtonTouchDown;
            collapseButton.TranslatesAutoresizingMaskIntoConstraints = false;
            titleView.Add(collapseButton);

            float contentViewTop = titleHeight + 5;
            UIView contentView = new UIView();
            contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            Add(contentView);

            NSLayoutConstraint[] instructionViewConstraints = new[]
            {
                NSLayoutConstraint.Create(topLine, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, 0),
                NSLayoutConstraint.Create(topLine, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(topLine, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(topLine, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 2),
                
                NSLayoutConstraint.Create(collapseButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 55),
                NSLayoutConstraint.Create(collapseButton, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 5),
                NSLayoutConstraint.Create(collapseButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 10),
                NSLayoutConstraint.Create(collapseButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30),

                NSLayoutConstraint.Create(instructionLable, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, DescriptionMarginLeft),
                NSLayoutConstraint.Create(instructionLable, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(instructionLable, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(instructionLable, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, titleHeight),

                NSLayoutConstraint.Create(titleView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, 0),
                NSLayoutConstraint.Create(titleView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(titleView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(titleView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, titleHeight),

                NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, DescriptionMarginLeft),
                NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, DescriptionMarginLeft),
                NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, contentViewTop),
                NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, 0),
            };
            AddConstraints(instructionViewConstraints);

            ResetChildrenLayout?.Invoke(contentView);
        }
    }
}