using System.Diagnostics;
using System.Reflection;
using ThinkGeo.UI.Maui;

namespace HowDoISample;
public partial class App
{
    public App()
    {
        InitializeComponent();
        MainPage = new LoadingPage();
    }

    protected override async void OnStart()
    {
        await LicenseLoader.LoadLicense("thinkgeo.howdoi.maui.android.mapsuitelicense");
        await LicenseLoader.LoadLicense("thinkgeo.howdoi.maui.ios.mapsuitelicense");
        await CopySampleData(FileSystem.Current.AppDataDirectory);
        MainPage = new AppShell();
    }

    private static async Task CopySampleData(string targetFolder)
    {
        // Option 1(Current Way): Add the data files as embed resources, and copy them to App's Data Directory when first time running it. 
        //      Con: The data exists in two places
        //      Pro: Use the File based APIs, reference the files in xaml, make everything more straightforward with full optimization. 
        //
        // Option 2: Add the data file to Raw folder, set them as MauiAsset, and use FileSystem.OpenAppPackageFileAsync(filename) to load its stream. 
        //      Con: It's not working because: 
        //          1. the stream returned by FileSystem.OpenAppPackageFileAsync on Android is not seekable(it's seekable on Windows), meaning the method is not 100% supported across the platform at least now with .net 8.0 in December 2023.
        //          2. we need to use layer.streamLoading event which is not awaitable for now, some of the layer don't support streams. This issue can be fixed though. 
        //      Pro: Of course in this way the data is only kept in one place. 

        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var parts = resourceName.Replace("HowDoISample.", "").Split('.');

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