using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private const int RequestStorageId = 0;
        private readonly string[] StoragePermissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };

        private TextView uploadTextView;
        private ProgressBar uploadProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);

            // Request read & write permission to storage.
            RequestRequiredPermissions();

            uploadTextView = FindViewById<TextView>(Resource.Id.uploadDataTextView);
            uploadProgressBar = FindViewById<ProgressBar>(Resource.Id.uploadProgressBar);

            Task updateSampleDatasTask = Task.Factory.StartNew(() =>
            {
                Collection<string> unLoadDatas = SampleHelper.CollectUnloadDatas(this.Assets, SampleHelper.SampleDataDictionary, SampleHelper.AssetsDataDictionary);
                SampleHelper.UploadDataFiles(this.Assets, SampleHelper.SampleDataDictionary, unLoadDatas, OnCopyingFiles);

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
                    StartActivity(typeof(NavigationDrawerActivity));
                    Finish();
                }, 200);
            });
        }

        private void RequestRequiredPermissions()
        {
            foreach (var permission in StoragePermissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    RequestPermissions(StoragePermissions, RequestStorageId);
                    break;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == RequestStorageId)
            {
                if (grantResults.Any(x => x == Permission.Denied))
                {
                    Toast.MakeText(this, "Storage Permissions Denied", ToastLength.Short).Show();
                }
            }
        }

        private void OnCopyingFiles(string targetPathFilename, int completeCount, int totalCount)
        {
            uploadTextView.Post(() =>
            {
                uploadTextView.Text = string.Format("Copying {0} ({1}/{2})", Path.GetFileName(targetPathFilename), completeCount, totalCount);
                uploadProgressBar.Progress = (int)(completeCount * 100f / totalCount);
            });
        }


    }
}

