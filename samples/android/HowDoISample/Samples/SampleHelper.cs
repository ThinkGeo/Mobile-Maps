using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class SampleHelper
    {
        // This ThinkGeo Cloud test key is exclusively for demonstration purposes and is limited to requesting base map 
        // tiles only. Do not use it in your own applications, as it may be restricted or disabled without prior notice. 
        // Please visit https://cloud.thinkgeo.com to create a free key for your own use.
        public static string ThinkGeoCloudId { get; } = "USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~";
        public static string ThinkGeoCloudSecret { get; } = "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~";
        public readonly static string AssetsDataDictionary = @"AppData";
        public readonly static string SampleDataDictionary = @"mnt/sdcard/MapSuiteSampleData/HowDoISamples";

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }

        public static Collection<string> CollectUnloadDatas(AssetManager assets, string targetDirectory, string sourceDirectory)
        {
            Collection<string> result = new Collection<string>();

            foreach (string filename in assets.List(sourceDirectory))
            {
                string sourcePath = System.IO.Path.Combine(sourceDirectory, filename);
                string targetPath = System.IO.Path.Combine(targetDirectory, sourcePath);

                bool isSourcePathAFile = !string.IsNullOrEmpty(Path.GetExtension(sourcePath));
                if (isSourcePathAFile && !File.Exists(targetPath))
                {
                    result.Add(sourcePath);
                }
                else if (!isSourcePathAFile)
                {
                    foreach (string item in CollectUnloadDatas(assets, targetDirectory, sourcePath))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public static void UploadDataFiles(AssetManager assets, string targetDirectory, IEnumerable<string> sourcePathFilenames, Action<string, int, int> onCopyingSourceFile = null)
        {
            int completeCount = 0;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            foreach (string sourcePathFilename in sourcePathFilenames)
            {
                string targetPathFilename = Path.Combine(targetDirectory, sourcePathFilename);
                if (!File.Exists(targetPathFilename))
                {
                    if (onCopyingSourceFile != null)
                    {
                        onCopyingSourceFile(targetPathFilename, completeCount, sourcePathFilenames.Count());
                    }

                    string targetPath = Path.GetDirectoryName(targetPathFilename);
                    if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                    Stream sourceStream = assets.Open(sourcePathFilename);
                    FileStream fileStream = File.Create(targetPathFilename);
                    sourceStream.CopyTo(fileStream);
                    fileStream.Close();
                    sourceStream.Close();

                    completeCount++;
                }
            }
        }
    }
}