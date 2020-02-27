using System;
using System.Linq;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace AnalyzingVisualization
{
    public class FilterStyleConditionModel : UIPickerViewModel
    {
        public Action<SimpleFilterConditionType> RowSelected;
        private SimpleFilterConditionType[] simpleFilterConditionTypes;

        public FilterStyleConditionModel()
        {
            simpleFilterConditionTypes = Enum.GetValues(typeof(SimpleFilterConditionType))
                .OfType<SimpleFilterConditionType>().ToArray();
        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Enum.GetValues(typeof(SimpleFilterConditionType)).Length;
        }

        public override UIView GetView(UIPickerView picker, nint row, nint component, UIView view)
        {
            UILabel lable = new UILabel();
            lable.Text = simpleFilterConditionTypes[(int)row].ToString();
            lable.TextColor = UIColor.White;
            if (iOSCapabilityHelper.IsOnIPhone) lable.Font = UIFont.FromName("Arial", 13);
            return lable;
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            RowSelected?.Invoke(simpleFilterConditionTypes[(int)row]);
        }
    }
}
