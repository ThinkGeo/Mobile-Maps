using System;
using System.Drawing;
using UIKit;

namespace DrawEditFeatures
{
    internal class ConstForDevice
    {
        public nfloat ContainerCenterX { get; set; }

        public nfloat ContainerCenterY { get; set; }

        public nfloat DescriptionMargin { get; set; }

        public nfloat InstructionWidth { get; set; }

        public nfloat InstructionWidthHalf { get; set; }

        public nfloat InstructionHeight { get; set; }

        public nfloat InstructionHeightHalf { get; set; }

        public ConstForDevice(UILabel label, UIView container)
        {
            SetInstructionSize(label);
            SetContainerCenter(container);
        }

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
}