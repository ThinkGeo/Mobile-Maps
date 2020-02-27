using MonoTouch.Dialog;

namespace LabelingStyle
{
    public class LabelingPolygonsStyleSettings : StyleSettings
    {
        public LabelingPolygonsStyleSettings()
        {
            Title = "Labeling Polygons Edit Settings";
            FittingFactorsOnlyWithin = true;
        }

        [Checkbox]
        [Caption("Fitting Factors Only Within")]
        public bool FittingFactorsOnlyWithin;

        [Checkbox]
        [Caption("Label All Polygon Parts")]
        public bool LabelAllPolygonParts;

        public override void Sync()
        {
            FittingFactorsOnlyWithin = GetCheckBoxElementValue(0);
            LabelAllPolygonParts = GetCheckBoxElementValue(1);
        }
    }
}