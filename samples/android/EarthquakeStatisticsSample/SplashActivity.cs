using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapSuiteEarthquakeStatistics
{
    [Activity(Theme = "@android:style/Theme.Light.NoTitleBar", Icon = "@drawable/sampleIcon", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden |
        Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SplashActivity : Activity
    {
        private TextView uploadTextView;
        private ProgressBar uploadProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);

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

            updateSampleDatasTask.ContinueWith(t =>
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