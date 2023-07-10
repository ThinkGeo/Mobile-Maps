using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HowDoISample.Views;
using ThinkGeo.UI.XamarinForms.HowDoI;

namespace HowDoISample
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LoadingPage();
        }

        protected override async void OnStart()
        {
            await CopyAssets();
            MainPage = new MainPage();
        }

        protected override void OnSleep()
        {}

        protected override void OnResume()
        {}

        private async Task CopyAssets()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filesWithoutExtensions = new List<string> { "gdb", "timestamps" };

            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var parts = resourceName.Replace("HowDoISample.", "").Split('.');

                var localPath = "";
                for (var i = 0; i < parts.Length; i++)
                {
                    // Default delimiter to '/' for the directory structure
                    var delimiter = "/";

                    // Use '.' delimiter for file extensions and for any FileGeoDatabase directory names and any file names within FileGeoDatabase directory contents
                    if (i == parts.Length - 1 && !filesWithoutExtensions.Contains(parts[i]) // files with extensions
                        || localPath.EndsWith("FileGeoDatabase/zoning") // the FileGeoDatabase zoning.gdb directory name
                        || localPath.Contains("zoning.gdb/")) // any files within zoning.gdb directory
                        delimiter = ".";

                    // Don't use a delimiter for the first part
                    if (i == 0) delimiter = "";

                    localPath += $"{delimiter}{parts[i]}";
                }

                var targetFilePath = Path.Combine(appDataPath, localPath);
                var targetDir = Path.GetDirectoryName(targetFilePath);

                if (targetDir == null)
                    return;

                if (!Directory.Exists(targetDir)) 
                    Directory.CreateDirectory(targetDir);

                if (File.Exists(targetFilePath)) continue;
                await using (var targetStream = File.Create(targetFilePath))
                {
                    var sourceStream = assembly.GetManifestResourceStream(resourceName);
                    if (sourceStream == null) 
                        continue;
                    await sourceStream.CopyToAsync(targetStream);
                    sourceStream.Close();
                }

                Debug.WriteLine($"<<<<< Copying embedded resource to {targetFilePath} >>>>>");
            }
        }
    }
}