using MonoTouch.Dialog;
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelingLinesStyleSettings : StyleSettings
    {
        public LabelingLinesStyleSettings()
        {
            Title = "Labeling Lines Edit Settings";
            SplineType = SplineType.ForceSplining;
            LineSegmentRatio = "0.9";
        }

        [Caption("Spline Type")]
        public SplineType SplineType;

        [Entry("Line Segment Ratio")]
        public string LineSegmentRatio;

        public override void Sync()
        {
            SplineType = GetRadioElementValue<SplineType>(0);
            LineSegmentRatio = GetEntryElementValue(1);
        }

        public double GetLineSegmentRatio()
        {
            return ParseToDouble(LineSegmentRatio, 0.9);
        }
    }
}