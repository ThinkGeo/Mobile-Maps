using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteSiteSelection
{
    [Activity(Label = "Site Selection", Icon = "@drawable/sampleIcon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private RadioButton panRadioButton;
        private RadioButton drawPointButton;
        private FilterByTypeDialog filterByTypeDialog;
        private FilterByAreaDialog filterByAreaDialog;
        private SelectBaseMapTypeDialog selectBaseMapTypeDialog;

        readonly string[] StoragePermissions =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        const int RequestStorageId = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            TryShowMapAsync();
        }

        async Task TryShowMapAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await ShowMapAsync();
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
                ShowMapAsync();
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
                            await ShowMapAsync();
                        }
                    }
                    break;
            }
        }

        async Task ShowMapAsync()
        {
            FrameLayout mapContainer = FindViewById<FrameLayout>(Resource.Id.MapContainer);
            mapContainer.AddView(SampleMapView.Current, 0, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            InitializeDialogs();

            panRadioButton = FindViewById<RadioButton>(Resource.Id.PanButton);
            drawPointButton = FindViewById<RadioButton>(Resource.Id.DrawPointButton);
            Button clearButton = FindViewById<Button>(Resource.Id.ClearButton);
            Button searchButton = FindViewById<Button>(Resource.Id.searchButton);

            panRadioButton.CheckedChange += PanRadioButton_CheckedChange;
            drawPointButton.CheckedChange += DrawPointButton_CheckedChange;
            clearButton.Click += ClearButton_Click;
            searchButton.Click += SearchButton_Click;
        }

        private void SearchButton_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(PotentialSimilarSitesActivity));
        }

        private void ClearButton_Click(object sender, System.EventArgs e)
        {
            SampleMapView.Current.ClearQueryResult();
        }

        private void DrawPointButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (drawPointButton.Checked)
            {
                SampleMapView.Current.TrackOverlay.TrackMode = TrackMode.Point;
            }
        }

        private void PanRadioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (panRadioButton.Checked)
            {
                SampleMapView.Current.TrackOverlay.TrackMode = TrackMode.None;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(Menu.None, Menu.First + 1, 1, "Base Map").SetIcon(Resource.Drawable.basemap);
            menu.Add(Menu.None, Menu.First + 2, 1, "Filter by type").SetIcon(Resource.Drawable.FilterByType);
            menu.Add(Menu.None, Menu.First + 3, 1, "Filter by area").SetIcon(Resource.Drawable.FilterByArea);
            menu.Add(Menu.None, Menu.First + 4, 1, "View Potential Similar Sites").SetIcon(Resource.Drawable.searchicon);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Menu.First + 1:
                    selectBaseMapTypeDialog.Show();
                    break;
                case Menu.First + 2:
                    filterByTypeDialog.Show();
                    break;
                case Menu.First + 3:
                    filterByAreaDialog.Show();
                    break;
                case Menu.First + 4:
                    StartActivity(typeof(PotentialSimilarSitesActivity));
                    break;
            }
            return false;
        }

        private void InitializeDialogs()
        {
            selectBaseMapTypeDialog = new SelectBaseMapTypeDialog(this);
            filterByTypeDialog = new FilterByTypeDialog(this);
            filterByAreaDialog = new FilterByAreaDialog(this);
        }
    }
}

