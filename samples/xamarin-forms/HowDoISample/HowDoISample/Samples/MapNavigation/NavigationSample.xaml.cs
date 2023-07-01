using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to programmatically zoom, pan, and rotate the map control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationSample : ContentPage
    {
        private Vertex empireStateBuilding;
        private PopupOverlay popupOverlay;
        private ThinkGeoCloudRasterMapsOverlay backgroundOverlay;

        public NavigationSample()
        {
            var projectionConverter = new ProjectionConverter(4326, 3857);
            projectionConverter.Open();
            empireStateBuilding = projectionConverter.ConvertToExternalProjection(-73.985665442769, 40.7484366107232);

            InitializeComponent();

            popupOverlay = new PopupOverlay();
            mapView.Overlays.Add(popupOverlay);
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudRasterMapsMapType.Light_V2_X2);
            // ThinkGeoCloudRasterMapsOverlay includes a default cache; however, we recommend specifying the cache location in your code to ensure you have control over its storage location.
            // This allows you to easily manage and access the cache as needed.
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoCloudRasterMaps");

            // You can also utilize ThinkGeo's vector maps overlay. Keep in mind that this option may result in slower performance since the tiles will be rendered on the client-side.
            // However, the advantage is that it will provide a sharper appearance due to the vector rendering.
            //var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
            //    "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
            //    "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");


            mapView.Overlays.Add(backgroundOverlay);

            mapView.CurrentExtentChanged += MapView_CurrentExtentChanged;

            RotationEnabled = true;

            AddMarker(empireStateBuilding);

            // Use CenterPoint/MapScale for the map extent. 
            MapRotation = -30;
            mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel14.Scale;
            mapView.CenterPoint = new PointShape(empireStateBuilding);

            await mapView.RefreshAsync();
        }

        private void AddMarker(Vertex position)
        {
            var simpleMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);

            var marker = new Marker
            {
                Position = new PointShape(position),
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/AQUA.png"),
                YOffset = -17
            };
            marker.TagObject = "Empire State Building";
            marker.Tap += Marker_Tap;


            simpleMarkerOverlay.Markers.Add(marker);
        }

        private async void Marker_Tap(object sender, EventArgs e)
        {
            var marker = (Marker)sender;
            var popup = new Popup
            {
                Position = marker.Position,
                Text = marker.TagObject.ToString()
            };
            popupOverlay.Popups.Clear();
            popupOverlay.Popups.Add(popup);
            await popupOverlay.RefreshAsync();
        }

        private async void DefaultExtentButton_OnClicked(object sender, System.EventArgs e)
        {
            mapView.MapRotation = -30;
            mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel14.Scale;
            mapView.CenterPoint = new PointShape(empireStateBuilding);
            await mapView.RefreshAsync();
        }

        private async void NorthUpButton_OnClicked(object sender, System.EventArgs e)
        {
            mapView.MapRotation = 0;
            await mapView.RefreshAsync();
        }

        private void MapView_CurrentExtentChanged(object sender, CurrentExtentChangedMapViewEventArgs e)
        {
            MapRotation = mapView.MapRotation;
        }

        public double MapRotation
        {
            get
            {
                if (mapView == null)
                    return 0;
                return mapView.MapRotation;
            }
            set
            {
                if (mapView == null)
                    return;

                mapView.MapRotation = value;
                OnPropertyChanged();
            }
        }

        public bool RotationEnabled
        {
            get => mapView.RotationEnabled;
            set
            {
                if (mapView.RotationEnabled == value) return;
                mapView.RotationEnabled = value;
                OnPropertyChanged();
            }
        }


        private bool _darkTheme;

        public bool DarkTheme
        {
            get => _darkTheme;
            set
            {
                if (_darkTheme == value) return;

                _darkTheme = value;

                // Add Cloud Maps as a background overlay
                backgroundOverlay.MapType = _darkTheme
                    ? ThinkGeoCloudRasterMapsMapType.Dark_V2_X2
                    : ThinkGeoCloudRasterMapsMapType.Light_V2_X2;

                backgroundOverlay.RefreshAsync();

                OnPropertyChanged();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mapView.Overlays.Clear();
        }
    }
}