using MonoTouch.Dialog;
using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelingPointsStyleSettings : StyleSettings
    {
        public LabelingPointsStyleSettings()
        {
            Title = "Custom Labeling Edit Settings";
            Placement = PointPlacement.UpperCenter;
            XOffset = "0";
            YOffset = "8";
        }

        [Caption("Placement")]
        public PointPlacement Placement;

        [Entry("XOffset")]
        public string XOffset;

        [Entry("YOffset")]
        public string YOffset;

        public override void Sync()
        {
            Placement = GetRadioElementValue<PointPlacement>(0);
            XOffset = GetEntryElementValue(1);
            YOffset = GetEntryElementValue(2);
        }

        public float GetXOffset()
        {
            return ParseToFloat(XOffset, 0f);
        }

        public float GetYOffset()
        {
            return ParseToFloat(YOffset, 8f);
        }
    }
}