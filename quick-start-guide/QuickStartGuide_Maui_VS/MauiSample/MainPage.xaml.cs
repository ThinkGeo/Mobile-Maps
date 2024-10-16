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

            // Add a shapefile layer with point style.
            var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "AppData", "WorldCapitals.shp");
            var capitalLayer = new ShapeFileFeatureLayer(filePath);
            var capitalStyle = new PointStyle()
            {
                SymbolType = PointSymbolType.Circle,
                SymbolSize = 8,
                FillBrush = new GeoSolidBrush(GeoColors.White),
                OutlinePen = new GeoPen(GeoColors.Black, 2)
            };
            // Each layer has 20 preset zoomlevels. Here we set the capitalStyle for ZoomLevel01 and apply the style to the other preset zoomlevels.
            capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = capitalStyle;
            capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // The shapefile is in Decimal Degrees, while the base overlay is in Spherical Mercator.
            // It's why the shapefile needs to be reprojected to match the coordinate system of the base overlay.
            capitalLayer.FeatureSource.ProjectionConverter =
                new ProjectionConverter(Projection.GetDecimalDegreesProjString(), Projection.GetSphericalMercatorProjString());

            // Add the layer to an overlay, add that overlay to the map.
            var customDataOverlay = new LayerOverlay();
            customDataOverlay.Layers.Add(capitalLayer);
            MapView.Overlays.Add(customDataOverlay);

            // set up the map extent and refresh
            MapView.CenterPoint = new PointShape(450061, 1074668);
            MapView.MapScale = 74000000;

            await MapView.RefreshAsync();
        }
    }

}
