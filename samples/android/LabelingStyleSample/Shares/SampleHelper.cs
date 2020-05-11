using System.IO;

namespace LabelingStyle
{
    internal class SampleHelper
    {
        public readonly static string AssetsDataDictionary = @"SampleData";
        public readonly static string SampleDataDictionary = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData).ToString() + @"/MapSuiteSampleData/LabelingStyle";

        public static string GetDataPath(params string[] fileNames)
        {
            string[] paths = new string[fileNames.Length + 2];

            paths[0] = SampleDataDictionary;
            paths[1] = AssetsDataDictionary;
            for (int i = 0; i < fileNames.Length; i++)
            {
                paths[i + 2] = fileNames[i];
            }

            return Path.Combine(paths);
        }
    }
}