using System.IO;

namespace MapSuiteEarthquakeStatistics
{
    internal class SampleHelper
    {
        public readonly static string AssetsDataDictionary = @"SampleData";
        public readonly static string SampleDataDictionary = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData).ToString() + @"MapSuiteSampleData/EarthquakeStatistics";

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }
    }
}