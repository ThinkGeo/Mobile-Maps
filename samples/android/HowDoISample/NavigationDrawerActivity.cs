using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ThinkGeo.UI.Android.HowDoI
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light")]
    public class NavigationDrawerActivity : AppCompatActivity
    {
        private SampleExpandableListAdapter sampleListAdapter;
        private ExpandableListView sampleList;
        private DrawerLayout drawerLayout;

        /// <summary>
        /// Prepares the main view of the application
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view to the NavigationDrawer layout
            SetContentView(Resource.Layout.NavigationDrawer);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            sampleList = FindViewById<ExpandableListView>(Resource.Id.sample_list);

            drawerLayout.SetBackgroundColor(Color.White);

            // Load the list of available samples
            List<SampleCategory> samples;
            using (StreamReader sr = new StreamReader(Assets.Open("samples.json")))
            {
                var serializer = new JsonSerializer();
                samples = serializer.Deserialize(sr, typeof(List<SampleCategory>)) as List<SampleCategory>;
            }

            // Create and add the list adapter to the ExpandableListView
            sampleListAdapter = new SampleExpandableListAdapter(this, samples);
            sampleList.SetAdapter(sampleListAdapter);

            // Setup a Click event handler on SampleListItems
            sampleList.ChildClick += ListView_ChildClick;

            // Load the first sample as the starting sample
            var firstSampleListItem = sampleListAdapter.Categories[0].Children[0];
            LoadSample(firstSampleListItem);
        }

        /// <summary>
        /// Close the drawer if it is open when the back button is pressed
        /// </summary>
        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                drawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        /// <summary>
        /// Inflate the top-right options menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the viewSourceMenu and apply it to the given menu
            MenuInflater.Inflate(Resource.Menu.viewSourceMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Click event handler that loads the sample fragment of the selected SampleListItem from the expandable list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            var selectedSampleListItem = sampleListAdapter.Categories[e.GroupPosition].Children[e.ChildPosition];

            // Load the sample fragment
            LoadSample(selectedSampleListItem);

            // Close the drawer
            drawerLayout.CloseDrawer(GravityCompat.Start);
        }

        /// <summary>
        /// Loads and displays a sample fragment
        /// </summary>
        /// <param name="sampleListItem"></param>
        private void LoadSample(SampleListItem sampleListItem)
        {
            // Create an instance of the sample fragment using the sampleListItem's ID
            var fragment = Activator.CreateInstance(Type.GetType(sampleListItem.Id)) as SampleFragment;

            // Update the toolbar title to the sample's title
            this.Title = sampleListItem.Title;

            // Replace the current sample fragment with the new instance
            var FragmentTransaction = SupportFragmentManager.BeginTransaction();
            FragmentTransaction.Replace(Resource.Id.content_frame, fragment, "CurrentFragmentSample");
            FragmentTransaction.Commit();
        }
    }
}
