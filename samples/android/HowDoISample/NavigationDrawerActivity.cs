using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;

namespace ThinkGeo.UI.Android.HowDoI
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light")]
    public class NavigationDrawerActivity : AppCompatActivity
    {
        private SampleListAdapter sampleListAdapter;
        private ExpandableListView drawerSampleList;
        private DrawerLayout drawerLayout;
        private ActionBarDrawerToggle drawerToggle;

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // The action bar home/up action should open or close the drawer.
            // ActionBarDrawerToggle will take care of this.
            if (drawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            else if (item.ItemId == Resource.Id.viewSource)
            {
                Intent intent = new Intent(this, typeof(WebViewActivity));

                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (!drawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                MenuInflater.Inflate(Resource.Menu.viewSourceMenu, menu);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NavigationDrawer);

            drawerSampleList = FindViewById<ExpandableListView>(Resource.Id.left_drawer);
            drawerSampleList.SetBackgroundColor(Color.White);

            // set up the drawer's list view with items and click listener
            sampleListAdapter = new SampleListAdapter(this, Assets.Open("SampleList.xml"));
            drawerSampleList.SetAdapter(sampleListAdapter);
            drawerSampleList.ChildClick += ListView_ChildClick;

            // enable ActionBar app icon to behave as action to toggle nav drawer
            this.ActionBar.SetDisplayHomeAsUpEnabled(true);
            this.ActionBar.SetHomeButtonEnabled(true);

            // ActionBarDrawerToggle ties together the the proper interactions
            // between the sliding drawer and the action bar app icon
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.AddDrawerListener(drawerToggle);
            //first launch, show Display a simple map sample
            if (savedInstanceState == null)
            {
                var FragmentTransaction = SupportFragmentManager.BeginTransaction();
                FragmentTransaction.Replace(Resource.Id.content_frame, new BufferShapeSample(), "CurrentFragmentSample");
                FragmentTransaction.Commit();

            }
        }

        private void ListView_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            SampleInfo selectedNode = sampleListAdapter.Samples.ElementAt(e.GroupPosition).Children.ElementAt(e.ChildPosition);
            LoadSample(selectedNode);

            // close the drawer
            drawerLayout.CloseDrawer(drawerSampleList);
        }

        private void LoadSample(SampleInfo sampleNode)
        {
            // update the main content by replacing fragments
            var fragment = Activator.CreateInstance(Type.GetType(sampleNode.ClassType.FullName)) as SampleFragment;

            var FragmentTransaction = SupportFragmentManager.BeginTransaction();
            FragmentTransaction.Replace(Resource.Id.content_frame, fragment, "CurrentFragmentSample");
            FragmentTransaction.Commit();

            // update selected item title
            this.Title = sampleNode.Name;
        }

        /**
	     * When using the ActionBarDrawerToggle, you must call it during
	     * onPostCreate() and onConfigurationChanged()...
	     */

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            // Sync the toggle state after onRestoreInstanceState has occurred.
            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // Pass any configuration change to the drawer toggls
            drawerToggle.OnConfigurationChanged(newConfig);
        }
    }
}


