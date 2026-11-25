using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace MauiSample
{
    public partial class MainPage : ContentPage
    {
        // How the capitals are drawn: the image named "capital" at every point in the
        // "capitals" layer, kept on screen even where it meets one of the base map's labels.
        // The same style language draws the base map.
        private const string CapitalStyle = """
        {
          "layers": [
            { "type": "symbol",
              "source-layer": "capitals",
              "layout": { "icon-image": "capital", "icon-allow-overlap": true } }
          ]
        }
        """;

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

            // ThinkGeo Cloud Maps as the background. The vector tiles are kept on the
            // device between runs, so a view that has been seen once opens without the network.
            var cloudTiles = new ThinkGeoVectorTileSource("SGjwpxF60knqZi3R0A9RRE3GqiuRA1LsnxHMot7rJ58~")
            {
                VectorTileCache = new FileTileCache(Path.Combine(FileSystem.Current.CacheDirectory, "ThinkGeoVectorTiles"))
            };

            // A shapefile of the world's capitals, served to the map as a layer named "capitals".
            // The shapefile is in Decimal Degrees, while the base map is in Spherical Mercator.
            // It's why the shapefile needs to be reprojected to match the coordinate system of the base map.
            var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "AppData", "WorldCapitals.shp");
            var capitalTiles = new FeatureSourceVectorTileSource();
            capitalTiles.FeatureSources.Add("capitals", new ShapeFileFeatureSource(filePath)
            {
                ProjectionConverter = new ProjectionConverter(4326, 3857)
            });

            // One style holds everything the map draws: the base map first, the capitals over it.
            var style = new MapStyle();
            style.AddStyle(ThinkGeoVectorStyles.Light, cloudTiles);
            style.AddStyle(CapitalStyle, capitalTiles);

            // The image the capitals are drawn with: a white circle with a black outline.
            style.Images.Add("capital", PointStyle.CreateSimpleCircleStyle(GeoColors.White, 8, GeoColors.Black, 2));

            // The map draws the style on the GPU.
            MapView.Basemap = new GpuBasemap(style);

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
