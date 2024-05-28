using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace MauiSample
{
    public partial class MainPage : ContentPage
    {
        private bool _initialized;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void MapView_OnSizeChanged(object sender, EventArgs e)
        {
            if (_initialized)
                return;
            _initialized = true;

            // Set the map's unit of measurement to meters(Spherical Mercator)
            MapView.MapUnit = GeographyUnit.Meter;

            // Add ThinkGeo Cloud Maps as the background 
            var backgroundOverlay = new ThinkGeoVectorOverlay
            {
                ClientId = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                ClientSecret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~",
                MapType = ThinkGeoCloudVectorMapsMapType.Light,
                TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
            };
            MapView.Overlays.Add(backgroundOverlay);

            // set up the map rotation and map tools
            MapView.IsRotationEnabled = true;
            MapView.MapTools.Add(new ZoomMapTool());

            // set up the map extent and refresh
            MapView.CenterPoint = new PointShape(450061, 1074668);
            MapView.MapScale = 74000000;

            await MapView.RefreshAsync();
        }
    }

}
