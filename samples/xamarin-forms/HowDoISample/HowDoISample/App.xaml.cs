using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HowDoISample.Views;
using ThinkGeo.UI.XamarinForms.HowDoI;
using Xamarin.Forms;

namespace HowDoISample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Device.SetFlags(new string[] { "Expander_Experimental", "RadioButton_Experimental" });
            MainPage = new LoadingPage();
        }

        protected override async void OnStart()
        {
            await CopyAssets();

            MainPage = new MainPage();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async Task CopyAssets()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                // HowDoISample.Data.Shapefile.City_ETJ.shp
                // HowDoISample.Data.FileGeoDatabase.zoning.gdb.a00000003.gdbindexes
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filesWithoutExtenstions = new List<string> {"gdb", "timestamps"};

                var parts = resourceName.Replace("HowDoISample.", "").Split('.');

                var localPath = "";
                for (var i = 0; i < parts.Length; i++)
                {
                    // Default delimiter to '/' for the directory structure
                    var delimiter = "/";

                    // Use '.' delimiter for file extensions and for any FileGeoDatabase directory names and any filenames within FileGeoDatabase directory contents
                    if (i == parts.Length - 1 && !filesWithoutExtenstions.Contains(parts[i]) // files with extensions
                        || localPath.EndsWith("FileGeoDatabase/zoning") // the FileGeoDatabase zoning.gdb directory name
                        || localPath.Contains("zoning.gdb/")) // any files within zoning.gdb directory
                        delimiter = ".";

                    // Don't use a delimiter for the first part
                    if (i == 0) delimiter = "";

                    localPath += $"{delimiter}{parts[i]}";
                }

                var targetFilePath = Path.Combine(appDataPath, localPath);
                var targetDir = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                if (!File.Exists(targetFilePath))
                {
                    using (var targetStream = File.Create(targetFilePath))
                    {
                        var sourceStream = assembly.GetManifestResourceStream(resourceName);
                        await sourceStream.CopyToAsync(targetStream);
                        sourceStream.Close();
                    }

                    Debug.WriteLine($"<<<<< Copying embedded resource to {targetFilePath} >>>>>");
                }
            }
        }
    }
}