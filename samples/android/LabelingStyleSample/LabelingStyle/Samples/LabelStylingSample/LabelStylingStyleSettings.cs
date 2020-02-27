
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelStylingStyleSettings : StyleSettings
    {
        public LabelStylingStyleSettings()
        {
            Title = "Label Styling Edit Settings";
            ApplyOutlineColor = true;
            ApplyBackgroundMask = true;
            LabelsOverlappingEachOther = false;
            GridSize = LabelGridSize.Large;
            DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            DrawingMarginPercentage = "256";
        }

        public bool ApplyOutlineColor { get; set; }
        public bool ApplyBackgroundMask { get; set; }
        public bool LabelsOverlappingEachOther { get; set; }
        public LabelGridSize GridSize { get; set; }
        public LabelDuplicateRule DuplicateRule { get; set; }
        public string DrawingMarginPercentage { get; set; }

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