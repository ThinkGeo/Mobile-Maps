using System;

namespace MapSuiteEarthquakeStatistics
{
    public class RangeChangedEventArgs : EventArgs
    {
        public nfloat LowerValue { get; set; }

        public nfloat UpperValue { get; set; }

        public RangeChangedEventArgs(nfloat lowerValue, nfloat upperValue)
        {
            LowerValue = lowerValue;
            UpperValue = upperValue;
        }
    }
}