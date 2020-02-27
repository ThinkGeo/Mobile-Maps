using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThinkGeo.MapSuite.Shapes;

namespace GeometricFunctions
{
    [Activity(Label = "Geometric Functions", Icon = "@drawable/icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private ListView listView;
        private SliderView sliderView;
        private BaseSample activitySample;
        private Collection<BaseSample> samples;
        private List<Tuple<string, int>> listItems;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            ActionBar.Hide();
            listView = FindViewById<ListView>(Resource.Id.sampleListView);
            sliderView = (SliderView)FindViewById(Resource.Id.slider_view);
            ImageButton helpButton = FindViewById<ImageButton>(Resource.Id.sampleListMoreButton);

            InitializeSmapleListView();
            activitySample = samples.FirstOrDefault();
            sliderView.MainView.AddView(samples.FirstOrDefault().GetSampleView());
            helpButton.Click += delegate
            {
                Intent helpIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.thinkgeo.com"));
                StartActivity(helpIntent);
            };
        }

        private void InitializeSmapleListView()
        {
            samples = new Collection<BaseSample>();
            listItems = new List<Tuple<string, int>>();
            XDocument xDoc = XDocument.Load(Assets.Open("GeometricFunctions.xml"));
            if (xDoc.Root != null)
            {
                foreach (var element in xDoc.Root.Elements())
                {
                    string image = element.Attribute("Image").Value;
                    string className = element.Attribute("Class").Value;
                    string name = element.Attribute("Name").Value;
                    XElement geometrySourceElement = element.Element("GeometrySource");

                    BaseSample sample = (BaseSample)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType("GeometricFunctions." + className), this, sliderView);
                    sample.TitleText = name;
                    samples.Add(sample);

                    foreach (var featureWktElement in geometrySourceElement.Elements())
                    {
                        if (!string.IsNullOrEmpty(featureWktElement.Value))
                        {
                            sample.GeometrySource.Add(new Feature(featureWktElement.Value));
                        }
                    }

                    int resouceId = (int)typeof(Resource.Drawable).GetField(image).GetValue(null);
                    listItems.Add(new Tuple<string, int>(name, resouceId));
                }
            }
            ActivityListItemAdapter adapter = new ActivityListItemAdapter(this, listItems);
            listView.Adapter = adapter;
            listView.ItemClick += sampleListView_ItemClick;
        }

        private void sampleListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            sliderView.SetSlided(false);

            activitySample.DisposeMap();
            sliderView.MainView.RemoveAllViews();
            sliderView.MainView.AddView(samples[e.Position].GetSampleView());
            activitySample = samples[e.Position];
        }
    }
}