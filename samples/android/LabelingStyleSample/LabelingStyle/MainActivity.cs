using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;

namespace LabelingStyle
{
    [Activity(Label = "Labeling", Icon = "@drawable/Icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private BaseSample currentSample;
        private ListView sampleListView;
        private SliderView sampleListContainer;
        private Collection<BaseSample> samples;

        readonly string[] StoragePermissions =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        const int RequestStorageId = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Main);

            TryInitializeSampleAsync();
        }

        async Task TryInitializeSampleAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await InitializeSampleAsync();
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
                InitializeSampleAsync();
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
                            await InitializeSampleAsync();
                        }
                    }
                    break;
            }
        }

        async Task InitializeSampleAsync()
        {

            sampleListContainer = FindViewById<SliderView>(Resource.Id.slider_view);
            sampleListView = FindViewById<ListView>(Resource.Id.sampleListView);

            InitializeSmapleListView();

            currentSample = samples.FirstOrDefault();
            currentSample.UpdateSampleLayout();
            sampleListContainer.MainView.AddView(currentSample.SampleView);

            FindViewById<ImageButton>(Resource.Id.sampleListMoreButton).Click += OnMoreButtonClick;
        }

        private void sampleListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            currentSample.DisposeMap();
            currentSample = samples[e.Position];
            currentSample.UpdateSampleLayout();

            sampleListContainer.SetSlided(false);
            sampleListContainer.MainView.RemoveAllViews();
            sampleListContainer.MainView.AddView(currentSample.SampleView);
        }

        private void InitializeSmapleListView()
        {
            samples = new Collection<BaseSample>();
            XDocument xDoc = XDocument.Load(Assets.Open("StyleList.xml"));
            if (xDoc.Root != null)
            {
                foreach (var element in xDoc.Root.Elements())
                {
                    string image = element.Attribute("Image").Value;
                    string className = element.Attribute("Class").Value;
                    string name = element.Attribute("Name").Value;

                    BaseSample sample = (BaseSample)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType("LabelingStyle." + className), this);
                    sample.Title = name;
                    sample.ImageId = (int)typeof(Resource.Drawable).GetField(image).GetValue(null);
                    sample.SampleListButtonClick += (s, e) => sampleListContainer.SetSlided(!sampleListContainer.IsSlided());

                    samples.Add(sample);
                }
            }

            SampleListItemAdapter adapter = new SampleListItemAdapter(this, samples);
            sampleListView.Adapter = adapter;
            sampleListView.ItemClick += sampleListView_ItemClick;
        }

        private void OnMoreButtonClick(object sender, EventArgs eventArgs)
        {
            Intent helpIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.thinkgeo.com"));
            StartActivity(helpIntent);
        }
    }
}

