using MonoTouch.Dialog;

namespace LabelingStyle
{
    public class CustomLabelingStyleSettings : StyleSettings
    {
        public CustomLabelingStyleSettings()
        {
            Title = "Custom Labeling Edit Settings";
            MinFontSize = "8";
            MaxFontSize = "25";
        }

        [Entry("Min Font Size")]
        public string MinFontSize;

        [Entry("Max Font Size")]
        public string MaxFontSize;

        public override void Sync()
        {
            MinFontSize = GetEntryElementValue(0);
            MaxFontSize = GetEntryElementValue(1);
        }

        public float GetMinFontSize()
        {
            return ParseToFloat(MinFontSize, 8);
        }

        public float GetMaxFontSize()
        {
            return ParseToFloat(MaxFontSize, 25);
        }
    }
}