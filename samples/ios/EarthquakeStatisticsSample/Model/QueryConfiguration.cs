namespace MapSuiteEarthquakeStatistics
{
    public class QueryConfiguration
    {
        public int LowerMagnitude { get; set; }

        public int UpperMagnitude { get; set; }

        public int LowerDepth { get; set; }

        public int UpperDepth { get; set; }

        public int LowerYear { get; set; }

        public int UpperYear { get; set; }

        public QueryConfiguration()
        {
            LowerMagnitude = 0;
            UpperMagnitude = 12;
            LowerDepth = 0;
            UpperDepth = 300;
            LowerYear = 1568;
            UpperYear = 2010;
        }
    }
}