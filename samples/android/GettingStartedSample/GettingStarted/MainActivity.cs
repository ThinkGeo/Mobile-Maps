using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the main window.
    /// </summary>
    [Activity(Label = "Getting Started", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity, ILocationListener
    {
        private bool isTracking;
        private float locationAccuracy;
        private string bestGpsProvider;
        private Marker locationMarker;
        private MarkerOverlay gpsOverlay;
        private PopupOverlay popupOverlay;
        private Animation toolsBarOutAnimation;
        private Animation toolsBarInAnimation;
        private LocationManager locationManager;
        private ProjectionConverter wgs84ToMeterProjection;
        private ScaleZoomLevelMapTool scaleZoomLevelMapTool;
        private SelectBaseMapTypeDialog selectBaseMapTypeDialog;

        readonly string[] LocationPermissions =
{
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        const int RequestLocationId = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
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
                RequestPermissions(LocationPermissions, RequestLocationId);
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
                case RequestLocationId:
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
            //Initialize the Map and relative information.
            InitializeGlobalVariables();
            InitializeAndroidMap();
            InitializeAnimations();

            selectBaseMapTypeDialog = new SelectBaseMapTypeDialog(this);

            LinearLayout toolsBarHeaderLayout = FindViewById<LinearLayout>(Resource.Id.ToolsBarHeaderLayout);
            ImageButton locationImageButon = FindViewById<ImageButton>(Resource.Id.locationImageButton);
            ImageButton fullExtentImageButton = FindViewById<ImageButton>(Resource.Id.fullextImageButton);
            ImageButton previousImageButton = FindViewById<ImageButton>(Resource.Id.preextImageButton);
            ImageButton nextImageButton = FindViewById<ImageButton>(Resource.Id.nxtextImageButton);
            ImageButton infoImageButton = FindViewById<ImageButton>(Resource.Id.infoImageButton);
            ImageButton baseMapButton = FindViewById<ImageButton>(Resource.Id.baseMapButton);

            //Register the events for controls.
            toolsBarHeaderLayout.Click += ToolsBarHeaderLayoutClick;
            locationImageButon.Click += LocationImageButtonClick;
            fullExtentImageButton.Click += FullExtentImageButtonClick;
            previousImageButton.Click += PreviousImageButtonClick;
            nextImageButton.Click += NextImageButtonClick;
            infoImageButton.Click += InfoImageButtonClick;
            baseMapButton.Click += BaseMapButtonClick;

            RefreshToolsBarWithConfig(Resources.Configuration);
            CheckGpsServiceAndOpenSettings();
        }
        

        /// <summary>
        /// Called by the system when the device configuration changes while your
        /// activity is running.
        /// </summary>
        /// <param name="newConfig">The new device configuration.</param>
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            FrameLayout toolsBarBorderLayout = FindViewById<FrameLayout>(Resource.Id.ToolsBarBorderFrameLayout);
            bool isVisible = toolsBarBorderLayout.Visibility == ViewStates.Visible;

            RefreshToolsBarWithConfig(newConfig);
            base.OnConfigurationChanged(newConfig);

            float centerCoordinateToolHeight = isVisible ? CustomMapView.Current.MapTools.CenterCoordinate.GetY() : CustomMapView.Current.MapTools.CenterCoordinate.GetY() - 50 * MapView.DisplayDensity;
            CustomMapView.Current.MapTools.CenterCoordinate.SetY(centerCoordinateToolHeight);

            float scaleZoomLevelMapToolHeight = isVisible ? scaleZoomLevelMapTool.GetY() : scaleZoomLevelMapTool.GetY() - 50 * MapView.DisplayDensity;
            scaleZoomLevelMapTool.SetY(scaleZoomLevelMapToolHeight);

            if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                CustomMapView.Current.MapTools.CenterCoordinate.SetY(CustomMapView.Current.MapTools.CenterCoordinate.GetY() - 30 * MapView.DisplayDensity);
                scaleZoomLevelMapTool.SetY(scaleZoomLevelMapTool.GetY() - 30 * MapView.DisplayDensity);

            }
            else if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                CustomMapView.Current.MapTools.CenterCoordinate.SetY(CustomMapView.Current.MapTools.CenterCoordinate.GetY() + 30 * MapView.DisplayDensity);
                scaleZoomLevelMapTool.SetY(scaleZoomLevelMapTool.GetY() + 30 * MapView.DisplayDensity);
            }
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode you started it with, the resultCode it returned, and any additional data from it.
        /// </summary>
        /// <param name="requestCode">The integer request code originally supplied to
        /// startActivityForResult(), allowing you to identify who this
        /// result came from.</param>
        /// <param name="resultCode">The integer result code returned by the child activity
        /// through its setResult().</param>
        /// <param name="data">An Intent, which can return result data to the caller
        /// (various data can be attached to Intent "extras").</param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 100)
            {
                if (CheckGpsService())
                {
                    StartGpsTracking();
                    RefreshGpsLocation(locationManager.GetLastKnownLocation(bestGpsProvider));
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void OnLocationChanged(Location location)
        {
            RefreshGpsLocation(location);
        }

        public void OnProviderDisabled(string provider)
        {
            RefreshGpsLocation(null);
        }

        public void OnProviderEnabled(string provider)
        {
            RefreshGpsLocation(locationManager.GetLastKnownLocation(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            switch (status)
            {
                case Availability.OutOfService:
                    Toast.MakeText(this, "Out Of Service!", ToastLength.Long);
                    break;
                case Availability.TemporarilyUnavailable:
                    Toast.MakeText(this, "Pause the Gps service!", ToastLength.Long);
                    break;
            }
        }


        private void BaseMapButtonClick(object sender, EventArgs e)
        {
            selectBaseMapTypeDialog.Show();
        }

        private void InfoImageButtonClick(object sender, EventArgs e)
        {
            StartActivity(typeof(InfoActivity));
        }

        private void NextImageButtonClick(object sender, EventArgs e)
        {
            CustomMapView.Current.ZoomToNextExtent();
        }

        private void PreviousImageButtonClick(object sender, EventArgs e)
        {
            CustomMapView.Current.ZoomToPreviousExtent();
        }

        private void FullExtentImageButtonClick(object sender, EventArgs e)
        {
            CustomMapView.Current.CurrentExtent = CustomMapView.Current.Overlays["ThinkGeoCloudMapsOverlay"].GetBoundingBox();
            CustomMapView.Current.Refresh();
        }

        private void LocationImageButtonClick(object sender, EventArgs e)
        {
            if (locationMarker != null) CustomMapView.Current.ZoomTo(locationMarker.Position, CustomMapView.Current.ZoomLevelSet.ZoomLevel15.Scale);
        }

        private void ToolsBarHeaderLayoutClick(object sender, EventArgs e)
        {
            LinearLayout toolsBarLayout = FindViewById<LinearLayout>(Resource.Id.ToolsBarLinearLayout);
            FrameLayout toolsBarBorderLayout = FindViewById<FrameLayout>(Resource.Id.ToolsBarBorderFrameLayout);
            bool isVisible = toolsBarBorderLayout.Visibility == ViewStates.Visible;

            float centerCoordinateToolHeight = isVisible ? CustomMapView.Current.MapTools.CenterCoordinate.GetY() + 50 * MapView.DisplayDensity : CustomMapView.Current.MapTools.CenterCoordinate.GetY() - 50 * MapView.DisplayDensity;
            CustomMapView.Current.MapTools.CenterCoordinate.SetY(centerCoordinateToolHeight);

            float scaleZoomLevelMapToolHeight = isVisible ? scaleZoomLevelMapTool.GetY() + 50 * MapView.DisplayDensity : scaleZoomLevelMapTool.GetY() - 50 * MapView.DisplayDensity;
            scaleZoomLevelMapTool.SetY(scaleZoomLevelMapToolHeight);

            if (isVisible)
            {
                toolsBarLayout.StartAnimation(toolsBarOutAnimation);
            }
            else
            {
                toolsBarLayout.StartAnimation(toolsBarInAnimation);
            }
            toolsBarBorderLayout.Visibility = ViewStates.Invisible;
        }

        private void mapView_MapSingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            Popup popup = CreateNewPopup(e);

            popupOverlay.Popups.Clear();
            popupOverlay.Popups.Add(popup);
            popupOverlay.Refresh();
        }

        private void mapView_MapDoubleTap(object sender, DoubleTapMapViewEventArgs e)
        {
            CustomMapView.Current.ZoomInByAnchorPoint(new ScreenPointF(e.ScreenX, e.ScreenY));
        }

        private void mapView_MapLongPress(object sender, LongPressMapViewEventArgs e)
        {
            SelectZoomLevelListDialog selectZoomLevelDialog = new SelectZoomLevelListDialog(this, CustomMapView.Current);
            selectZoomLevelDialog.Show();

            selectZoomLevelDialog.CancelEvent += selectZoomLevelDialogCancelEvent;
        }

        private void mapView_CurrentScaleChanged(object sender, CurrentScaleChangedMapViewEventArgs e)
        {
            if (locationMarker != null)
            {
                double mapResolution = Math.Max(CustomMapView.Current.CurrentExtent.Width / CustomMapView.Current.Width, CustomMapView.Current.CurrentExtent.Height / CustomMapView.Current.Height);
                int radius = (int)(locationAccuracy / mapResolution);
                ((GpsMarker)locationMarker).AccuracyRadius = radius;
            }
        }

        private void selectZoomLevelDialogCancelEvent(object sender, EventArgs e)
        {
            SelectZoomLevelListDialog dialog = (SelectZoomLevelListDialog)sender;
            if (dialog.CurrentZoomLevel != null)
            {
                CustomMapView.Current.ZoomToScale(dialog.CurrentZoomLevel.Scale);
            }
        }

        private void StartGpsTracking()
        {
            if (!isTracking)
            {
                isTracking = true;
                locationManager.RequestLocationUpdates(bestGpsProvider, 5 * 1000, 5, this);
            }
        }

        /// <summary>
        /// Creates the new popup.
        /// </summary>
        /// <param name="e">The Motion Event.</param>
        /// <returns>Returns the Popup.</returns>
        private Popup CreateNewPopup(SingleTapMapViewEventArgs e)
        {
            Popup popup = new Popup(this);
            PointF location = new PointF(e.ScreenX, e.ScreenY);
            popup.Position = MapUtil.ToWorldCoordinate(CustomMapView.Current.CurrentExtent, location.X, location.Y, (float)CustomMapView.Current.Width, (float)CustomMapView.Current.Height);

            TextView textView = new TextView(this);
            PointShape locationShape = wgs84ToMeterProjection.ConvertToInternalProjection(popup.Position) as PointShape;
            textView.Text = $"X : {locationShape.X:N2}" + "\r\n" + $"Y : {locationShape.Y:N2}";
            textView.SetTextColor(Color.Black);
            textView.SetTextSize(ComplexUnitType.Px, 30);

            LinearLayout.LayoutParams btnLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            btnLayoutParams.Gravity = GravityFlags.CenterVertical;

            ImageButton button = new ImageButton(this);
            button.LayoutParameters = btnLayoutParams;
            button.SetBackgroundResource(Resource.Layout.ButtonBackgroundSelector);
            button.SetImageResource(Resource.Drawable.close);
            button.Click += (s, clickEvent) =>
            {
                popupOverlay.Popups.Clear();
                popupOverlay.Refresh();
            };

            LinearLayout linearLayout = new LinearLayout(this);
            linearLayout.SetPadding(8, 0, 0, 0);
            linearLayout.Orientation = Android.Widget.Orientation.Horizontal;
            linearLayout.SetGravity(GravityFlags.CenterHorizontal);
            linearLayout.AddView(textView);
            linearLayout.AddView(button);
            popup.AddView(linearLayout);

            return popup;
        }

        /// <summary>
        /// Refreshes the GPS marker when location is updated.
        /// </summary>
        /// <param name="gpsLocation">The GPS location.</param>
        private void RefreshGpsLocation(Location gpsLocation)
        {
            if (gpsLocation != null)
            {
                PointShape location = new PointShape(gpsLocation.Longitude, gpsLocation.Latitude);
                location = wgs84ToMeterProjection.ConvertToExternalProjection(location) as PointShape;
                locationAccuracy = gpsLocation.Accuracy;

                if (locationMarker == null)
                {
                    locationMarker = new GpsMarker(this);
                    gpsOverlay.Markers.Add(locationMarker);
                }

                double mapResolution = Math.Max(CustomMapView.Current.CurrentExtent.Width / CustomMapView.Current.Width, CustomMapView.Current.CurrentExtent.Height / CustomMapView.Current.Height);
                int radius = (int)(gpsLocation.Accuracy / mapResolution);
                ((GpsMarker)locationMarker).AccuracyRadius = radius;
                locationMarker.Position = location;
                if (CustomMapView.Current.Width != 0 && CustomMapView.Current.Height != 0)
                {
                    CustomMapView.Current.CenterAt(location);
                }
            }
        }

        /// <summary>
        /// Refreshes the tools bar with configuration changes.
        /// </summary>
        /// <param name="newConfig">The new configuration.</param>
        private void RefreshToolsBarWithConfig(Configuration newConfig)
        {
            LinearLayout toolsBarButtonsLayout = FindViewById<LinearLayout>(Resource.Id.ToolButtonsLayout);
            LinearLayout toolsBarHeaderLayout = FindViewById<LinearLayout>(Resource.Id.ToolsBarHeaderLayout);
            FrameLayout toolsBarBorderFrameLayout = FindViewById<FrameLayout>(Resource.Id.ToolsBarBorderFrameLayout);
            ImageButton infoButton = FindViewById<ImageButton>(Resource.Id.infoImageButton);
            TextView landscapeTextView = FindViewById<TextView>(Resource.Id.landscapeTextView);

            // If the Orientation is Portrait, refresh the layout for the new configuration.
            if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                landscapeTextView.Visibility = ViewStates.Gone;
                toolsBarHeaderLayout.Visibility = ViewStates.Visible;
                FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                lp.Gravity = GravityFlags.Left;
                FrameLayout.LayoutParams rightLp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                rightLp.Gravity = GravityFlags.Right;

                if (toolsBarButtonsLayout.FindViewById(Resource.Id.infoImageButton) == infoButton)
                {
                    toolsBarButtonsLayout.RemoveView(infoButton);
                    toolsBarBorderFrameLayout.AddView(infoButton);
                    infoButton.LayoutParameters = rightLp;
                }

                toolsBarButtonsLayout.LayoutParameters = lp;
            }
            // If the Orientation is Landscape, refresh the layout for the new configuration.
            else if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                landscapeTextView.Visibility = ViewStates.Visible;
                toolsBarHeaderLayout.Visibility = ViewStates.Gone;
                toolsBarBorderFrameLayout.Visibility = ViewStates.Visible;
                FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                lp.Gravity = GravityFlags.Right;

                if (toolsBarButtonsLayout.FindViewById(Resource.Id.infoImageButton) != infoButton)
                {
                    toolsBarBorderFrameLayout.RemoveView(infoButton);
                    toolsBarButtonsLayout.AddView(infoButton);
                }

                toolsBarButtonsLayout.LayoutParameters = lp;
            }
        }

        /// <summary>
        /// Initializes the global variables.
        /// </summary>
        private void InitializeGlobalVariables()
        {
            Criteria criteria = new Criteria();
            criteria.Accuracy = Accuracy.Coarse;
            criteria.PowerRequirement = Power.Low;
            criteria.AltitudeRequired = false;
            criteria.BearingRequired = false;
            criteria.SpeedRequired = false;
            criteria.CostAllowed = true;

            isTracking = false;
            locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bestGpsProvider = locationManager.GetBestProvider(criteria, true);
            wgs84ToMeterProjection = new ProjectionConverter();
            wgs84ToMeterProjection.InternalProjection = new Projection(Projection.GetWgs84ProjString());
            wgs84ToMeterProjection.ExternalProjection = new Projection(Projection.GetGoogleMapProjString());
            wgs84ToMeterProjection.Open();
        }

        /// <summary>
        /// Initializes the android map.
        /// </summary>
        private void InitializeAndroidMap()
        {
            FrameLayout mapContainer = FindViewById<FrameLayout>(Resource.Id.MapContainer);
            mapContainer.AddView(CustomMapView.Current, 0, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            CustomMapView.Current.MapUnit = GeographyUnit.Meter;
            CustomMapView.Current.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            CustomMapView.Current.CurrentExtent = new RectangleShape(-19062735.6816748, 9273256.52450252, -5746827.16371793, 2673516.56066139);
            CustomMapView.Current.SetBackgroundColor(new Color(255, 244, 242, 238));

            //WorldMapKitOverlay worldMapKitOverlay = new WorldMapKitOverlay();
            //worldMapKitOverlay.Projection = WorldMapKitProjection.SphericalMercator;
            //worldMapKitOverlay.ClientId = "ThinkGeo";
            //worldMapKitOverlay.PrivateKey = "MWSN2234230+SDFADS(AADS(A23werq@#$@";
            //mapView.Overlays.Add("WorldMapKitOverlay", worldMapKitOverlay);

            gpsOverlay = new MarkerOverlay();
            CustomMapView.Current.Overlays.Add("GpsOverlay", gpsOverlay);

            popupOverlay = new PopupOverlay();
            CustomMapView.Current.Overlays.Add("popupOverlay", popupOverlay);

            CustomMapView.Current.LongPress += mapView_MapLongPress;
            CustomMapView.Current.SingleTap += mapView_MapSingleTap;
            CustomMapView.Current.DoubleTap += mapView_MapDoubleTap;
            CustomMapView.Current.CurrentScaleChanged += mapView_CurrentScaleChanged;

            scaleZoomLevelMapTool = new ScaleZoomLevelMapTool(CustomMapView.Current, this);
            scaleZoomLevelMapTool.IsEnabled = true;
            RelativeLayout.LayoutParams scaleZoomLevelMapToolLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            scaleZoomLevelMapToolLayoutParams.AddRule(LayoutRules.AlignParentRight);
            scaleZoomLevelMapToolLayoutParams.AddRule(LayoutRules.AlignParentBottom);
            scaleZoomLevelMapTool.LayoutParameters = scaleZoomLevelMapToolLayoutParams;
            scaleZoomLevelMapTool.SetY(-100 * MapView.DisplayDensity);
            CustomMapView.Current.MapTools.Add(scaleZoomLevelMapTool);

            CustomMapView.Current.MapTools.CenterCoordinate.IsEnabled = true;
            CustomMapView.Current.MapTools.CenterCoordinate.SetY(-100 * MapView.DisplayDensity);
        }

        /// <summary>
        /// Initializes the animations for toolsBar.
        /// </summary>
        private void InitializeAnimations()
        {
            toolsBarInAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.toolsBar_in);
            toolsBarOutAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.toolsBar_out);

            toolsBarInAnimation.AnimationEnd += (sender, e) =>
            {
                FrameLayout toolsBarBorderLayout = FindViewById<FrameLayout>(Resource.Id.ToolsBarBorderFrameLayout);

                if (toolsBarBorderLayout != null)
                {
                    toolsBarBorderLayout.Visibility = ViewStates.Visible;
                }
            };

            toolsBarOutAnimation.AnimationEnd += (sender, e) =>
            {
                FrameLayout toolsBarBorderLayout = FindViewById<FrameLayout>(Resource.Id.ToolsBarBorderFrameLayout);

                if (toolsBarBorderLayout != null)
                {
                    toolsBarBorderLayout.Visibility = ViewStates.Gone;
                }
            };
        }

        /// <summary>
        /// Checks the GPS service is enable or not.
        /// </summary>
        private bool CheckGpsService(bool withToast = false)
        {
            bool result = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

            if (withToast && !result)
            {
                Toast.MakeText(this, "Please Open Gps Service!", ToastLength.Long).Show();
            }

            return result;
        }

        /// <summary>
        /// Checks the GPS service, if it is enable, start GPS Tracking, if not, open GPS setting dialog.
        /// </summary>
        private void CheckGpsServiceAndOpenSettings()
        {
            if (!CheckGpsService())
            {
                OpenGpsSettingsDialog gpsSettingDialog = new OpenGpsSettingsDialog(this);
                gpsSettingDialog.Show();

                gpsSettingDialog.CancelEvent += (sender, e) =>
                {
                    if (gpsSettingDialog.OpenSettings)
                    {
                        Intent intent = new Intent("android.settings.LOCATION_SOURCE_SETTINGS");
                        StartActivityForResult(intent, 100);
                    }
                };
            }
            else
            {
                StartGpsTracking();
                RefreshGpsLocation(locationManager.GetLastKnownLocation(bestGpsProvider));
            }
        }
    }
}