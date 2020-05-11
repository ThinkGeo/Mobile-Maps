using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    public class LabelingLinesStyleSettings : StyleSettings
    {
        public LabelingLinesStyleSettings()
        {
            Title = "Labeling Lines Edit Settings";
            SplineType = SplineType.Default;
            LineSegmentRatio = "0.9";
        }

        public SplineType SplineType { get; set; }
        public string LineSegmentRatio { get; set; }

        public double GetLineSegmentRatio()
        {
            return ParseToDouble(LineSegmentRatio, 0.9);
        }
    }
}
