namespace LabelingStyle
{
    public class LabelingPolygonsStyleSettings : StyleSettings
    {
        public LabelingPolygonsStyleSettings()
        {
            Title = "Labeling Polygons Edit Settings";
            FittingFactorsOnlyWithin = true;
            LabelAllPolygonParts = false;
        }

        public bool FittingFactorsOnlyWithin { get; set; }
        public bool LabelAllPolygonParts { get; set; }
    }
}