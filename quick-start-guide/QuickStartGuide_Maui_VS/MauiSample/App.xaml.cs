using System.Diagnostics;
using System.Reflection;
using ThinkGeo.UI.Maui;

namespace MauiSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            await LicenseLoader.LoadLicense("mauidemo.thinkgeo.com.android.mapsuitelicense");
            await LicenseLoader.LoadLicense("mauidemo.thinkgeo.com.ios.mapsuitelicense");
            await CopySampleData(FileSystem.Current.AppDataDirectory);
            MainPage = new AppShell();
        }

        private static async Task CopySampleData(string targetFolder)
        {
            // Option 1(Current Way): Add the data files as embed resources, and copy them to App's Data Directory when first time running it. 
            //      Con: The data exists in two places
            //      Pro: Use the File based APIs, reference the files in xaml, make everything more straightforward with full optimization. 

            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var parts = resourceName.Replace("MauiSample.", "").Split('.');

                var localPath = parts[0];
                for (var i = 1; i < parts.Length; i++)
                {
                    // Default delimiter to '/' for the directory structure
                    var delimiter = "/";
                    if (i == parts.Length - 1)
                        delimiter = ".";
                    // Don't use a delimiter for the first part
                    localPath += $"{delimiter}{parts[i]}";
                }

                var targetFilePath = Path.Combine(targetFolder, localPath);
                if (File.Exists(targetFilePath)) continue;

                var targetDir = Path.GetDirectoryName(targetFilePath);

                if (targetDir == null)
                    return;

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

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
