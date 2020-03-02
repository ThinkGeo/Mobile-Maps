namespace LabelingStyle
{
    public class CustomLabelingStyleSettings : StyleSettings
    {
        public CustomLabelingStyleSettings()
        {
            Title = "Custom Labeling Edit Settings";
            MinFontSize = "8";
            MaxFontSize = "60";
        }

        public string MinFontSize { get; set; }
        public string MaxFontSize { get; set; }

        public float GetMinFontSize()
        {
            return ParseToFloat(MinFontSize, 8);
        }

        public float GetMaxFontSize()
        {
            return ParseToFloat(MaxFontSize, 60);
        }
    }
}