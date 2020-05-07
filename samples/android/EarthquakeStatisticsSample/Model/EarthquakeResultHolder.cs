using Android.Widget;
using Java.Lang;

namespace MapSuiteEarthquakeStatistics
{
    internal class EarthquakeResultHolder : Object
    {
        public TextView YearValue { get; set; }
        public TextView LongitudeValue { get; set; }
        public TextView LatitudeValue { get; set; }
        public TextView DepthValue { get; set; }
        public TextView MagnitudeValue { get; set; }
        public TextView LocationValue { get; set; }
    }
}