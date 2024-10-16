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
            var assembly = Assembly.GetExecutingAssembly();
            List<string> dataFiles = new List<string>
            {
                @"WorldCapitals.dbf",
                @"WorldCapitals.ids",
                @"WorldCapitals.idx",
                @"WorldCapitals.shp",
                @"WorldCapitals.shx"
            };
            foreach (var dataFile in dataFiles)
            {
                var localPath = @"AppData/" + dataFile;
                var targetFilePath = Path.Combine(targetFolder, localPath);
                if (File.Exists(targetFilePath)) continue;

                var targetDir = Path.GetDirectoryName(targetFilePath);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                await using (var targetStream = File.Create(targetFilePath))
                {
                    var sourceStream = assembly.GetManifestResourceStream(@"MauiSample.AppData." + dataFile);
                    await sourceStream.CopyToAsync(targetStream);
                    sourceStream.Close();
                }
            }
        }

    }
}
