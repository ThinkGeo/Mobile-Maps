using Android.App;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace RoutingSample
{
    public static class DataManager
    {
        public readonly static string AssetsDataDictionary = @"AppData";
        public readonly static string SampleDataDictionary = @"/mnt/sdcard/Android.Sample/RoutingSample/";

        public static void CopySampleData()
        {
            if (!Directory.Exists(SampleDataDictionary))
            {
                Directory.CreateDirectory(SampleDataDictionary);
                foreach (string filename in Application.Context.Assets.List(AssetsDataDictionary))
                {
                    Stream stream = Application.Context.Assets.Open(Path.Combine(AssetsDataDictionary, filename));
                    FileStream fileStream = File.Create(Path.Combine(SampleDataDictionary, filename));
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    stream.Close();
                }
            }
        }

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }

        public static void UploadDataFiles(string targetDirectory, IEnumerable<string> sourcePathFilenames, Action<string, int, int> onCopyingSourceFile = null)
        {
            int completeCount = 0;
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

            foreach (string sourcePathFilename in sourcePathFilenames)
            {
                string targetPathFilename = Path.Combine(targetDirectory, sourcePathFilename);
                if (!File.Exists(targetPathFilename))
                {
                    if (onCopyingSourceFile != null) onCopyingSourceFile(targetPathFilename, completeCount, sourcePathFilenames.Count());

                    string targetPath = Path.GetDirectoryName(targetPathFilename);
                    if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                    Stream sourceStream = Application.Context.Assets.Open(sourcePathFilename);
                    FileStream fileStream = File.Create(targetPathFilename);
                    sourceStream.CopyTo(fileStream);
                    fileStream.Close();
                    sourceStream.Close();

                    completeCount++;
                }
            }
        }

        public static Collection<string> CollectUnloadDatas(string targetDirectory, string sourceDirectory)
        {
            Collection<string> result = new Collection<string>();

            foreach (string filename in Application.Context.Assets.List(sourceDirectory))
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
                    foreach (string item in CollectUnloadDatas(targetDirectory, sourcePath))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }
    }
}