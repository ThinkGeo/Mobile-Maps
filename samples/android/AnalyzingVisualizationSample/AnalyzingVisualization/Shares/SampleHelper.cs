using System.IO;

namespace AnalyzingVisualization
{
    internal class SampleHelper
    {
        public static readonly string AssetsDataDictionary = @"AppData";
        public static readonly string SampleDataDictionary = @"mnt/sdcard/MapSuiteSampleData/AnalyzingVisualization";

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }
    }
}