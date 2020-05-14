using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace RoutingSample
{
    [Activity(Theme = "@android:style/Theme.Light.NoTitleBar", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
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
                Collection<string> unLoadDatas = DataManager.CollectUnloadDatas(DataManager.SampleDataDictionary, DataManager.AssetsDataDictionary);
                DataManager.UploadDataFiles(DataManager.SampleDataDictionary, unLoadDatas, OnCopyingSourceFile);

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
    }
}