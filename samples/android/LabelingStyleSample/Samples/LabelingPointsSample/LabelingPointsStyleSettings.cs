using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace LabelingStyle
{
    public class LabelingPointsStyleSettings : StyleSettings
    {
        public LabelingPointsStyleSettings()
        {
            Title = "Labeling Points Edit Settings";
            Placement = TextPlacement.Upper;
            XOffset = "0";
            YOffset = "8";
        }

        public TextPlacement Placement { get; set; }
        public string XOffset { get; set; }
        public string YOffset { get; set; }

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