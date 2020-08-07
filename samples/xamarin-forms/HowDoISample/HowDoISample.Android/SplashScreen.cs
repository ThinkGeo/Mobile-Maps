using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace HowDoISample.Droid
{
    [Activity(Theme = "@style/MainTheme.Base", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private TextView uploadTextView;
        private ProgressBar uploadProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreenLayout);

            uploadTextView = FindViewById<TextView>(Resource.Id.uploadDataTextView);
            uploadProgressBar = FindViewById<ProgressBar>(Resource.Id.uploadProgressBar);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { Startup(); });
            startupWork.Start();
        }

        private async void Startup()
        {
            await CopyAssets(this.Assets, "AppData");

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        private async Task CopyAssets(AssetManager assetManager, string sourceDir)
        {
            var pendingAssets = GatherMissingData(assetManager, sourceDir);
            for (int i = 0; i < pendingAssets.Count; i++)
            {
                var asset = pendingAssets[i];
                var targetFilePath = Path.Combine(FileSystem.AppDataDirectory, asset);
                var targetDir = Path.GetDirectoryName(targetFilePath);

                OnCopyingFiles(Path.GetFileName(asset), i, pendingAssets.Count);

                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                using (var targetStream = File.Create(targetFilePath))
                {
                    var sourceStream = assetManager.Open(asset);
                    await sourceStream.CopyToAsync(targetStream);
                    sourceStream.Close();
                }
            }
        }

        private Collection<string> GatherMissingData(AssetManager assetManager, string sourceDir)
        {
            var paths = new Collection<string>();
            foreach (var asset in assetManager.List(sourceDir))
            {
                var path = Path.Combine(sourceDir, asset);
                if (assetManager.List(path).Length > 0)
                {
                    foreach (var subPath in GatherMissingData(assetManager, path))
                    {
                        paths.Add(subPath);
                    }
                }
                else if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, path)))
                {
                    paths.Add(path);
                }
            }

            return paths;
        }

        private void OnCopyingFiles(string fileName, int completeCount, int totalCount)
        {
            uploadTextView.Post(() =>
            {
                uploadTextView.Text = $"Copying {fileName} ({completeCount}/{totalCount})";
                uploadProgressBar.Progress = (int)(completeCount * 100f / totalCount);
            });
        }

    }
}