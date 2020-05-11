using System.IO;

namespace GeometricFunctions
{
    internal class SampleHelper
    {
        public static readonly string AssetsDataDictionary = @"AppData";
        public static readonly string SampleDataDictionary = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData).ToString() + @"MapSuiteSampleData/GeometricFunctions";

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }
    }
}