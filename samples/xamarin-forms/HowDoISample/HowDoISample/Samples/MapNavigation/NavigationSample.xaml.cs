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
        private Vertex empireStateBuiilding;

        public NavigationSample()
        {
            var projectionConverter = new ProjectionConverter(4326, 3857);
            projectionConverter.Open();
            empireStateBuiilding = projectionConverter.ConvertToExternalProjection(-73.985665442769, 40.7484366107232);

            InitializeComponent();
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
            var backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay(
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

            IsRotationEnabled = true;

            AddMarker(empireStateBuiilding);

            // Use CenterPoint/MapScale for the map extent. 
            mapView.ExtentSettingMode = ExtentSettingMode.CenterPointAndMapScale;
            MapRotation = -30;
            mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel14.Scale;
            mapView.CenterPoint = new PointShape(empireStateBuiilding);

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

            simpleMarkerOverlay.Markers.Add(marker);
        }

        private async void DefaultExtentButton_OnClicked(object sender, System.EventArgs e)
        {
            mapView.MapRotation = -30;
            mapView.MapScale = mapView.ZoomLevelSet.ZoomLevel14.Scale;
            mapView.CenterPoint = new PointShape(empireStateBuiilding);
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

        public bool IsRotationEnabled
        {
            get => mapView.IsRotationEnabled;
            set
            {
                if (mapView.IsRotationEnabled == value) return;
                mapView.IsRotationEnabled = value;
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