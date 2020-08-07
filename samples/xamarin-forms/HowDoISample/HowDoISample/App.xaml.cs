using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.UI.Xamarin.HowDoI;
using System.Diagnostics;
using HowDoISample.Views;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace HowDoISample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "Expander_Experimental", "RadioButton_Experimental" });
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
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                // HowDoISample.Data.Shapefile.City_ETJ.shp
                // HowDoISample.Data.FileGeoDatabase.zoning.gdb.a00000003.gdbindexes
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                List<string> filesWithoutExtenstions = new List<string>() { "gdb", "timestamps" };

                string[] parts = resourceName.Replace("HowDoISample.", "").Split('.');

                string localPath = "";
                for (int i = 0; i < parts.Length; i++)
                {
                    // Default delimiter to '/' for the directory structure
                    var delimiter = "/";

                    // Use '.' delimiter for file extensions and for any FileGeoDatabase directory names and any filenames within FileGeoDatabase directory contents
                    if ((i == parts.Length - 1 && !filesWithoutExtenstions.Contains(parts[i])) // files with extensions
                        || localPath.EndsWith("FileGeoDatabase/zoning") // the FileGeoDatabase zoning.gdb directory name
                        || localPath.Contains("zoning.gdb/")) // any files within zoning.gdb directory
                    { delimiter = "."; }

                    // Don't use a delimiter for the first part
                    if (i == 0) delimiter = "";

                    localPath += $"{delimiter}{parts[i]}";
                }

                string targetFilePath = Path.Combine(appDataPath, localPath);
                string targetDir = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                if (!File.Exists(targetFilePath))
                {
                    using (var targetStream = File.Create(targetFilePath))
                    {
                        Stream sourceStream = assembly.GetManifestResourceStream(resourceName);
                        await sourceStream.CopyToAsync(targetStream);
                        sourceStream.Close();
                    }

                    Debug.WriteLine($"<<<<< Copying embedded resource to {targetFilePath} >>>>>");
                }
            }
        }
    }
}
