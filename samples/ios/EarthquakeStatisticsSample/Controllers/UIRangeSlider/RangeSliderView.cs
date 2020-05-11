using CoreGraphics;
using System.Drawing;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public class RangeSliderView : UIView
    {
        private UILabel name;
        private UILabel lowerValue;
        private UILabel upperValue;

        public RangeSliderView(UIRangeSlider rangeSlider)
        {
            rangeSlider.RangeChanged += RangeSlider_RangeChanged;
            name = new UILabel(new RectangleF(0, 0, 100, 40));
            lowerValue = new UILabel(new CGRect(100, 0, 50, 40));
            upperValue = new UILabel(new CGRect(150 + rangeSlider.Frame.Width + 10, 0, 50, 40));

            name.Text = rangeSlider.Name;
            lowerValue.Text = rangeSlider.LowerValue.ToString("N0");
            upperValue.Text = rangeSlider.UpperValue.ToString("N0");

            Add(name);
            Add(lowerValue);
            Add(rangeSlider);
            Add(upperValue);
        }

        private void RangeSlider_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            UIRangeSlider slider = (UIRangeSlider)sender;
            switch (slider.Name)
            {
                case "Magnitude:":
                    Global.QueryConfiguration.LowerMagnitude = (int)e.LowerValue;
                    Global.QueryConfiguration.UpperMagnitude = (int)e.UpperValue;
                    break;
                case "Depth(KM):":
                    Global.QueryConfiguration.LowerDepth = (int)e.LowerValue;
                    Global.QueryConfiguration.UpperDepth = (int)e.UpperValue;
                    break;
                case "Date(Year):":
                    Global.QueryConfiguration.LowerYear = (int)e.LowerValue;
                    Global.QueryConfiguration.UpperYear = (int)e.UpperValue;
                    break;
            }

            lowerValue.Text = e.LowerValue.ToString("N0");
            upperValue.Text = e.UpperValue.ToString("N0");
        }
    }
}