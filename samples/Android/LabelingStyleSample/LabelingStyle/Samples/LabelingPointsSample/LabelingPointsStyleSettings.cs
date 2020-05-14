using ThinkGeo.MapSuite.Styles;

namespace LabelingStyle
{
    public class LabelingPointsStyleSettings : StyleSettings
    {
        public LabelingPointsStyleSettings()
        {
            Title = "Labeling Points Edit Settings";
            Placement = PointPlacement.UpperCenter;
            XOffset = "0";
            YOffset = "8";
        }

        public PointPlacement Placement { get; set; }
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