using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    internal class EarthquakeRow
    {
        public EarthquakeRow()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        { }

        private EarthquakeRow(string yearValue, string longitudeValue, string latitudeValue, string depthValue, string magnitudeValue, string locationValue)
        {
            YearValue = yearValue;
            LongitudeValue = longitudeValue;
            LatitudeValue = latitudeValue;
            DepthValue = depthValue;
            MagnitudeValue = magnitudeValue;
            LocationValue = locationValue;
        }

        public string YearValue { get; set; }

        public string LongitudeValue { get; set; }

        public string LatitudeValue { get; set; }

        public string DepthValue { get; set; }

        public string MagnitudeValue { get; set; }

        public string LocationValue { get; set; }

        public UIImageView AccessoryView { get; set; }

        public override string ToString()
        {
            return string.Format("Year: {0}. At: Lon: {1}, Lat: {2}. Depth: {3}. Magnitude: {4}.", YearValue,
                LongitudeValue ?? string.Empty, LatitudeValue ?? string.Empty, DepthValue ?? string.Empty, MagnitudeValue ?? string.Empty);
        }
    }
}