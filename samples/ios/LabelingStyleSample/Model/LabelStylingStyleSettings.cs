using MonoTouch.Dialog;
using ThinkGeo.Core;

namespace LabelingStyle
{
    public class LabelStylingStyleSettings : StyleSettings
    {
        public LabelStylingStyleSettings()
        {
            Title = "Label Styling Settings";
            ApplyOutlineColor = true;
            ApplyBackgroundMask = true;
            LabelsOverlappingEachOther = false;
            GridSize = LabelGridSize.Large;
            DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            DrawingMarginPercentage = "256";
        }

        [Checkbox]
        [Caption("Apply Outline Color")]
        public bool ApplyOutlineColor;

        [Checkbox]
        [Caption("Apply Background Mask")]
        public bool ApplyBackgroundMask;

        [Checkbox]
        [Caption("Labels Overlapping Each Other")]
        public bool LabelsOverlappingEachOther;

        [Caption("Grid Size")]
        public LabelGridSize GridSize;

        [Caption("Duplicate Rule")]
        public LabelDuplicateRule DuplicateRule;

        [Entry("Drawing Margins")]
        [Caption("Drawing Margins")]
        public string DrawingMarginPercentage;

        public override void Sync()
        {
            ApplyOutlineColor = GetCheckBoxElementValue(0);
            ApplyBackgroundMask = GetCheckBoxElementValue(1);
            LabelsOverlappingEachOther = GetCheckBoxElementValue(2);
            GridSize = GetRadioElementValue<LabelGridSize>(3);
            DuplicateRule = GetRadioElementValue<LabelDuplicateRule>(4);
            DrawingMarginPercentage = GetEntryElementValue(5);
        }

        public double GetDrawingMarginPercentage()
        {
            return ParseToDouble(DrawingMarginPercentage, 256);
        }
    }

    public enum LabelGridSize
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }
}