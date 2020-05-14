using UIKit;

namespace DrawEditFeatures
{
    internal class ConstForIPhone : ConstForDevice
    {
        public ConstForIPhone(UILabel label, UIView container)
            : base(label, container)
        {
            DescriptionMargin = 10;
        }
    }
}