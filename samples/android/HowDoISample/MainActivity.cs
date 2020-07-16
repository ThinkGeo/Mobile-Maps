using Android;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ThinkGeo.UI.Android.HowDoI
{
    [Activity(
        Label = "@string/app_name",
        Theme = "@android:style/Theme.Light.NoTitleBar",
        MainLauncher = true,
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private TextView uploadTextView;
        private ProgressBar uploadProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);

            uploadTextView = FindViewById<TextView>(Resource.Id.uploadDataTextView);
            uploadProgressBar = FindViewById<ProgressBar>(Resource.Id.uploadProgressBar);

            Task.Factory.StartNew(() => CopyAssets(this.Assets, "AppData")).ContinueWith(t =>
            {
                uploadTextView.Post(() =>
                {
                    uploadTextView.Text = "Ready";
                    uploadProgressBar.Progress = 100;
                });

                uploadTextView.PostDelayed(() =>
                {
                    StartActivity(typeof(NavigationDrawerActivity));
                    Finish();
                }, 200);
            });
        }

        private void CopyAssets(AssetManager assetManager, string sourceDir)
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
                    sourceStream.CopyTo(targetStream);
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

