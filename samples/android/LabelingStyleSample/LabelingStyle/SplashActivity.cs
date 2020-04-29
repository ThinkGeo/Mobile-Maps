using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabelingStyle
{
    [Activity(Theme = "@android:style/Theme.Light.NoTitleBar", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden |
        Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SplashActivity : Activity
    {
        private TextView uploadTextView;
        private ProgressBar uploadProgressBar;

        readonly string[] StoragePermissions =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        const int RequestStorageId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);

            TryCopySampleDataAsync();
        }

        async Task TryCopySampleDataAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await CopySampleDataAsync();
                return;
            }

            await GetStoragePermissionsAsync();
        }

        async Task GetStoragePermissionsAsync()
        {
            const string readPermission = Manifest.Permission.ReadExternalStorage;
            const string writePermission = Manifest.Permission.WriteExternalStorage;

            if (!(CheckSelfPermission(readPermission) == (int)Permission.Granted) || !(CheckSelfPermission(writePermission) == (int)Permission.Granted))
            {
                RequestPermissions(StoragePermissions, RequestStorageId);
            }
            else
            {
                await CopySampleDataAsync();
            }
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestStorageId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            await CopySampleDataAsync();
                        }
                    }
                    break;
            }
        }

        async Task CopySampleDataAsync()
        {
            uploadTextView = FindViewById<TextView>(Resource.Id.uploadDataTextView);
            uploadProgressBar = FindViewById<ProgressBar>(Resource.Id.uploadProgressBar);

            Task updateSampleDatasTask = Task.Factory.StartNew(() =>
            {
                Collection<string> unLoadDatas = CollectUnloadDatas(SampleHelper.SampleDataDictionary, SampleHelper.AssetsDataDictionary);
                UploadDataFiles(SampleHelper.SampleDataDictionary, unLoadDatas, OnCopyingSourceFile);

                uploadTextView.Post(() =>
                {
                    uploadTextView.Text = "Ready";
                    uploadProgressBar.Progress = 100;
                });
            });

            await updateSampleDatasTask.ContinueWith(t =>
            {
                uploadTextView.PostDelayed(() =>
                {
                    StartActivity(typeof(MainActivity));
                    Finish();
                }, 200);
            });
        }

        private void OnCopyingSourceFile(string targetPathFilename, int completeCount, int totalCount)
        {
            uploadTextView.Post(() =>
            {
                uploadTextView.Text = string.Format("Copying {0} ({1}/{2})", Path.GetFileName(targetPathFilename), completeCount, totalCount);
                uploadProgressBar.Progress = (int)(completeCount * 100f / totalCount);
            });
        }

        private Collection<string> CollectUnloadDatas(string targetDirectory, string sourceDirectory)
        {
            Collection<string> result = new Collection<string>();

            foreach (string filename in Assets.List(sourceDirectory))
            {
                string sourcePath = System.IO.Path.Combine(sourceDirectory, filename);
                string targetPath = System.IO.Path.Combine(targetDirectory, sourcePath);

                if (!string.IsNullOrEmpty(Path.GetExtension(sourcePath)) && !File.Exists(targetPath))
                {
                    result.Add(sourcePath);
                }
                else if (string.IsNullOrEmpty(Path.GetExtension(sourcePath)))
                {
                    foreach (string item in CollectUnloadDatas(targetDirectory, sourcePath))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private void UploadDataFiles(string targetDirectory, IEnumerable<string> sourcePathFilenames, Action<string, int, int> onCopyingSourceFile = null)
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
                    Stream sourceStream = Assets.Open(sourcePathFilename);
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