using UIKit;

namespace DrawEditFeatures
{
    internal class ConstForIPad : ConstForDevice
    {
        public ConstForIPad(UILabel label, UIView container)
            : base(label, container)
        {
            DescriptionMargin = 20;
        }
    }
}