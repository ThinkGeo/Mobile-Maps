using CoreGraphics;
using ThinkGeo.UI.iOS;
using ThinkGeo.Core;
using UIKit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the location popup which display the location information. 
    /// </summary>
    public class CurrentLocationPopup : Popup
    {
        private UILabel lblLocation;
        private string content;

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                lblLocation.Text = content;
            }
        }

        public CurrentLocationPopup(PointShape position, string content)
            : base(position)
        {
            UIView view = new UIView(new CGRect(0, 0, 200, 45));
            lblLocation = new UILabel(new CGRect(5, 5, 165, 40))
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 2,
                Font = UIFont.FromName("Arial", 16)
            };
            Content = content;

            UIButton popupClose = UIButton.FromType(UIButtonType.System);
            popupClose.Frame = new CGRect(170, 10, 30, 30);
            popupClose.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
            popupClose.TintColor = UIColor.Black;
            popupClose.Layer.CornerRadius = 15;
            popupClose.TouchUpInside += (s, e1) => Hidden = true;

            view.Add(lblLocation);
            view.Add(popupClose);
            ContentView.Add(view);
        }
    }
}